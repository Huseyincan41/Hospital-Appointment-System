using Entity.Services;
using Microsoft.AspNetCore.Mvc;

namespace HospitalAppointmentSystem.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class HomeController : Controller
    {
        private readonly IAccountService _accountService;

        public HomeController(IAccountService accountService)
        {
            _accountService = accountService;
        }

        public IActionResult Index()
        {
            return View();
        }
        public async Task<IActionResult> Users()
        {
            var users = await _accountService.GetAllUsers();
            return View(users);
        }
    }
}
