using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using HomeCare.Api.Services;

namespace HomeCare.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize(Roles = "Caregiver")]
public class CaregiverController : ControllerBase
{
    private readonly BookingService _bookingService;

    public CaregiverController(BookingService bookingService)
    {
        _bookingService = bookingService;
    }

    [HttpGet("clients")]
    public async Task<IActionResult> GetAssignedClients()
    {
        var clients = await _bookingService.GetClientsForCaregiverAsync(User);
        return Ok(clients);
    }

    [HttpGet("schedule")]
    public async Task<IActionResult> GetSchedule()
    {
        var schedule = await _bookingService.GetScheduleForCaregiverAsync(User);
        return Ok(schedule);
    }

    [HttpPut("visit/{bookingId}/complete")]
    public async Task<IActionResult> CompleteVisit(int bookingId)
    {
        var result = await _bookingService.CompleteVisitAsync(bookingId);
        return Ok(result);
    }
}
