using HomeCare.Api.DTOs;
using HomeCare.Api.Models;
using HomeCare.Api.Repositories.Interfaces;
using System.Security.Claims;

namespace HomeCare.Api.Services;

public class BookingService
{
    private readonly IBookingRepository _repo;

    public BookingService(IBookingRepository repo)
    {
        _repo = repo;
    }

    public Task<IEnumerable<AvailableDate>> GetAvailableDatesAsync()
        => _repo.GetAvailableDatesAsync();

    public Task<IEnumerable<TimeSlot>> GetTimeSlotsAsync(int dateId)
        => _repo.GetTimeSlotsForDateAsync(dateId);

    public async Task<object> CreateBookingAsync(CreateBookingDto dto, ClaimsPrincipal user)
    {
        var timeSlot = await _repo.GetAvailableTimeSlotAsync(dto.TimeSlotId);
        if (timeSlot == null || timeSlot.IsBooked)
            throw new Exception("Time slot not available");

        var userId = user.FindFirstValue(ClaimTypes.NameIdentifier)!;

        var startTime = TimeSpan.Parse(timeSlot.Slot.Split('-')[0]);

        var appointment = new Appointment
        {
            UserId = userId,
            TimeSlotId = dto.TimeSlotId,
            CategoryId = dto.CategoryId,
            Notes = dto.Notes,
            DateTime = timeSlot.AvailableDate.Date.Add(startTime)
        };

        await _repo.AddAppointmentAsync(appointment);

        timeSlot.IsBooked = true;
        await _repo.UpdateTimeSlotAsync(timeSlot);

        return new { message = "Appointment created", appointment };
    }

    public async Task<object> UpdateBookingStatusAsync(int id, string status)
    {
        var appt = await _repo.GetAppointmentByIdAsync(id);
        if (appt == null)
            throw new Exception("Appointment not found");

        appt.Status = status;
        await _repo.UpdateAppointmentAsync(appt);

        return new { message = "Status updated", appt };
    }

    // caregiver functionality
    public Task<IEnumerable<Appointment>> GetScheduleForCaregiverAsync(ClaimsPrincipal user)
    {
        var caregiverId = user.FindFirstValue(ClaimTypes.NameIdentifier)!;
        return _repo.GetAppointmentsForCaregiverAsync(caregiverId);
    }

    public Task<IEnumerable<ApplicationUser>> GetClientsForCaregiverAsync(ClaimsPrincipal user)
    {
        var caregiverId = user.FindFirstValue(ClaimTypes.NameIdentifier)!;
        return _repo.GetClientsForCaregiverAsync(caregiverId);
    }

    public async Task<object> CompleteVisitAsync(int appointmentId)
    {
        var appt = await _repo.GetAppointmentByIdAsync(appointmentId);
        if (appt == null)
            throw new Exception("Appointment not found");

        appt.Status = "Completed";
        await _repo.UpdateAppointmentAsync(appt);

        return new { message = "Visit completed", appt };
    }
}
