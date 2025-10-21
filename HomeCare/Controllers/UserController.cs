using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using HomeCare.Models;
using HomeCare.Data;
using HomeCare.ViewModels;

namespace HomeCare.Controllers
{
    public class UserController : Controller
    {
        private readonly AppDbContext _context;

        public UserController(AppDbContext context)
        {
            _context = context;
        }

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

        // GET: Booking page with available dates and categories
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

        // POST: Handle booking submission
        [HttpPost]
        public IActionResult Booking(BookingViewModel model)
        {
            var selectedCategory = _context.Categories.FirstOrDefault(c => c.Id == model.CategoryId);

            // Require notes if category is "OTHER"
            if (selectedCategory?.Name.ToUpper() == "OTHER" && string.IsNullOrWhiteSpace(model.Notes))
            {
                ModelState.AddModelError("Notes", "Please provide details for 'Other' category.");
            }

            // Retrieve selected time slot and ensure it's not already booked
            var selectedSlot = _context.TimeSlots
                .Include(ts => ts.AvailableDate)
                .FirstOrDefault(ts => ts.Id == model.TimeSlotId && !ts.IsBooked);

            if (selectedSlot == null)
            {
                ModelState.AddModelError("TimeSlotId", "Selected time slot is no longer available.");
            }

            // Attempt to parse start time from slot string
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

            // Save or update appointment if all validations pass
            if (ModelState.IsValid && validTime)
            {
                if (model.AppointmentId > 0)
                {
                    // Editing existing appointment
                    var existing = _context.Appointments.FirstOrDefault(a => a.Id == model.AppointmentId);
                    if (existing != null)
                    {
                        // Unbook previous time slot
                        var oldSlot = _context.TimeSlots.FirstOrDefault(ts => ts.Id == existing.TimeSlotId);
                        if (oldSlot != null) oldSlot.IsBooked = false;

                        // Update appointment details
                        existing.DateTime = selectedSlot.AvailableDate.Date.Add(startTime);
                        existing.TimeSlotId = selectedSlot.Id;
                        existing.CategoryId = selectedCategory!.Id;
                        existing.Notes = model.Notes;

                        selectedSlot.IsBooked = true;

                        _context.SaveChanges();

                        TempData["BookingSuccess"] = $"Appointment has changed to {existing.DateTime:yyyy-MM-dd} {selectedSlot.Slot}!";
                        return RedirectToAction("Booking");
                    }
                }
                else
                {
                    // Creating new appointment
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

            // Reload view model if validation fails
            model.AvailableDates = _context.AvailableDates
                .Include(d => d.TimeSlots)
                .Where(d => d.Date >= DateTime.Today && d.TimeSlots.Any(ts => !ts.IsBooked))
                .OrderBy(d => d.Date)
                .ToList();

            model.Categories = _context.Categories
                .OrderBy(c => c.Name)
                .ToList();

            ViewBag.Appointments = _context.Appointments
                .Include(a => a.TimeSlot)
                .Include(a => a.Category)
                .Where(a => a.DateTime >= DateTime.Today)
                .OrderBy(a => a.DateTime)
                .ToList();

            return View(model);
        }

        // Cancel appointment
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
        // Edit appointment
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
                AppointmentId = appointment.Id // Add this to your ViewModel
            };

            // List of booked appointments
            ViewBag.Appointments = _context.Appointments
        .Include(a => a.TimeSlot)
        .Include(a => a.Category)
        .Where(a => a.DateTime >= DateTime.Today)
        .OrderBy(a => a.DateTime)
        .ToList();

            // For highlight editing appointment
            ViewBag.EditingId = appointment.Id;

            return View("Booking", model);
        }


        // Convert HH:mm string to hour as double (e.g. "14:30" => 14.0)
        private double TimeToHour(string time)
        {
            return double.Parse(time.Split(':')[0]);
        }
    }

}
