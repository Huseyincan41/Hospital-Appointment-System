using Entity.Services;
using Entity.ViewModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.CodeAnalysis.Scripting;

using Service.Services;

namespace HospitalAppointmentSystem.Controllers
{
    public class DoctorController : Controller
    {
        private readonly IDoctorService _doctorService;
        private readonly IAppointmentService appointmentService;

        public DoctorController(IDoctorService doctorService, IAppointmentService appointmentService)
        {
            _doctorService = doctorService;
            this.appointmentService = appointmentService;
        }
        public async Task<IActionResult> DoctorAppointments()
        {
            int? doctorId = HttpContext.Session.GetInt32("doctorId");

            if (!doctorId.HasValue)
            {
                TempData["ErrorMessage"] = "Lütfen önce giriş yapın.";
                return RedirectToAction("DoctorLogin");
            }

            // Randevuları al
            var appointments = await appointmentService.GetAppointmentsByDoctorIdAsync(doctorId.Value);

            // Randevuları ViewModel'e dönüştür
            var appointmentViewModels = appointments.Select(a => new AppointmentViewModel
            {
                Id = a.Id,
                Date = a.Date,
                Time = a.Time,
                CustomerName = a.Customer?.FullName ?? "N/A", // Hasta adı
                DepartmentName = a.Department?.Name ?? "N/A",
                Status=a.Status
            }).ToList();

            return View(appointmentViewModels);
        }
        public IActionResult Index()
        {
            return View();
        }
        [HttpGet]
        public IActionResult DoctorLogin()
        {
            return View();
        }
        [HttpPost]
        public async Task<IActionResult> DoctorLogin(DoctorLoginViewModel model)
        {
            if (ModelState.IsValid)
            {
                var doctor = await _doctorService.GetDoctorByEmailAsync(model.Email);

                if (doctor != null /*&& BCrypt.Net.BCrypt.Verify(model.Password, doctor.Password)*/)
                {
                    // Giriş başarılı, oturum bilgisi saklanabilir
                    HttpContext.Session.SetInt32("DoctorId", doctor.Id);

                    return RedirectToAction("DoctorDashboard");
                }

                TempData["ErrorMessage"] = "E-posta veya şifre yanlış.";
            }

            return View(model);
        }
        [HttpGet]
        public async Task<IActionResult> DoctorDashboard(string searchTc)
        {

            // Oturumdan giriş yapan doktorun ID'sini al
            var doctorId = HttpContext.Session.GetInt32("DoctorId");

            if (doctorId == null)
            {
                return RedirectToAction("DoctorLogin");
            }

            // Doktora ait randevuları getir
            var appointments = await appointmentService.GetAppointmentsByDoctorIdAsync(doctorId.Value);
            if (!string.IsNullOrEmpty(searchTc))
            {
                appointments = appointments.Where(a => a.Customer != null && a.Customer.TcNo.Contains(searchTc)).ToList();
            }
            // Randevuları ViewModel'e dönüştür
            var appointmentViewModels = appointments
       .OrderBy(a => a.Date) // Önce tarih sırasına göre sırala
       .ThenBy(a => a.Time) // Aynı tarihteki randevuları saat sırasına göre sırala
       .Select(a => new AppointmentViewModel
       {
           Id = a.Id,
           Date = a.Date,
           Time = a.Time,
           CustomerName = a.Customer?.FullName ?? "N/A",
           DepartmentName = a.Department?.Name ?? "N/A",
           CustomerTc = a.Customer?.TcNo ?? "N/A",
           Status = a.Status,
       })
       .ToList();

            return View(appointmentViewModels);
        }
        [HttpPost]
        public async Task<IActionResult> UpdateAppointmentStatus(int appointmentId)
        {
            await appointmentService.UpdateAppointmentStatusAsync(appointmentId, "Onaylandı");

            TempData["SuccessMessage"] = "Randevu onaylandı.";
            return RedirectToAction("DoctorDashboard");
        }
        [HttpPost]
        public async Task<IActionResult> UpdateAppointment(int appointmentId, string status)
        {
            if (string.IsNullOrEmpty(status))
            {
                TempData["ErrorMessage"] = "Geçerli bir durum bilgisi gönderilmedi.";
                return RedirectToAction("DoctorDashboard");
            }

            var appointment = await appointmentService.GetAppointmentByIdAsync(appointmentId);
            if (appointment == null)
            {
                TempData["ErrorMessage"] = "Randevu bulunamadı.";
                return RedirectToAction("DoctorDashboard");
            }

            appointment.Status = status; // Status alanını güncelle

            await appointmentService.UpdateAppointmentAsync(appointment);

            TempData["SuccessMessage"] = $"Randevu durumu '{status}' olarak güncellendi.";
            return RedirectToAction("DoctorDashboard");
        }

    }
}
