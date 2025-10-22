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
        private readonly AppDbContext _context;
        private readonly IUserRepository _userRepo;
        private readonly ILogger<UserController> _logger;

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

        public IActionResult Dashboard(int? year, int? month)
        {
            var targetDate = new DateTime(
                year ?? DateTime.Today.Year,
                month ?? DateTime.Today.Month,
                1
            );

            var reminders = GetTodayReminders();

            ViewBag.CalendarYear = targetDate.Year;
            ViewBag.CalendarMonth = targetDate.Month;

            var appointments = _context.Appointments
                .Include(a => a.Category)
                .Where(a => a.DateTime >= DateTime.Today)
                .OrderBy(a => a.DateTime)
                .ToList();

            return View((reminders, appointments));
        }

        private List<Reminder> GetTodayReminders()
        {
            return new List<Reminder>
            {
                new Reminder { Time = "08:00", Message = "Medicine" },
                new Reminder { Time = "14:00", Message = "Visit by staff A" }
            };
        }

        // GET: Booking page
        public IActionResult Booking()
        {
            var availableDates = _context.AvailableDates
                .Include(d => d.TimeSlots)
                .Where(d => d.Date >= DateTime.Today && d.TimeSlots.Any(ts => !ts.IsBooked))
                .OrderBy(d => d.Date)
                .ToList();

            var categories = _context.Categories
                .OrderBy(c => c.Name)
                .ToList();

            var model = new BookingViewModel
            {
                SelectedDate = DateTime.Today,
                TimeSlotId = 0,
                CategoryId = categories.FirstOrDefault()?.Id ?? 0,
                AvailableDates = availableDates,
                Categories = categories
            };

            ViewBag.Appointments = _context.Appointments
                .Include(a => a.TimeSlot)
                .Include(a => a.Category)
                .Where(a => a.DateTime >= DateTime.Today)
                .OrderBy(a => a.DateTime)
                .ToList();

            return View(model);
        }

        // POST: Booking submission
        [HttpPost]
        public IActionResult Booking(BookingViewModel model)
        {
            var selectedCategory = _context.Categories.FirstOrDefault(c => c.Id == model.CategoryId);

            if (selectedCategory?.Name.ToUpper() == "OTHER" && string.IsNullOrWhiteSpace(model.Notes))
            {
                ModelState.AddModelError("Notes", "Please provide details for 'Other' category.");
            }

            var selectedSlot = _context.TimeSlots
                .Include(ts => ts.AvailableDate)
                .FirstOrDefault(ts => ts.Id == model.TimeSlotId && !ts.IsBooked);

            if (selectedSlot == null)
            {
                ModelState.AddModelError("TimeSlotId", "Selected time slot is no longer available.");
            }

            TimeSpan startTime = default;
            bool validTime = false;

            if (selectedSlot != null)
            {
                var slotParts = selectedSlot.Slot.Replace("–", "-").Split('-');
                if (slotParts.Length == 2 && TimeSpan.TryParse(slotParts[0], out startTime))
                {
                    validTime = true;
                }
                else
                {
                    ModelState.AddModelError("TimeSlotId", "Invalid time slot format.");
                }
            }

            if (ModelState.IsValid && validTime)
            {
                if (model.AppointmentId > 0)
                {
                    var existing = _context.Appointments.FirstOrDefault(a => a.Id == model.AppointmentId);
                    if (existing != null)
                    {
                        var oldSlot = _context.TimeSlots.FirstOrDefault(ts => ts.Id == existing.TimeSlotId);
                        if (oldSlot != null) oldSlot.IsBooked = false;

                        if (selectedSlot?.AvailableDate == null)
                        {
                            ModelState.AddModelError("TimeSlotId", "No available date found for this slot.");
                            return View(model);
                        }

                        existing.DateTime = selectedSlot.AvailableDate.Date.Add(startTime);
                        existing.TimeSlotId = selectedSlot.Id;
                        existing.CategoryId = selectedCategory!.Id;
                        existing.Notes = model.Notes;

                        selectedSlot.IsBooked = true;
                        _context.SaveChanges();

                        TempData["BookingSuccess"] = $"Appointment changed to {existing.DateTime:yyyy-MM-dd} {selectedSlot.Slot}!";
                        return RedirectToAction("Booking");
                    }
                }
                else
                {
                    if (selectedSlot?.AvailableDate == null)
                    {
                        ModelState.AddModelError("TimeSlotId", "No available date found for this slot.");
                        return View(model);
                    }

                    var appointment = new Appointment
                    {
                        DateTime = selectedSlot.AvailableDate.Date.Add(startTime),
                        TimeSlotId = selectedSlot.Id,
                        CategoryId = selectedCategory!.Id,
                        Notes = model.Notes
                    };

                    selectedSlot.IsBooked = true;
                    _context.Appointments.Add(appointment);
                    _context.SaveChanges();

                    TempData["BookingSuccess"] = $"Appointment booked for {model.SelectedDate:yyyy-MM-dd} {selectedSlot.Slot}!";
                    return RedirectToAction("Booking");
                }
            }

            model.AvailableDates = _context.AvailableDates
                .Include(d => d.TimeSlots)
                .Where(d => d.Date >= DateTime.Today && d.TimeSlots.Any(ts => !ts.IsBooked))
                .OrderBy(d => d.Date)
                .ToList();

            model.Categories = _context.Categories.OrderBy(c => c.Name).ToList();

            ViewBag.Appointments = _context.Appointments
                .Include(a => a.TimeSlot)
                .Include(a => a.Category)
                .Where(a => a.DateTime >= DateTime.Today)
                .OrderBy(a => a.DateTime)
                .ToList();

            return View(model);
        }

        public IActionResult CancelAppointment(int id)
        {
            var appointment = _context.Appointments.FirstOrDefault(a => a.Id == id);
            if (appointment != null)
            {
                var slot = _context.TimeSlots.FirstOrDefault(ts => ts.Id == appointment.TimeSlotId);
                if (slot != null) slot.IsBooked = false;

                _context.Appointments.Remove(appointment);
                _context.SaveChanges();
                TempData["BookingSuccess"] = "Appointment has been cancelled.";
            }
            return RedirectToAction("Booking");
        }

        public IActionResult EditAppointment(int id)
        {
            var appointment = _context.Appointments
                .Include(a => a.TimeSlot)
                .Include(a => a.Category)
                .FirstOrDefault(a => a.Id == id);

            if (appointment == null) return NotFound();

            var model = new BookingViewModel
            {
                SelectedDate = appointment.DateTime.Date,
                TimeSlotId = appointment.TimeSlotId,
                CategoryId = appointment.CategoryId,
                Notes = appointment.Notes,
                AvailableDates = _context.AvailableDates.Include(d => d.TimeSlots)
                    .Where(d => d.Date >= DateTime.Today && d.TimeSlots.Any(ts => !ts.IsBooked || ts.Id == appointment.TimeSlotId))
                    .OrderBy(d => d.Date).ToList(),
                Categories = _context.Categories.OrderBy(c => c.Name).ToList(),
                AppointmentId = appointment.Id
            };

            ViewBag.Appointments = _context.Appointments
                .Include(a => a.TimeSlot)
                .Include(a => a.Category)
                .Where(a => a.DateTime >= DateTime.Today)
                .OrderBy(a => a.DateTime)
                .ToList();

            ViewBag.EditingId = appointment.Id;

            return View("Booking", model);
        }
    }
}
