using HomeCare.Api.Enums;
using HomeCare.Api.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Security.Claims;
using System.Threading.Tasks;
using HomeCare.Api.Services.Interfaces;


namespace HomeCare.Api.Controllers
{
    // controller for caregiver-related actions
    [Authorize(Roles = $"{UserRoleExtensions.Roles.Caregiver},{UserRoleExtensions.Roles.Admin}")]
    [ApiController]
    [Route("api/[controller]")]
    public class CaregiverController : AuthorizedControllerBase
    {
        private readonly ICaregiverService _service;
        private readonly ILogger<CaregiverController> _logger;

        public CaregiverController(ICaregiverService service, ILogger<CaregiverController> logger)
        {
            _service = service;
            _logger = logger;
        }

        // dashboard data
        [HttpGet("dashboard")]
        public async Task<IActionResult> GetDashboard([FromQuery] int? year, [FromQuery] int? month)
        {
            _logger.LogInformation("Caregiver Dashboard requested. User claims: {Claims}",
                string.Join(", ", User.Claims.Select(c => $"{c.Type}={c.Value}")));

            try
            {
                var userId = GetCurrentUserId();
                _logger.LogInformation("Caregiver User ID: {UserId}", userId);

                int calendarYear = year ?? DateTime.Today.Year;
                int calendarMonth = month ?? DateTime.Today.Month;

                var model = await _service.GetDashboardAsync(userId, year, month);
                _logger.LogInformation("Dashboard model retrieved: {HasModel}, AvailableDates count: {Count}",
                    model != null, model?.AvailableDates?.Count ?? 0);

                return Ok(new
                {
                    calendarYear,
                    calendarMonth,
                    availableDates = model?.AvailableDates?.OrderBy(d => d.Date).ToList() ?? new List<DateTime>(),
                    caregiverId = userId,
                    model
                });
            }
            catch (UnauthorizedAccessException ex)
            {
                _logger.LogWarning("Unauthorized access: {Message}", ex.Message);
                return Unauthorized(new { message = "Ikke autentisert." });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error in GetDashboard: {Message}", ex.Message);
                return StatusCode(500, new { message = "En feil oppstod.", error = ex.Message });
            }
        }

        // register working day
        [HttpPost("availability")]
        public async Task<IActionResult> RegisterAvailability([FromBody] DateTime AvailableDate)
        {
            if (AvailableDate.Date < DateTime.Today)
            {
                return BadRequest(new { message = "Du kan ikke registrere en dato i fortiden." });
            }

            var userId = GetCurrentUserId();
            await _service.RegisterAvailabilityAsync(userId, AvailableDate);
            return NoContent();
        }
// register multiple working days
        [HttpPost("availability/batch")]
        public async Task<IActionResult> RegisterMultipleAvailability([FromBody] List<DateTime> SelectedDates)
        {
            var userId = GetCurrentUserId();
            await _service.RegisterMultipleAvailabilityAsync(userId, SelectedDates);
            return NoContent();
        }
// delete working day
        [HttpDelete("availability")]
        public async Task<IActionResult> DeleteAvailability([FromQuery] DateTime dateToDelete)
        {
            var userId = GetCurrentUserId();

            try
            {
                await _service.DeleteAvailabilityAsync(userId, dateToDelete.Date);
                return NoContent();
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }
// request deletion of working day
        [HttpPost("availability/request-deletion")]
        public async Task<IActionResult> RequestAvailabilityDeletion([FromQuery] int CaregiverId, [FromQuery] DateTime date)
        {
            var deleted = await _service.RequestAvailabilityDeletionAsync(CaregiverId, date.Date);

            if (deleted)
            {
                return NoContent();
            }
            else
            {
                return Conflict(new { message = "Denne dagen er allerede reservert av en kunde. Administrator vil håndtere forespørselen manuelt." });
            }
        }
    }
}
