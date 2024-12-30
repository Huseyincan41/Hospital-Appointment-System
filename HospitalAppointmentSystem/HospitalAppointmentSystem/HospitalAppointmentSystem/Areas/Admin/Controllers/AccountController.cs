﻿using Entity.Services;
using Entity.ViewModels;
using Microsoft.AspNetCore.Mvc;

namespace HospitalAppointmentSystem.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class AccountController : Controller
    {

        private readonly IAccountService _service;

        public AccountController(IAccountService service)
        {
            _service = service;
        }

        public IActionResult Index()
        {
            return View();
        }
        public IActionResult Login(string? ReturnUrl)
        {
            LoginViewModel model = new LoginViewModel()
            {
                ReturnUrl = ReturnUrl
            };

            return View(model);
        }
        [HttpPost]
        public async Task<IActionResult> Login(LoginViewModel model)
        {
            string msg = await _service.FindByNameAsync(model);
            if (msg == "Kullanıcı bulunamadı!")
            {
                ModelState.AddModelError("", msg);
                return RedirectToAction("Index", "Home", new { area = "Admin" });
            }
            else if (msg == "OK")
            {
                return Redirect(model.ReturnUrl ?? "~/");

            }
            else
            {
                ModelState.AddModelError("", "Kullanıcı adı veya şifre hatalı");
            }
            return RedirectToAction("Index", "Home", new { area = "Admin" });
        }
        public IActionResult Register()
        {
            return View();
        }
        [HttpPost]
        public async Task<IActionResult> Register(RegisterViewModel model)
        {
            string msg = await _service.CreateUserAsync(model);
            if (msg == "OK")
            {
                return RedirectToAction("Login");
            }
            else
            {
                ModelState.AddModelError("", msg);
            }
            return View(model);
        }
    }
}
