using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using HomeCare.Models;
using HomeCare.Data;

namespace HomeCare.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class UserController : ControllerBase
    {
        private readonly AppDbContext _context;
        private readonly ILogger<UserController> _logger;

        public UserController(AppDbContext context, ILogger<UserController> logger)
        {
            _context = context;
            _logger = logger;
        }

        // GET: /api/user/dashboard?year=2025&month=11
        [HttpGet("dashboard")]
        public async Task<ActionResult<UserDashboardDto>> Dashboard(int? year, int? month)
        {
            try
            {
                var targetDate = new DateTime(
                    year ?? DateTime.Today.Year,
                    month ?? DateTime.Today.Month,
                    1
                );

                _logger.LogInformation("Loading dashboard data for {Year}-{Month}", targetDate.Year, targetDate.Month);

                // Fake reminders for now (can later come from DB)
                var reminders = GetTodayReminders();

                // Retrieve upcoming appointments (can later filter by logged-in user)
                var appointments = await _context.Appointments
                    .Include(a => a.Category)
                    .Where(a => a.DateTime >= DateTime.Today)
                    .OrderBy(a => a.DateTime)
                    .ToListAsync();

                if (!appointments.Any())
                {
                    _logger.LogInformation("No upcoming appointments found for dashboard view.");
                }

                var dto = new UserDashboardDto
                {
                    CalendarYear = targetDate.Year,
                    CalendarMonth = targetDate.Month,
                    Reminders = reminders,
                    Appointments = appointments
                };

                return Ok(dto);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error loading user dashboard data.");
                return StatusCode(500, new { message = "Unexpected error while loading dashboard data." });
            }
        }

        // Temporary dummy reminders
        private List<Reminder> GetTodayReminders()
        {
            return new List<Reminder>
            {
                new Reminder { Time = "08:00", Message = "Take medication" },
                new Reminder { Time = "14:00", Message = "Scheduled visit by caregiver" }
            };
        }
    }

    // DTO for React frontend
    public class UserDashboardDto
    {
        public int CalendarYear { get; set; }
        public int CalendarMonth { get; set; }
        public List<Reminder> Reminders { get; set; } = new();
        public List<Appointment> Appointments { get; set; } = new();
    }
}
