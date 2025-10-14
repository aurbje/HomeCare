using Microsoft.AspNetCore.Mvc;
using HomeCare.ViewModels.Account;
using HomeCare.ViewModels.User;
using HomeCare.Models;

namespace HomeCare.Controllers
{
    public class UserController : Controller
    {
        // === SIGN IN ===
        [HttpGet]
        public IActionResult SignIn() => View();

        [HttpPost]
        public IActionResult SignIn(SignInViewModel model)
        {
            if (ModelState.IsValid)
            {
                // TODO: autentisering
                return RedirectToAction("Welcome", new { name = "Kari Nordmann" });
            }
            return View(model);
        }

        // === SIGN UP ===
        [HttpGet]
        public IActionResult SignUp() => View();

        [HttpPost]
        public IActionResult SignUp(SignUpViewModel model)
        {
            if (ModelState.IsValid)
            {
                // TODO: lagre ny bruker
                return RedirectToAction("Welcome", new { name = model.FullName });
            }
            return View(model);
        }

        // === WELCOME ===
        [HttpGet]
        public IActionResult Welcome(string name = "Bruker")
        {
            ViewData["UserName"] = name;
            return View();
        }

        // === DASHBOARD ===
        [HttpGet]
        public IActionResult Dashboard()
        {
            var model = new DashboardViewModel
            {
                FullName = "Kari Nordmann",
                Email = "kari@example.com",
                UpcomingBookings = 2,
                CompletedBookings = 5,
                LastLogin = DateTime.Now.AddDays(-1),
                Notifications = new List<string>
                {
                    "Hjemmebesøk i morgen kl. 10:00 med Anne.",
                    "Din faktura for september er tilgjengelig."
                },
                Reminders = new List<Reminder>
                {
                    new Reminder { Time = "08:00", Message = "Ta medisiner" },
                    new Reminder { Time = "14:00", Message = "Besøk av hjemmesykepleier" }
                },
                Appointments = new List<Appointment>
                {
                    new Appointment { DateTime = DateTime.Today.AddDays(1), ServiceType = "Handlehjelp" },
                    new Appointment { DateTime = DateTime.Today.AddDays(3), ServiceType = "Renhold" }
                }
            };

            return View(model);
        }
    }
}
