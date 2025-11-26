using HomeCare.Api.Models;

namespace HomeCare.Api.Repositories.Interfaces;

public interface IBookingRepository
{
    // date & timeslot
    Task<IEnumerable<AvailableDate>> GetAvailableDatesAsync();
    Task<IEnumerable<TimeSlot>> GetTimeSlotsForDateAsync(int dateId);
    Task<TimeSlot?> GetAvailableTimeSlotAsync(int timeSlotId);
    Task UpdateTimeSlotAsync(TimeSlot slot);

    // categories
    Task<IEnumerable<Category>> GetCategoriesAsync();
    Task<Category?> GetCategoryByIdAsync(int categoryId);

    // appointments (bookings)
    Task<Appointment?> GetAppointmentByIdAsync(int id);
    Task AddAppointmentAsync(Appointment appointment);
    Task UpdateAppointmentAsync(Appointment appointment);
    Task DeleteAppointmentAsync(int appointmentId);
}
