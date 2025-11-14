using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using HomeCare.Models;
using HomeCare.Data;
using HomeCare.ViewModels;
using Microsoft.AspNetCore.Authorization;


//[Authorize(Roles = UserRoles.User)]
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
    }
}