using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using HomeCare.Api.Models;
using HomeCare.Api.DTO.User;
using HomeCare.Api.Enums;
using System.Threading.Tasks;
using System;
using System.Collections.Generic;
using System.Linq;
using HomeCare.Api.Services.Interfaces;

namespace HomeCare.Api.Controllers
{
    // controller for managing bookings
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class BookingController : AuthorizedControllerBase
    {
        private readonly IBookingService _bookingService;
        private readonly ILogger<BookingController> _logger;

        public BookingController(IBookingService bookingService, ILogger<BookingController> logger)
        {
            _bookingService = bookingService;
            _logger = logger;
        }

// GET: api/Booking/init
        [HttpGet("init")]
        public async Task<IActionResult> GetBookingInit()
        {
            var userId = GetCurrentUserId();
            var result = await _bookingService.GetBookingInitAsync(userId);
            return Ok(result);
        }

// POST: api/Booking
        [HttpPost]
        public async Task<IActionResult> CreateOrUpdateBooking([FromBody] BookingRequestDto model)
        {
            var UserId = GetCurrentUserId();
            var result = await _bookingService.CreateOrUpdateBookingAsync(model, UserId);

            return result.ResultType switch
            {
                BookingResultType.Success => Ok(new { message = result.Message, bookingId = result.BookingId }),
                BookingResultType.NotFound => NotFound(new { message = result.Message }),
                BookingResultType.Forbidden => Forbid(),
                BookingResultType.ValidationError => ValidationProblem(CreateModelState(result.ValidationErrors)),
                _ => BadRequest(new { message = result.Message })
            };
        }

// DELETE: api/Booking/id
        [HttpDelete("{id}")]
        public async Task<IActionResult> CancelBooking(int id)
        {
            var userId = GetCurrentUserId();
            var isAdmin = IsInRole(UserRoleExtensions.Roles.Admin);
            var result = await _bookingService.CancelBookingAsync(id, userId, isAdmin);

            return result.ResultType switch
            {
                BookingResultType.Success => NoContent(),
                BookingResultType.NotFound => NotFound(new { message = result.Message }),
                BookingResultType.Forbidden => Forbid(),
                _ => BadRequest(new { message = result.Message })
            };
        }
// GET: api/Booking/id
        [HttpGet("{id}")]
        public async Task<IActionResult> GetBooking(int id)
        {
            var userId = GetCurrentUserId();
            var isAdmin = IsInRole(UserRoleExtensions.Roles.Admin);
            var booking = await _bookingService.GetBookingAsync(id, userId, isAdmin);

            if (booking == null) return NotFound();
            return Ok(booking);
        }

// GET: api/Booking/select-Caregiver
        [HttpGet("select-Caregiver")]
        public async Task<IActionResult> SelectCaregiver([FromQuery] string selectedDate, [FromQuery] int? timeSlotId, [FromQuery] int? bookingId)
        {
            if (string.IsNullOrWhiteSpace(selectedDate))
            {
                return Ok(Array.Empty<object>());
            }

            if (!DateTime.TryParseExact(selectedDate, "yyyy-MM-dd", System.Globalization.CultureInfo.InvariantCulture, System.Globalization.DateTimeStyles.None, out var date))
            {
                if (!DateTime.TryParse(selectedDate, out date))
                {
                    return Ok(Array.Empty<object>());
                }
            }
// if timeSlotId is not provided, return empty list 
            var Caregiver = await _bookingService.GetAvailableCaregiverForSlotAsync(date, timeSlotId, bookingId);
            var CaregiverDto = Caregiver.Select(p => new { p.Id, p.FullName }).ToList();
            return Ok(CaregiverDto);
        }
// helper method to create ModelStateDictionary from validation errors
        private Microsoft.AspNetCore.Mvc.ModelBinding.ModelStateDictionary CreateModelState(Dictionary<string, string>? errors)
        {
            var modelState = new Microsoft.AspNetCore.Mvc.ModelBinding.ModelStateDictionary();
            if (errors != null)
            {
                foreach (var (key, value) in errors)
                {
                    modelState.AddModelError(key, value);
                }
            }
            return modelState;
        }
    }
}