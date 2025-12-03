/* ============================================================
 * ORIGINAL GROUP CODE - Commented out
 * Replaced with merged version in UserController.cs
 * This file kept for reference purposes
 * ============================================================

using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using HomeCare.Api.Models;
using HomeCare.Api.Data;

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

                // Temporary hardcoded reminders (can later come from DB)
                var reminders = GetTodayReminders();

                // Fetch upcoming bookings instead of appointments
                var bookings = await _context.Bookings
                    .Include(b => b.Category)
                    .Where(b => b.Date >= DateTime.Today)
                    .OrderBy(b => b.Date)
                    .ToListAsync();

                if (!bookings.Any())
                {
                    _logger.LogInformation("No upcoming bookings found for dashboard view.");
                }

                var dto = new UserDashboardDto
                {
                    CalendarYear = targetDate.Year,
                    CalendarMonth = targetDate.Month,
                    Reminders = reminders,
                    Bookings = bookings
                };

                return Ok(dto);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error loading user dashboard data.");
                return StatusCode(500, new { message = "Unexpected error while loading dashboard data." });
            }
        }

        // Temporary dummy reminders for dashboard
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
        public List<Booking> Bookings { get; set; } = new();
    }
}

*/
