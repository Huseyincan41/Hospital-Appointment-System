using Data.Migrations;
using Entity.Entities;
using Entity.Services;
using Entity.ViewModels;
using Microsoft.AspNetCore.Mvc;
using NuGet.Protocol.Core.Types;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace HospitalAppointmentSystem.Controllers
{
    public class AppointmentController : Controller
    {
        private readonly IAppointmentService _appointmentService;
        private readonly IDepartmentService _departmentService;
        private readonly IDoctorService _doctorService;
        public AppointmentController(IAppointmentService appointmentService, IDepartmentService departmentService, IDoctorService doctorService)
        {
            _appointmentService = appointmentService;
            _departmentService = departmentService;
            _doctorService = doctorService;
        }
        [HttpGet]
        public async Task<IActionResult> Create(int? departmentId = null)
        {
            var departments = await _departmentService.GetAllAsync();
            var doctors = departmentId.HasValue
        ? await _doctorService.GetDoctorsByDepartmentIdAsync(departmentId.Value)
        : new List<Doctor>();
            var availableTimes = _appointmentService.GetAvailableTimes();
            ViewBag.AvailableTimes = availableTimes;

            // Eğer bir departman seçilmişse o departmanın doktorlarını getir
            if (departmentId.HasValue && departmentId > 0)
            {
                doctors = await _doctorService.GetDoctorsByDepartmentIdAsync(departmentId.Value);
            }

            // ViewModel'i doldur
            var viewModel = new AppointmentViewModel
            {
                Departments = departments,
                
                Department = departmentId.HasValue
            ? departments.FirstOrDefault(d => d.Id == departmentId.Value)
            : null,
                Doctors = departmentId.HasValue
            ? await _doctorService.GetDoctorsByDepartmentIdAsync(departmentId.Value)
            : null

            };

            return View(viewModel);
        }

        [HttpPost]
        public async Task<IActionResult> Create(AppointmentViewModel model)
        {
            


            model.Departments = await _departmentService.GetAllAsync();

            // Eğer departman seçildiyse doktorları getir
            if (model.Department?.Id > 0)
            {
                model.Doctors = await _doctorService.GetDoctorsByDepartmentIdAsync(model.Department.Id);
            }
            else
            {
                // Hiçbir departman seçilmemişse doktor listesi boş bırak
                model.Doctors = new List<Doctor>();
            }
            var isTimeAvailable = await _appointmentService.IsTimeAvailableAsync(model.Doctor.Id, model.Date, model.Time);
            if (!isTimeAvailable)
            {
                TempData["ErrorMessage"] = "Bu saat için randevu mevcut. Lütfen başka bir saat seçin.";
                return View(model); // Kullanıcıya tekrar formu göster
            }
            // Randevu kaydını işle

            var appointment = new Appointment
                {
                    Date = model.Date,
                    Customer = model.Customer, // Müşteri bilgileri
                    DoctorId = model.Doctor.Id,
                    DepartmentId = model.Department.Id,
                    Time=model.Time,
                    Status = model.Status,
                     
                };

                await _appointmentService.AddAppointmentAsync(appointment);
                return RedirectToAction("Index", "Home");  // Başarılı işlem sonrası yönlendirme
            
            
            
        }


        [HttpGet]
        public async Task<IActionResult> GetDoctorsByDepartment(int departmentId)
        {
            if (departmentId <= 0)
            {
                return BadRequest("Invalid Department ID");
            }

            var doctors = await _doctorService.GetDoctorsByDepartmentIdAsync(departmentId);
            if (doctors == null || !doctors.Any())
            {
                Console.WriteLine("No doctors found for the given department ID.");
            }
            else
            {
                Console.WriteLine($"Found {doctors.Count()} doctors.");
            }
            return Json(doctors.Select(d => new { id = d.Id, name = d.FullName }));
        }
        [HttpGet]
        public IActionResult CancelAppointment()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> CancelAppointment(string tcNo)
        {
            //if (string.IsNullOrEmpty(tcNo))
            //{
            //    TempData["ErrorMessage"] = "Lütfen T.C. kimlik numaranızı girin.";
            //    return View();
            //}

            var appointment = await _appointmentService.GetAppointmentssByTcNoAsync(tcNo);

            if (appointment == null)
            {
                TempData["ErrorMessage"] = "Bu T.C. kimlik numarasına ait aktif bir randevu bulunamadı.";
                return View();
            }

            var viewModel = new AppointmentViewModel
            {
                Id = appointment.Id,
                Date = appointment.Date,
                CustomerName = appointment.Customer.FullName,
                DoctorName = appointment.Doctor.FullName,
                DepartmentName = appointment.Department.Name,
                Status = appointment.Status,
            };

            return View("ConfirmCancel", viewModel); // Onaylama ekranı
        }

        [HttpPost]
        public async Task<IActionResult> ConfirmCancel(int id)
        {
            var result = await _appointmentService.CancelAppointmentAsync(id);

            if (result)
            {
                TempData["SuccessMessage"] = "Randevunuz başarıyla iptal edildi.";
                return RedirectToAction("CancelAppointment");
            }

            TempData["ErrorMessage"] = "Randevuyu iptal ederken bir hata oluştu.";
            return RedirectToAction("CancelAppointment");
        }
        [HttpGet]
        public IActionResult BookAppointment()
        {
            var availableTimes = _appointmentService.GetAvailableTimes();
            ViewBag.AvailableTimes = availableTimes;

            return View();
        }
        [HttpPost]
        public async Task<IActionResult> BookAppointment(AppointmentViewModel model)
        {
            var allowedTimes = _appointmentService.GetAvailableTimes();
            if (!allowedTimes.Contains(model.Time))
            {
                TempData["ErrorMessage"] = "Geçersiz bir saat seçtiniz. Lütfen izin verilen saatler arasından seçim yapın.";
                return RedirectToAction("BookAppointment");
            }

            // Randevu oluşturma işlemleri
            var isTimeAvailable = await _appointmentService.IsTimeAvailableAsync(model.Doctor.Id, model.Date, model.Time);
            if (!isTimeAvailable)
            {
                TempData["ErrorMessage"] = "Bu saat için randevu mevcut. Lütfen başka bir saat seçin.";
                return RedirectToAction("BookAppointment");
            }

            var appointment = new Appointment
            {
                Date = model.Date,
                Customer = model.Customer, // Müşteri bilgileri
                DoctorId = model.Doctor.Id,
                DepartmentId = model.Department.Id,
                Time = model.Time,
                Status = model.Status,

            };
            // Randevuyu kaydetme işlemleri
            await _appointmentService.AddAppointmentAsync(appointment);
            TempData["SuccessMessage"] = "Randevunuz başarıyla oluşturuldu.";
            return RedirectToAction("Index");
        }
        public IActionResult Success()
        {
            return View();
        }

        public async Task<IActionResult> Index()
        {
            return View();
        }
    }
}
