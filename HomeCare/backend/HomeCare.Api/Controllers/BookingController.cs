using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using HomeCare.Api.DTOs;
using HomeCare.Api.Services;

namespace HomeCare.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class BookingController : ControllerBase
{
    private readonly BookingService _service;

    public BookingController(BookingService service)
    {
        _service = service;
    }

    [HttpGet("dates")]
    public async Task<IActionResult> GetDateOptions()
    {
        var dates = await _service.GetAvailableDatesAsync();
        return Ok(dates);
    }

    [HttpGet("slots/{dateId}")]
    public async Task<IActionResult> GetSlots(int dateId)
    {
        var slots = await _service.GetTimeSlotsAsync(dateId);
        return Ok(slots);
    }

    [Authorize(Roles = "User")]
    [HttpPost]
    public async Task<IActionResult> CreateBooking([FromBody] CreateBookingDto dto)
    {
        var result = await _service.CreateBookingAsync(dto, User);
        return Ok(result);
    }

    [Authorize(Roles = "Caregiver")]
    [HttpPut("{id}/status")]
    public async Task<IActionResult> UpdateStatus(int id, [FromBody] UpdateBookingStatusDto dto)
    {
        var result = await _service.UpdateBookingStatusAsync(id, dto.Status);
        return Ok(result);
    }
}
