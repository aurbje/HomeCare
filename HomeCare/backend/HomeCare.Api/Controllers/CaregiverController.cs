using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using HomeCare.Api.Services;

namespace HomeCare.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize(Roles = "Caregiver")]
    public class CaregiverController : ControllerBase
    {
        private readonly BookingService _bookingService;
        private readonly ILogger<CaregiverController> _logger;

        public CaregiverController(BookingService bookingService, ILogger<CaregiverController> logger)
        {
            _bookingService = bookingService;
            _logger = logger;
        }

        // GET: /api/caregiver/clients
        [HttpGet("clients")]
        public async Task<IActionResult> GetAssignedClients()
        {
            try
            {
                var clients = await _bookingService.GetClientsForCaregiverAsync(User);
                if (clients == null || !clients.Any())
                {
                    _logger.LogInformation("No clients found for caregiver {User}.", User.Identity?.Name);
                    return NotFound(new { message = "No assigned clients found." });
                }

                return Ok(clients);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error while fetching assigned clients for caregiver {User}.", User.Identity?.Name);
                return StatusCode(500, new { message = "Unexpected error while fetching clients." });
            }
        }

        // GET: /api/caregiver/schedule
        [HttpGet("schedule")]
        public async Task<IActionResult> GetSchedule()
        {
            try
            {
                var schedule = await _bookingService.GetScheduleForCaregiverAsync(User);
                if (schedule == null || !schedule.Any())
                {
                    _logger.LogInformation("No schedule found for caregiver {User}.", User.Identity?.Name);
                    return NotFound(new { message = "No scheduled visits found." });
                }

                return Ok(schedule);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error while fetching schedule for caregiver {User}.", User.Identity?.Name);
                return StatusCode(500, new { message = "Unexpected error while fetching schedule." });
            }
        }

        // PUT: /api/caregiver/visit/{bookingId}/complete
        [HttpPut("visit/{bookingId}/complete")]
        public async Task<IActionResult> CompleteVisit(int bookingId)
        {
            try
            {
                var result = await _bookingService.CompleteVisitAsync(bookingId);
                if (!result.Success)
                {
                    return BadRequest(new { message = result.Message ?? "Unable to complete visit." });
                }

                _logger.LogInformation("Caregiver {User} completed visit {BookingId}.", User.Identity?.Name, bookingId);
                return Ok(new { message = "Visit marked as completed." });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error completing visit {BookingId} for caregiver {User}.", bookingId, User.Identity?.Name);
                return StatusCode(500, new { message = "Unexpected error while completing visit." });
            }
        }
    }
}
