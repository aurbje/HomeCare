using Microsoft.AspNetCore.Mvc;
using HomeCare.Models;


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

        private List<Reminder> GetTodayReminders()
        {
            return new List<Reminder> {
                new Reminder { Time = "08:00", Message = "Medicine" },
                new Reminder { Time = "14:00", Message = "Visit by staff A" }
            };
        }

        private List<Appointment> GetUpcomingAppointments()
        {
            return new List<Appointment> {
                new Appointment { DateTime = DateTime.Today.AddDays(1), ServiceType = "Shopping support" },
                new Appointment { DateTime = DateTime.Today.AddDays(3), ServiceType = "Daily chore" }
            };
        }
    }
}
