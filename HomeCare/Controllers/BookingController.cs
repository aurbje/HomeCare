using Microsoft.AspNetCore.Mvc;
using HomeCare.Models;
using HomeCare.ViewModels;
using HomeCare.Repositories.Interfaces;
using Microsoft.Extensions.Logging;
using System;
using System.Linq;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace HomeCare.Controllers
{
    // controller for handling booking-related API endpoints
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

        [HttpGet]
        public async Task<ActionResult<BookingPageDto>> GetBookingPage()
        {
            // just loading all the stuff the frontend needs to render the booking view
            _logger.LogInformation("Loading booking data with available dates, categories and upcoming appointments.");

            try
            {
                var availableDates = (await _bookingRepo.GetAvailableDatesAsync()).ToList();
                var categories = (await _bookingRepo.GetCategoriesAsync()).ToList();
                var appointments = (await _bookingRepo.GetUpcomingAppointmentsAsync()).ToList();

                var model = new BookingPageDto
                {
                    SelectedDate = DateTime.Today,
                    TimeSlotId = 0,
                    CategoryId = categories.FirstOrDefault()?.Id ?? 0,
                    AvailableDates = availableDates,
                    Categories = categories,
                    Appointments = appointments
                };

                return Ok(model);
            }
            catch (Exception ex)
            {
                // if something blows up, we log it and send a generic error to the client
                _logger.LogError(ex, "Error while loading booking data.");
                return StatusCode(500, new
                {
                    message = "An unexpected error occurred while loading booking data."
                });
            }
        }

        [HttpPost]
        public async Task<ActionResult> CreateOrUpdateBooking([FromBody] BookingViewModel model)
        {
            // first we make sure the basic validation rules are ok
            _logger.LogInformation("Booking attempt started for categoryId {CategoryId} on {Date}", model.CategoryId, model.SelectedDate);

            if (!ModelState.IsValid)
            {
                var errors = ModelState
                    .Where(kvp => kvp.Value?.Errors.Any() == true)
                    .ToDictionary(
                        kvp => kvp.Key,
                        kvp => kvp.Value!.Errors.Select(e => e.ErrorMessage).ToArray()
                    );

                _logger.LogWarning("Booking validation failed for categoryId {CategoryId}. Errors: {@Errors}", model.CategoryId, errors);

                return BadRequest(new
                {
                    message = "Validation failed",
                    errors
                });
            }

            try
            {
                // grab the selected category from the db
                var selectedCategory = await _bookingRepo.GetCategoryByIdAsync(model.CategoryId);

                // if category is "OTHER" we require some notes from the user
                if (selectedCategory?.Name.ToUpper() == "OTHER" && string.IsNullOrWhiteSpace(model.Notes))
                {
                    _logger.LogWarning("Booking attempt failed validation: 'Other' category requires notes.");
                    ModelState.AddModelError("Notes", "Please provide details for 'Other' category.");
                }

                // get the time slot and make sure it exists and is free
                var selectedSlot = await _bookingRepo.GetAvailableTimeSlotAsync(model.TimeSlotId);

                if (selectedSlot == null)
                {
                    _logger.LogWarning("Booking attempt failed: Selected time slot {TimeSlotId} is unavailable.", model.TimeSlotId);
                    ModelState.AddModelError("TimeSlotId", "Selected time slot is no longer available.");
                }

                // trying to parse the start time from the slot string like "10:00-11:00"
                TimeSpan startTime = default;
                bool validTime = false;

                if (selectedSlot != null)
                {
                    var slotParts = selectedSlot.Slot.Replace("–", "-").Split('-');
                    if (slotParts.Length == 2 && TimeSpan.TryParse(slotParts[0], out startTime))
                    {
                        validTime = true;
                    }
                    else
                    {
                        _logger.LogError("Invalid time slot format for slot {Slot}.", selectedSlot.Slot);
                        ModelState.AddModelError("TimeSlotId", "Invalid time slot format.");
                    }
                }

                // if we still have validation errors at this point, just return them
                if (!ModelState.IsValid || !validTime)
                {
                    var errors = ModelState
                        .Where(kvp => kvp.Value?.Errors.Any() == true)
                        .ToDictionary(
                            kvp => kvp.Key,
                            kvp => kvp.Value!.Errors.Select(e => e.ErrorMessage).ToArray()
                        );

                    _logger.LogWarning("Booking failed: Model state invalid after slot and category checks.");
                    return BadRequest(new
                    {
                        message = "Validation failed",
                        errors
                    });
                }

                // by now we know the model is good and the time is valid, so we can either update or create
                if (model.AppointmentId > 0)
                {
                    // editing an existing appointment
                    var existing = await _bookingRepo.GetAppointmentByIdAsync(model.AppointmentId);
                    if (existing != null)
                    {
                        // updating the appointment details based on the selected slot and category
                        existing.DateTime = selectedSlot!.AvailableDate.Date.Add(startTime);
                        existing.TimeSlotId = selectedSlot.Id;
                        existing.CategoryId = selectedCategory!.Id;
                        existing.Notes = model.Notes;

                        selectedSlot.IsBooked = true;
                        await _bookingRepo.UpdateTimeSlotAsync(selectedSlot);

                        await _bookingRepo.UpdateAppointmentAsync(existing);

                        _logger.LogInformation("Appointment {AppointmentId} updated successfully.", existing.Id);

                        return Ok(new
                        {
                            message = $"Appointment has changed to {existing.DateTime:yyyy-MM-dd} {selectedSlot.Slot}!",
                            appointment = existing
                        });
                    }
                    else
                    {
                        _logger.LogWarning("Update failed: Appointment {AppointmentId} not found.", model.AppointmentId);
                        return NotFound(new
                        {
                            message = "Appointment not found."
                        });
                    }
                }
                else
                {
                    // creating a brand new appointment
                    var appointment = new Appointment
                    {
                        DateTime = selectedSlot!.AvailableDate.Date.Add(startTime),
                        TimeSlotId = selectedSlot.Id,
                        CategoryId = selectedCategory!.Id,
                        Notes = model.Notes
                    };

                    selectedSlot.IsBooked = true;

                    await _bookingRepo.AddAppointmentAsync(appointment);

                    _logger.LogInformation("New appointment created for {DateTime} in category {CategoryId}.",
                        appointment.DateTime, appointment.CategoryId);

                    return StatusCode(201, new
                    {
                        message = $"Appointment booked for {model.SelectedDate:yyyy-MM-dd} {selectedSlot.Slot}!",
                        appointment
                    });
                }
            }
            catch (Exception ex)
            {
                // something unexpected happened while saving the booking
                _logger.LogError(ex, "Error occurred while creating or updating a booking.");
                return StatusCode(500, new
                {
                    message = "An unexpected error occurred while saving the booking."
                });
            }
        }

        [HttpDelete("appointment/{id:int}")]
        public async Task<ActionResult> CancelAppointment(int id)
        {
            _logger.LogInformation("Attempting to cancel appointment {AppointmentId}.", id);

            try
            {
                var appointment = await _bookingRepo.GetAppointmentByIdAsync(id);
                if (appointment != null)
                {
                    // free up the time slot again
                    var slot = await _bookingRepo.GetAvailableTimeSlotAsync(appointment.TimeSlotId);
                    if (slot != null)
                    {
                        slot.IsBooked = false;
                        await _bookingRepo.UpdateTimeSlotAsync(slot);
                    }

                    await _bookingRepo.DeleteAppointmentAsync(id);

                    _logger.LogInformation("Appointment {AppointmentId} cancelled successfully.", id);

                    return Ok(new
                    {
                        message = "Appointment has been cancelled."
                    });
                }
                else
                {
                    _logger.LogWarning("Cancel attempt failed: Appointment {AppointmentId} not found.", id);
                    return NotFound(new
                    {
                        message = "Appointment not found."
                    });
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while cancelling appointment {AppointmentId}.", id);
                return StatusCode(500, new
                {
                    message = "An unexpected error occurred while cancelling the appointment."
                });
            }
        }

        [HttpGet("appointment/{id:int}")]
        public async Task<ActionResult<BookingEditDto>> GetAppointmentForEdit(int id)
        {
            _logger.LogInformation("Loading edit data for appointment {AppointmentId}.", id);

            try
            {
                var appointment = await _bookingRepo.GetAppointmentByIdAsync(id);

                if (appointment == null)
                {
                    _logger.LogWarning("Edit attempt failed: Appointment {AppointmentId} not found.", id);
                    return NotFound(new
                    {
                        message = "Appointment not found."
                    });
                }

                // frontend needs both the appointment data and the lists to build the form
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

                _logger.LogInformation("Edit data loaded successfully for appointment {AppointmentId}.", id);

                return Ok(model);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while loading edit data for appointment {AppointmentId}.", id);
                return StatusCode(500, new
                {
                    message = "An unexpected error occurred while loading the appointment."
                });
            }
        }
    }

// loading data for the booking page
    public class BookingPageDto
    {
        public DateTime SelectedDate { get; set; }
        public int TimeSlotId { get; set; }
        public int CategoryId { get; set; }
        public List<AvailableDate> AvailableDates { get; set; } = new();
        public List<Category> Categories { get; set; } = new();
        public List<Appointment> Appointments { get; set; } = new();
    }

    // to edit an existing appointment
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
