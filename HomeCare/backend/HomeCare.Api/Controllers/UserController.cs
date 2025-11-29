using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using HomeCare.Models;
using HomeCare.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace HomeCare.Controllers
{
    // gives user dashboard info
    [ApiController]
    [Route("api/[controller]")]
    public class UserController : ControllerBase
    {
        private readonly AppDbContext _context;

        public UserController(AppDbContext context)
        {
            _context = context;
        }

        /// returns reminders, calendar info and upcoming appointments
        [HttpGet("dashboard")]
        public async Task<ActionResult<UserDashboardDto>> Dashboard(int? year, int? month)
        {
            // figuring out which month the user wants to see
            var targetDate = new DateTime(
                year ?? DateTime.Today.Year,
                month ?? DateTime.Today.Month,
                1
            );

            // fake reminders for now
            var reminders = GetTodayReminders();

            // loading upcoming appointments 
            var appointments = await _context.Appointments
                .Include(a => a.Category)
                .Where(a => a.DateTime >= DateTime.Today)
                .OrderBy(a => a.DateTime)
                .ToListAsync();

            var dto = new UserDashboardDto
            {
                CalendarYear = targetDate.Year,
                CalendarMonth = targetDate.Month,
                Reminders = reminders,
                Appointments = appointments
            };

            return Ok(dto);
        }

        // simple dummy reminders for dashboard
        private List<Reminder> GetTodayReminders()
        {
            // just hardcoded reminders to show how it works
            return new List<Reminder>
            {
                new Reminder { Time = "08:00", Message = "Medicine" },
                new Reminder { Time = "14:00", Message = "Visit by staff A" }
            };
        }
    }


    public class UserDashboardDto
    {
        public int CalendarYear { get; set; }
        public int CalendarMonth { get; set; }
        public List<Reminder> Reminders { get; set; } = new();
        public List<Appointment> Appointments { get; set; } = new();
    }
}