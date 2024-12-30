using Entity.Entities;
using Entity.Services;
using Entity.UnitOfWorks;
using Entity.ViewModels;
using Microsoft.AspNetCore.Mvc;

namespace HospitalAppointmentSystem.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class DoctorController : Controller
    {
        private readonly IDoctorService _doctorService;
        private readonly IDepartmentService _departmentService;
        private readonly IUnitOfWork _unitOfWork;

        public DoctorController(IDoctorService doctorService, IDepartmentService departmentService, IUnitOfWork unitOfWork)
        {
            _doctorService = doctorService;
            _departmentService = departmentService;
            _unitOfWork = unitOfWork;
        }

        [HttpGet]
        public async Task<IActionResult> Create()
        {
            ViewBag.Departments = await _departmentService.GetAllAsync();
            return View(new DoctorViewModel());
        }

        [HttpPost]
        public async Task<IActionResult> Create(DoctorViewModel model, IFormFile Image)
        {
            
                string imageUrl = null;

                if (Image != null && Image.Length > 0)
                {
                    // Dosyayı kaydetmek için bir yol oluşturun
                    var uploadsFolder = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/images/doctors");
                    Directory.CreateDirectory(uploadsFolder); // Klasör yoksa oluştur
                    var uniqueFileName = Guid.NewGuid().ToString() + "_" + Image.FileName;
                    var filePath = Path.Combine(uploadsFolder, uniqueFileName);

                    using (var fileStream = new FileStream(filePath, FileMode.Create))
                    {
                        await Image.CopyToAsync(fileStream);
                    }

                    // Kaydedilen dosyanın URL'sini oluştur
                    imageUrl = $"/images/doctors/{uniqueFileName}";
                }

                // ViewModel'deki ImageUrl alanına resim yolunu ata
                model.ImageUrl = imageUrl;

                // Doktor ekleme işlemi
                await _doctorService.AddDoctorAsync(model);
                return RedirectToAction("Index");
            
        }
        [HttpGet]
        public async Task<IActionResult> Index()
        {
            var doctors = await _doctorService.GetAllAsync();
            return View(doctors);
        }
        public async Task<bool> DeleteDoctorWithAppointmentsAsync(int doctorId)
        {
            var doctor = await _unitOfWork.GetRepository<Doctor>().GetByIdAsync(doctorId);

            if (doctor == null)
                return false;

            _unitOfWork.GetRepository<Doctor>().Delete(doctor);
            await _unitOfWork.CommitAsync();
            return true;
        }
        [HttpPost]
        public async Task<IActionResult> Delete(int id)
        {
            // Doktor silme işlemi
            var result = await _doctorService.DeleteDoctorWithAppointmentsAsync(id);

            if (!result)
            {
                TempData["ErrorMessage"] = "Doktor silinemedi. İlgili kayıt bulunamadı veya başka kayıtlara bağlı.";
                return RedirectToAction("Index");
            }

            TempData["SuccessMessage"] = "Doktor başarıyla silindi.";
            return RedirectToAction("Index");
        }
    }

}