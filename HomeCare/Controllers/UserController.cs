using Microsoft.AspNetCore.Mvc;
using HomeCare.Models;
using HomeCare.ViewModels;


namespace HomeCare.Controllers
{
    public class UserController : Controller
    {
        public IActionResult Dashboard()
        {
            var reminders = GetTodayReminders();
            var appointments = GetUpcomingAppointments();
            return View((reminders, appointments));
        }

        // Dummy Reminders in Dashboard
        private List<Reminder> GetTodayReminders()
        {
            return new List<Reminder> {
                new Reminder { Time = "08:00", Message = "Medicine" },
                new Reminder { Time = "14:00", Message = "Visit by staff A" }
            };
        }

        // Dummy UpComingAppointments in Dashboard
        private List<Appointment> GetUpcomingAppointments()
        {
            return new List<Appointment> {
                new Appointment { DateTime = DateTime.Today.AddDays(1), ServiceType = "Shopping support" },
                new Appointment { DateTime = DateTime.Today.AddDays(3), ServiceType = "Daily chore" }
            };
        }

        // Booking
        public IActionResult Booking()
        {
            var model = new BookingViewModel
            {
                SelectedDate = DateTime.Today,
                Time = "08:00",
                Category = "Shopping"
            };
            ViewBag.AvailableDates = new List<string>{
                "2025-10-15",
                "2025-10-17",
                "2025-10-20"};
            return View(model);
        }

        [HttpPost]
        public IActionResult Booking(BookingViewModel model)
        {
            if (ModelState.IsValid)
            {
                TempData["BookingSuccess"] = $"Appointment booked for {model.SelectedDate} {model.Time}";
                return RedirectToAction("Dashboard");
            }
            ViewBag.AvailableDates = new List<string>
    {
        "2025-10-15",
        "2025-10-17",
        "2025-10-20"
    };

            return View(model);
        }
    }
}
