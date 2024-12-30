using Entity.Services;
using HospitalAppointmentSystem.Models;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;

namespace HospitalAppointmentSystem.Controllers
{
    public class HomeController : Controller
    {
        private readonly IDoctorService _doctorService;
        private readonly IDepartmentService _departmenService;
        private readonly ILogger<HomeController> _logger;

        public HomeController(ILogger<HomeController> logger, IDoctorService doctorService, IDepartmentService departmenService)
        {
            _logger = logger;
            _doctorService = doctorService;
            _departmenService = departmenService;
        }

        public async Task<IActionResult> Index()
        {
            var doctors = await _doctorService.GetAllAsync();
            return View(doctors);
        }
        public async Task<IActionResult> Doctors()
        {
            var doctors = await _doctorService.GetAllAsync();
            return View(doctors);
        }
        public async Task<IActionResult> Departments()
        {
            var departments = await _departmenService.GetAllDepartmentsAsync();
            return View(departments);
        }
        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
