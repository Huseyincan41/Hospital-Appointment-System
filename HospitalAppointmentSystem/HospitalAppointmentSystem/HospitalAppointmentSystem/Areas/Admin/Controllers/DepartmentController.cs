using Entity.Entities;
using Entity.Services;
using Entity.ViewModels;
using Microsoft.AspNetCore.Mvc;

namespace HospitalAppointmentSystem.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class DepartmentController : Controller
    {
        private readonly IDepartmentService _departmentService;

        public DepartmentController(IDepartmentService departmentService)
        {
            _departmentService = departmentService;
        }

        [HttpGet]
        public IActionResult Create()
        {
            return View(new DepartmentViewModel());
        }

        [HttpPost]
        public async Task<IActionResult> Create(DepartmentViewModel model, IFormFile Image)
        {
            string imageUrl = null;

            if (Image != null && Image.Length > 0)
            {
                // Dosyayı kaydetmek için bir yol oluşturun
                var uploadsFolder = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/images/departments");
                Directory.CreateDirectory(uploadsFolder); // Klasör yoksa oluştur
                var uniqueFileName = Guid.NewGuid().ToString() + "_" + Image.FileName;
                var filePath = Path.Combine(uploadsFolder, uniqueFileName);

                using (var fileStream = new FileStream(filePath, FileMode.Create))
                {
                    await Image.CopyToAsync(fileStream);
                }

                // Kaydedilen dosyanın URL'sini oluştur
                imageUrl = $"/images/departments/{uniqueFileName}";
            }

            // ViewModel'deki ImageUrl alanına resim yolunu ata
            model.ImageUrl = imageUrl;
            if (ModelState.IsValid)
            {
                await _departmentService.AddDepartmentAsync(model);
                TempData["Success"] = "Departman başarıyla eklendi.";
                return RedirectToAction("Index");
            }
            return View(model);
        }

        [HttpGet]
        public async Task<IActionResult> Index()
        {
            var departments = await _departmentService.GetAllDepartmentsAsync();
            return View(departments);
        }
        [HttpPost]
        public async Task<IActionResult> Delete(int id)
        {
            var result = await _departmentService.DeleteDepartmentAsync(id);
            if (result)
            {
                TempData["Success"] = "Departman başarıyla silindi.";
            }
            else
            {
                TempData["Error"] = "Departman silinemedi.";
            }

            return RedirectToAction("Index");
        }
    }
}
