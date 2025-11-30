using Microsoft.AspNetCore.Mvc;
using HomeCare.Models;
using HomeCare.ViewModels;
using HomeCare.Repositories.Interfaces;
using Microsoft.Extensions.Logging;

namespace HomeCare.Controllers
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
                var appointments = (await _bookingRepo.GetUpcomingAppointmentsAsync()).ToList();

                var model = new BookingPageDto
                {
                    SelectedDate = DateTime.Today,
                    CategoryId = categories.FirstOrDefault()?.Id ?? 0,
                    AvailableDates = availableDates,
                    Categories = categories,
                    Appointments = appointments
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
        public async Task<IActionResult> CreateOrUpdateBooking([FromBody] BookingViewModel model)
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

                if (selectedCategory.Name.Equals("OTHER", StringComparison.OrdinalIgnoreCase) &&
                    string.IsNullOrWhiteSpace(model.Notes))
                {
                    return BadRequest(new { message = "Please provide details for 'Other' category." });
                }

                var selectedSlot = await _bookingRepo.GetAvailableTimeSlotAsync(model.TimeSlotId);
                if (selectedSlot == null)
                    return BadRequest(new { message = "Selected time slot is no longer available." });

                if (!TimeSpan.TryParse(selectedSlot.Slot.Split('-')[0].Trim(), out TimeSpan startTime))
                    return BadRequest(new { message = "Invalid time slot format." });

                if (model.AppointmentId > 0)
                {
                    var existing = await _bookingRepo.GetAppointmentByIdAsync(model.AppointmentId);
                    if (existing == null)
                        return NotFound(new { message = "Appointment not found." });

                    existing.DateTime = selectedSlot.AvailableDate.Date.Add(startTime);
                    existing.TimeSlotId = selectedSlot.Id;
                    existing.CategoryId = selectedCategory.Id;
                    existing.Notes = model.Notes;

                    selectedSlot.IsBooked = true;
                    await _bookingRepo.UpdateTimeSlotAsync(selectedSlot);
                    await _bookingRepo.UpdateAppointmentAsync(existing);

                    return Ok(new
                    {
                        message = $"Appointment updated to {existing.DateTime:yyyy-MM-dd} {selectedSlot.Slot}.",
                        appointment = existing
                    });
                }
                else
                {
                    var appointment = new Appointment
                    {
                        DateTime = selectedSlot.AvailableDate.Date.Add(startTime),
                        TimeSlotId = selectedSlot.Id,
                        CategoryId = selectedCategory.Id,
                        Notes = model.Notes
                    };

                    selectedSlot.IsBooked = true;
                    await _bookingRepo.AddAppointmentAsync(appointment);

                    return StatusCode(201, new
                    {
                        message = $"Appointment booked for {appointment.DateTime:yyyy-MM-dd} {selectedSlot.Slot}.",
                        appointment
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
        public async Task<IActionResult> CancelAppointment(int id)
        {
            _logger.LogInformation("Attempting to cancel appointment {AppointmentId}.", id);

            try
            {
                var appointment = await _bookingRepo.GetAppointmentByIdAsync(id);
                if (appointment == null)
                    return NotFound(new { message = "Appointment not found." });

                var slot = await _bookingRepo.GetAvailableTimeSlotAsync(appointment.TimeSlotId);
                if (slot != null)
                {
                    slot.IsBooked = false;
                    await _bookingRepo.UpdateTimeSlotAsync(slot);
                }

                await _bookingRepo.DeleteAppointmentAsync(id);
                return Ok(new { message = "Appointment cancelled successfully." });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error while cancelling appointment {AppointmentId}.", id);
                return StatusCode(500, new { message = "Unexpected error while cancelling appointment." });
            }
        }

        // GET: /api/booking/{id}
        [HttpGet("{id:int}")]
        public async Task<ActionResult<BookingEditDto>> GetAppointmentForEdit(int id)
        {
            _logger.LogInformation("Loading data for editing appointment {AppointmentId}.", id);

            try
            {
                var appointment = await _bookingRepo.GetAppointmentByIdAsync(id);
                if (appointment == null)
                    return NotFound(new { message = "Appointment not found." });

                var availableDates = (await _bookingRepo.GetAvailableDatesAsync()).ToList();
                var categories = (await _bookingRepo.GetCategoriesAsync()).ToList();
                var appointments = (await _bookingRepo.GetUpcomingAppointmentsAsync()).ToList();

                var model = new BookingEditDto
                {
                    AppointmentId = appointment.Id,
                    SelectedDate = appointment.DateTime.Date,
                    TimeSlotId = appointment.TimeSlotId,
                    CategoryId = appointment.CategoryId,
                    Notes = appointment.Notes,
                    AvailableDates = availableDates,
                    Categories = categories,
                    Appointments = appointments
                };

                return Ok(model);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error while loading appointment {AppointmentId}.", id);
                return StatusCode(500, new { message = "Unexpected error while loading appointment." });
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
        public List<Appointment> Appointments { get; set; } = new();
    }

    public class BookingEditDto
    {
        public int AppointmentId { get; set; }
        public DateTime SelectedDate { get; set; }
        public int TimeSlotId { get; set; }
        public int CategoryId { get; set; }
        public string? Notes { get; set; }
        public List<AvailableDate> AvailableDates { get; set; } = new();
        public List<Category> Categories { get; set; } = new();
        public List<Appointment> Appointments { get; set; } = new();
    }
}
