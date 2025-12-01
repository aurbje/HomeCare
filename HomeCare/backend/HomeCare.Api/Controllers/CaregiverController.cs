using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using HomeCare.Api.Services;
using Microsoft.Extensions.Logging;

namespace HomeCare.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize(Roles = "Caregiver")]
    public class CaregiverController : ControllerBase
    {
        private readonly CaregiverService _caregiverService;
        private readonly ILogger<CaregiverController> _logger;

        public CaregiverController(CaregiverService caregiverService, ILogger<CaregiverController> logger)
        {
            _caregiverService = caregiverService;
            _logger = logger;
        }

        // GET: /api/caregiver/clients
        [HttpGet("clients")]
        public async Task<IActionResult> GetAssignedClients()
        {
            try
            {
                var response = await _caregiverService.GetClientsAsync(User);

                if (response == null || response.Data == null || !response.Data.Any())
                {
                    _logger.LogInformation("No clients found for caregiver {User}.", User.Identity?.Name);
                    return NotFound(new { message = "No assigned clients found." });
                }

                return Ok(response.Data);
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
                var response = await _caregiverService.GetScheduleAsync(User);

                if (response == null || response.Data == null || !response.Data.Any())
                {
                    _logger.LogInformation("No schedule found for caregiver {User}.", User.Identity?.Name);
                    return NotFound(new { message = "No scheduled bookings found." });
                }

                return Ok(response.Data);
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
                var result = await _caregiverService.CompleteVisitAsync(bookingId);

                if (result == null || !result.Success)
                    return NotFound(new { message = "Booking not found or could not be completed." });

                _logger.LogInformation("Caregiver {User} completed booking {BookingId}.", User.Identity?.Name, bookingId);
                return Ok(new { message = "Visit marked as completed.", data = result.Data });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error completing booking {BookingId} for caregiver {User}.", bookingId, User.Identity?.Name);
                return StatusCode(500, new { message = "Unexpected error while completing visit." });
            }
        }
    }
}
