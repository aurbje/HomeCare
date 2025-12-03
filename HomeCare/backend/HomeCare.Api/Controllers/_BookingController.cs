/* ============================================================
 * ORIGINAL GROUP CODE - Commented out
 * Replaced with merged version in BookingController.cs
 * This file kept for reference purposes
 * ============================================================

using Microsoft.AspNetCore.Mvc;
using HomeCare.Api.Models;
using HomeCare.Api.DTO;
using HomeCare.Api.DAL.Interfaces;
using HomeCare.Api.DTO.User;

namespace HomeCare.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class BookingController : ControllerBase
    {
        private readonly IBookingRepository _bookingRepo;
        private readonly ILogger<BookingController> _logger;

        public BookingController(IBookingRepository bookingRepo, ILogger<BookingController> logger)
        {
            _bookingRepo = bookingRepo;
            _logger = logger;
        }

        // GET: /api/booking
        [HttpGet]
        public async Task<ActionResult<BookingPageDto>> GetBookingPage()
        {
            _logger.LogInformation("Loading booking page data.");

            try
            {
                var availableDates = (await _bookingRepo.GetAvailableDatesAsync()).ToList();
                var categories = (await _bookingRepo.GetCategoriesAsync()).ToList();
                var bookings = (await _bookingRepo.GetAllBookingsAsync()).ToList();

                var model = new BookingPageDto
                {
                    SelectedDate = DateTime.Today,
                    CategoryId = categories.FirstOrDefault()?.Id ?? 0,
                    AvailableDates = availableDates,
                    Categories = categories,
                    Bookings = bookings
                };

                return Ok(model);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error while loading booking data.");
                return StatusCode(500, new { message = "Unexpected error while loading booking data." });
            }
        }

        // POST: /api/booking
        [HttpPost]
        public async Task<IActionResult> CreateOrUpdateBooking([FromBody] CreateBookingDto model)
        {
            _logger.LogInformation("Booking request for category {CategoryId} on {Date}", model.CategoryId, model.SelectedDate);

            if (!ModelState.IsValid)
            {
                var errors = ModelState
                    .Where(kvp => kvp.Value?.Errors.Any() == true)
                    .ToDictionary(kvp => kvp.Key, kvp => kvp.Value!.Errors.Select(e => e.ErrorMessage).ToArray());

                return BadRequest(new { message = "Validation failed", errors });
            }

            try
            {
                var selectedCategory = await _bookingRepo.GetCategoryByIdAsync(model.CategoryId);
                if (selectedCategory == null)
                    return BadRequest(new { message = "Invalid category." });

                var selectedSlot = await _bookingRepo.GetAvailableTimeSlotAsync(model.TimeSlotId);
                if (selectedSlot == null)
                    return BadRequest(new { message = "Selected time slot is no longer available." });

                // Parse slot time string (e.g. "10:00-11:00")
                if (!TimeSpan.TryParse(selectedSlot.Slot.Split('-')[0].Trim(), out TimeSpan startTime))
                    return BadRequest(new { message = "Invalid time slot format." });

                if (model.BookingId > 0)
                {
                    var existing = await _bookingRepo.GetBookingByIdAsync(model.BookingId);
                    if (existing == null)
                        return NotFound(new { message = "Booking not found." });

                    existing.Date = selectedSlot.AvailableDate.Date;
                    existing.Time = selectedSlot.Slot;
                    existing.TimeSlotId = selectedSlot.Id;
                    existing.ServiceType = selectedCategory.Name;
                    existing.Notes = model.Notes;

                    selectedSlot.IsBooked = true;
                    await _bookingRepo.UpdateTimeSlotAsync(selectedSlot);
                    await _bookingRepo.UpdateBookingAsync(existing);

                    return Ok(new
                    {
                        message = $"Booking updated for {existing.Date:yyyy-MM-dd} at {existing.Time}.",
                        booking = existing
                    });
                }
                else
                {
                    var booking = new Booking
                    {
                        Date = selectedSlot.AvailableDate.Date,
                        Time = selectedSlot.Slot,
                        TimeSlotId = selectedSlot.Id,
                        CategoryId = selectedCategory.Id,
                        ServiceType = selectedCategory.Name,
                        Notes = model.Notes,
                        Status = "Booked"
                    };

                    selectedSlot.IsBooked = true;
                    await _bookingRepo.AddBookingAsync(booking);

                    return StatusCode(201, new
                    {
                        message = $"Booking created for {booking.Date:yyyy-MM-dd} {booking.Time}.",
                        booking
                    });
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error while creating or updating booking.");
                return StatusCode(500, new { message = "Unexpected error while saving booking." });
            }
        }

        // DELETE: /api/booking/{id}
        [HttpDelete("{id:int}")]
        public async Task<IActionResult> CancelBooking(int id)
        {
            _logger.LogInformation("Attempting to cancel booking {BookingId}.", id);

            try
            {
                var booking = await _bookingRepo.GetBookingByIdAsync(id);
                if (booking == null)
                    return NotFound(new { message = "Booking not found." });

                var slot = await _bookingRepo.GetAvailableTimeSlotAsync(booking.TimeSlotId);
                if (slot != null)
                {
                    slot.IsBooked = false;
                    await _bookingRepo.UpdateTimeSlotAsync(slot);
                }

                await _bookingRepo.DeleteBookingAsync(id);
                return Ok(new { message = "Booking cancelled successfully." });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error while cancelling booking {BookingId}.", id);
                return StatusCode(500, new { message = "Unexpected error while cancelling booking." });
            }
        }

        // GET: /api/booking/{id}
        [HttpGet("{id:int}")]
        public async Task<ActionResult<BookingEditDto>> GetBookingForEdit(int id)
        {
            _logger.LogInformation("Loading data for editing booking {BookingId}.", id);

            try
            {
                var booking = await _bookingRepo.GetBookingByIdAsync(id);
                if (booking == null)
                    return NotFound(new { message = "Booking not found." });

                var availableDates = (await _bookingRepo.GetAvailableDatesAsync()).ToList();
                var categories = (await _bookingRepo.GetCategoriesAsync()).ToList();
                var bookings = (await _bookingRepo.GetAllBookingsAsync()).ToList();

                var model = new BookingEditDto
                {
                    BookingId = booking.Id,
                    SelectedDate = booking.Date,
                    TimeSlotId = booking.TimeSlotId,
                    CategoryId = booking.CategoryId,
                    Notes = booking.Notes,
                    AvailableDates = availableDates,
                    Categories = categories,
                    Bookings = bookings
                };

                return Ok(model);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error while loading booking {BookingId}.", id);
                return StatusCode(500, new { message = "Unexpected error while loading booking." });
            }
        }
    }

    // DTOs (response shapes)
    public class BookingPageDto
    {
        public DateTime SelectedDate { get; set; }
        public int CategoryId { get; set; }
        public List<AvailableDate> AvailableDates { get; set; } = new();
        public List<Category> Categories { get; set; } = new();
        public List<Booking> Bookings { get; set; } = new();
    }

    public class BookingEditDto
    {
        public int BookingId { get; set; }
        public DateTime SelectedDate { get; set; }
        public int TimeSlotId { get; set; }
        public int CategoryId { get; set; }
        public string? Notes { get; set; }
        public List<AvailableDate> AvailableDates { get; set; } = new();
        public List<Category> Categories { get; set; } = new();
        public List<Booking> Bookings { get; set; } = new();
    }
}

*/