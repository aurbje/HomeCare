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
    [Authorize(Roles = $"{UserRoleExtensions.Roles.Caregiver},{UserRoleExtensions.Roles.Admin}")]
    [ApiController]
    [Route("api/[controller]")]
    public class CaregiverController : AuthorizedControllerBase
    {
        private readonly ICaregiverService _service;

        public CaregiverController(ICaregiverService service)
        {
            _service = service;
        }

        // dashboard data
        [HttpGet("dashboard")]
        public async Task<IActionResult> GetDashboard([FromQuery] int? year, [FromQuery] int? month)
        {
            try
            {
                var userId = GetCurrentUserId();

                int calendarYear = year ?? DateTime.Today.Year;
                int calendarMonth = month ?? DateTime.Today.Month;

                var model = await _service.GetDashboardAsync(userId, year, month);

                return Ok(new
                {
                    calendarYear,
                    calendarMonth,
                    availableDates = model?.AvailableDates?.OrderBy(d => d.Date).ToList() ?? new List<DateTime>(),
                    caregiverId = userId,
                    model
                });
            }
            catch (Exception ex)
            {
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

        [HttpPost("availability/batch")]
        public async Task<IActionResult> RegisterMultipleAvailability([FromBody] List<DateTime> SelectedDates)
        {
            var userId = GetCurrentUserId();
            await _service.RegisterMultipleAvailabilityAsync(userId, SelectedDates);
            return NoContent();
        }

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
