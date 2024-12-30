using Entity.Entities;
using Entity.Services;
using Entity.ViewModels;
using Microsoft.AspNetCore.Mvc;
using Service.Services;

namespace HospitalAppointmentSystem.Areas.Admin.Controllers
{
    [Area("Admin")]
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
        public async Task<IActionResult> Index(string? tcNo)
        {

            List<Appointment> appointments;

            // Eğer bir T.C. kimlik numarası girilmişse filtreleme yap
            

            if (!string.IsNullOrEmpty(tcNo))
            {
                appointments = (await _appointmentService.GetAppointmentsByTcNoAsync(tcNo)).ToList();
            }
            else
            {
                appointments = (await _appointmentService.GetAllAppointmentsAsync()).ToList();
            }

            // ViewModel'e dönüştür
            var appointmentViewModels = appointments.Select(a => new AppointmentViewModel
            {
                Id = a.Id,
                Date = a.Date,
                Time=a.Time,
                CustomerName = a.Customer?.FullName ?? "N/A", // Müşteri Adı
                DoctorName = a.Doctor?.FullName ?? "N/A",    // Doktor Adı
                DepartmentName = a.Department?.Name ?? "N/A", // Departman Adı
                CustomerTc = a.Customer?.TcNo ?? "N/A" ,
                Status=a.Status// Müşteri T.C.
            }).ToList();

            ViewBag.TcNo = tcNo; // Arama kutusunda gösterim için
            return View(appointmentViewModels);
        }
        [HttpGet]
        public async Task<IActionResult> Edit(int id)
        {
            var appointment = await _appointmentService.GetAppointmentByIdAsync(id);
            if (appointment == null)
            {
                return NotFound("Randevu bulunamadı.");
            }

            // Gerekli verileri doldurun (Departman ve Doktor listesi)
            ViewBag.Departments = await _departmentService.GetAllAsync();
            ViewBag.Doctors = await _doctorService.GetDoctorsByDepartmentIdAsync(appointment.DepartmentId);

            return View(appointment); // Appointment entity'sini doğrudan View'e gönderiyoruz
        }

        [HttpPost]
        public async Task<IActionResult> Edit(Appointment appointment)
        {
           

            // Appointment güncelle
            var existingAppointment = await _appointmentService.GetAppointmentByIdAsync(appointment.Id);
            if (existingAppointment == null)
            {
                return NotFound("Randevu bulunamadı.");
            }

            // Güncellemeleri yap
            existingAppointment.Date = appointment.Date;
            existingAppointment.Customer = appointment.Customer;
            existingAppointment.DepartmentId = appointment.DepartmentId;
            existingAppointment.DoctorId = appointment.DoctorId;

            // Güncellenmiş randevuyu kaydet
            await _appointmentService.UpdateAppointmentAsync(existingAppointment);

            return RedirectToAction("Index", "Appointment"); 
        }
        [HttpPost]
        public async Task<IActionResult> Delete(int id)
        {
            try
            {
                await _appointmentService.DeleteAppointmentAsync(id);
                TempData["SuccessMessage"] = "Randevu başarıyla silindi.";
            }
            catch (KeyNotFoundException ex)
            {
                TempData["ErrorMessage"] = ex.Message;
            }
            catch (Exception)
            {
                TempData["ErrorMessage"] = "Randevu silinirken bir hata oluştu.";
            }

            return RedirectToAction(nameof(Index));
        }

    }
}
