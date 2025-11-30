using HomeCare.Api.DTOs;
using HomeCare.Api.Models;
using HomeCare.Api.Repositories.Interfaces;
using System.Security.Claims;

namespace HomeCare.Api.Services
{
    public class BookingService
    {
        private readonly IBookingRepository _repo;
        private readonly ILogger<BookingService> _logger;

        public BookingService(IBookingRepository repo, ILogger<BookingService> logger)
        {
            _repo = repo;
            _logger = logger;
        }

        public async Task<ServiceResponse<IEnumerable<AvailableDate>>> GetAvailableDatesAsync()
        {
            try
            {
                var dates = await _repo.GetAvailableDatesAsync();
                return ServiceResponse<IEnumerable<AvailableDate>>.Success(dates);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving available dates");
                return ServiceResponse<IEnumerable<AvailableDate>>.Fail("Failed to load available dates");
            }
        }

        public async Task<ServiceResponse<IEnumerable<TimeSlot>>> GetTimeSlotsAsync(int dateId)
        {
            try
            {
                var slots = await _repo.GetTimeSlotsForDateAsync(dateId);
                return ServiceResponse<IEnumerable<TimeSlot>>.Success(slots);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving timeslots for dateId {DateId}", dateId);
                return ServiceResponse<IEnumerable<TimeSlot>>.Fail("Failed to load timeslots");
            }
        }

        public async Task<ServiceResponse<object>> CreateBookingAsync(CreateBookingDto dto, ClaimsPrincipal user)
        {
            try
            {
                var userId = user.FindFirstValue(ClaimTypes.NameIdentifier);
                if (userId == null)
                    return ServiceResponse<object>.Fail("User not authenticated");

                var timeSlot = await _repo.GetAvailableTimeSlotAsync(dto.TimeSlotId);
                if (timeSlot == null || timeSlot.IsBooked)
                    return ServiceResponse<object>.Fail("Time slot not available");

                var startTime = TimeSpan.Parse(timeSlot.Slot.Split('-')[0]);

                var appointment = new Appointment
                {
                    UserId = userId,
                    TimeSlotId = dto.TimeSlotId,
                    CategoryId = dto.CategoryId,
                    Notes = dto.Notes,
                    DateTime = timeSlot.AvailableDate.Date.Add(startTime),
                    Status = "Booked"
                };

                await _repo.AddAppointmentAsync(appointment);

                timeSlot.IsBooked = true;
                await _repo.UpdateTimeSlotAsync(timeSlot);

                _logger.LogInformation("Created booking for user {UserId} at {DateTime}", userId, appointment.DateTime);

                return ServiceResponse<object>.Success(new
                {
                    message = "Appointment created successfully",
                    appointment
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error creating booking");
                return ServiceResponse<object>.Fail("An unexpected error occurred while creating booking");
            }
        }

        public async Task<ServiceResponse<object>> UpdateBookingStatusAsync(int id, string status)
        {
            try
            {
                var appt = await _repo.GetAppointmentByIdAsync(id);
                if (appt == null)
                    return ServiceResponse<object>.Fail("Appointment not found");

                appt.Status = status;
                await _repo.UpdateAppointmentAsync(appt);

                _logger.LogInformation("Updated booking {AppointmentId} status to {Status}", id, status);

                return ServiceResponse<object>.Success(new { message = "Status updated", appt });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error updating booking status for appointment {AppointmentId}", id);
                return ServiceResponse<object>.Fail("Error updating booking status");
            }
        }

        // caregiver functionality
        public async Task<ServiceResponse<IEnumerable<Appointment>>> GetScheduleForCaregiverAsync(ClaimsPrincipal user)
        {
            try
            {
                var caregiverId = user.FindFirstValue(ClaimTypes.NameIdentifier);
                if (caregiverId == null)
                    return ServiceResponse<IEnumerable<Appointment>>.Fail("User not authenticated");

                var schedule = await _repo.GetAppointmentsForCaregiverAsync(caregiverId);
                return ServiceResponse<IEnumerable<Appointment>>.Success(schedule);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error fetching caregiver schedule");
                return ServiceResponse<IEnumerable<Appointment>>.Fail("Failed to load caregiver schedule");
            }
        }

        public async Task<ServiceResponse<IEnumerable<ApplicationUser>>> GetClientsForCaregiverAsync(ClaimsPrincipal user)
        {
            try
            {
                var caregiverId = user.FindFirstValue(ClaimTypes.NameIdentifier);
                if (caregiverId == null)
                    return ServiceResponse<IEnumerable<ApplicationUser>>.Fail("User not authenticated");

                var clients = await _repo.GetClientsForCaregiverAsync(caregiverId);
                return ServiceResponse<IEnumerable<ApplicationUser>>.Success(clients);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error fetching caregiver clients");
                return ServiceResponse<IEnumerable<ApplicationUser>>.Fail("Failed to load clients for caregiver");
            }
        }

        public async Task<ServiceResponse<object>> CompleteVisitAsync(int appointmentId)
        {
            try
            {
                var appt = await _repo.GetAppointmentByIdAsync(appointmentId);
                if (appt == null)
                    return ServiceResponse<object>.Fail("Appointment not found");

                appt.Status = "Completed";
                await _repo.UpdateAppointmentAsync(appt);

                _logger.LogInformation("Marked appointment {AppointmentId} as completed", appointmentId);

                return ServiceResponse<object>.Success(new { message = "Visit marked as completed", appt });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error completing visit for appointment {AppointmentId}", appointmentId);
                return ServiceResponse<object>.Fail("Failed to complete visit");
            }
        }
    }

    // Generic helper for consistent responses across the backend
    public class ServiceResponse<T>
    {
        public bool Success { get; set; }
        public string Message { get; set; } = string.Empty;
        public T? Data { get; set; }

        public static ServiceResponse<T> Success(T data, string message = "") =>
            new() { Success = true, Data = data, Message = message };

        public static ServiceResponse<T> Fail(string message) =>
            new() { Success = false, Message = message };
    }
}
