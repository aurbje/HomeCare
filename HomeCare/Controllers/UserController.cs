using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using HomeCare.ViewModels.Account;  // SignInViewModel, SignUpViewModel
using HomeCare.ViewModels;          // BookingViewModel
using HomeCare.Models;
using HomeCare.Data;
using HomeCare.Repositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace HomeCare.Controllers
{
    public class UserController : Controller
    {
        private readonly AppDbContext _context; // OK
        private readonly IUserRepository _userRepo;
        private readonly ILogger<UserController> _logger;

        // public UserController(AppDbContext context)
        // {
        //     _context = context;
        // }

        public UserController(AppDbContext context, IUserRepository userRepo, ILogger<UserController> logger)
        {
            _context = context;
            _userRepo = userRepo;
            _logger = logger;
        }

        // ======================= BRUKERINNLOGGING OG REGISTRERING =======================

        [HttpGet]
        public IActionResult SignIn() => View();

        [HttpPost]
        public async Task<IActionResult> SignIn(SignInViewModel model)
        {
            if (!ModelState.IsValid)
            {
                _logger.LogWarning("Login attempt failed validation for email {Email}", model.Email);
                return View(model);
            }

            try
            {
                var user = await _userRepo.GetByEmailAsync(model.Email);

                if (user == null || !BCrypt.Net.BCrypt.Verify(model.Password, user.PasswordHash))
                {
                    _logger.LogWarning("Invalid login attempt for email {Email}", model.Email);
                    ModelState.AddModelError("", "Ugyldig e-post eller passord");
                    return View(model);
                }

                _logger.LogInformation("User {Email} logged in successfully", model.Email);
                return RedirectToAction("Dashboard");
            }
            catch (Exception e)
            {
                _logger.LogError(e, "Unexpected error during login for email {Email}", model.Email);
                return View("Error");
            }
        }

        [HttpGet]
        public IActionResult SignUp() => View();

        [HttpPost]
        public async Task<IActionResult> SignUp(SignUpViewModel model)
        {
            if (!ModelState.IsValid)
            {
                var errors = ModelState.Values.SelectMany(v => v.Errors).Select(e => e.ErrorMessage);
                _logger.LogWarning("Signup validation failed for email {Email}. Errors: {Errors}", model.Email, errors);
                return View(model);
            }

            try
            {
                if (await _userRepo.EmailExistsAsync(model.Email))
                {
                    _logger.LogInformation("Signup attempt with already registered email {Email}", model.Email);
                    ModelState.AddModelError("Email", "E-postadressen er allerede registrert");
                    return View(model);
                }

                var user = new User
                {
                    FullName = model.FullName,
                    Email = model.Email,
                    PasswordHash = HashPassword(model.Password),
                    Role = "user",
                    TlfNumber = model.TlfNumber,
                    Address = model.Address
                };

                await _userRepo.AddAsync(user);
                await _userRepo.SaveChangesAsync();

                _logger.LogInformation("New user registered with email {Email}", model.Email);

                return RedirectToAction("SignIn");
            }
            catch (Exception e)
            {
                _logger.LogError(e, "Error occurred while registering new user {Email}", model.Email);
                return View("Error");
            }
        }

        private string HashPassword(string password) => BCrypt.Net.BCrypt.HashPassword(password);


        // ======================= DASHBOARD OG BOOKING =======================


        // Display dashboard with today's reminders and upcoming appointments
        public IActionResult Dashboard(int? year, int? month) // calandar
        {
            // after login
            // return View(); // Login function in AccountController.cs

            // Show username on dashboard
            // string username = User.Identity.Name; // get username
            // ViewBag.Username = username;
            // return View();

            // calander
            var targetDate = new DateTime(
            year ?? DateTime.Today.Year,
            month ?? DateTime.Today.Month,
            1
    );

            // reminder
            var reminders = GetTodayReminders();

            // calander
            ViewBag.CalendarYear = targetDate.Year;
            ViewBag.CalendarMonth = targetDate.Month;

            // booked appointments
            var appointments = _context.Appointments
                .Include(a => a.Category)
                .Where(a => a.DateTime >= DateTime.Today)
                .OrderBy(a => a.DateTime)
                .ToList();
            return View((reminders, appointments));
        }

        // Generate dummy reminders for dashboard
        private List<Reminder> GetTodayReminders()
        {
            return new List<Reminder>
            {
                new Reminder { Time = "08:00", Message = "Medicine" },
                new Reminder { Time = "14:00", Message = "Visit by staff A" }
            };
        }
    }
}