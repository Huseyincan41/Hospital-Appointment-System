using Entity.Services;
using Microsoft.AspNetCore.Mvc;

namespace HospitalAppointmentSystem.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class CustomerController : Controller
    {
        private readonly ICustomerService _customerService;

        public CustomerController(ICustomerService customerService)
        {
            _customerService = customerService;
        }

        [HttpGet]
        public async Task<IActionResult> Index()
        {
            var customers = await _customerService.GetAllCustomersAsync();
            return View(customers);
        }
        [HttpPost]
        public async Task<IActionResult> Delete(int id)
        {
            var result = await _customerService.DeleteCustomerAsync(id);

            if (!result)
            {
                TempData["Error"] = "Müşteri silinemedi. Müşteri bulunamadı.";
                return RedirectToAction("Index");
            }

            TempData["Success"] = "Müşteri başarıyla silindi.";
            return RedirectToAction("Index");
        }
    }
}
