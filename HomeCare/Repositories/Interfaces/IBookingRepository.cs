using HomeCare.Models;

namespace HomeCare.Repositories.Interfaces
{
    public interface IBookingRepository
    {
        Task<IEnumerable<Booking>> GetAllBookingsAsync();   
        Task<Booking?> GetBookingByIdAsync(int id);         // get a booking  by id
        Task AddBookingAsync(Booking booking);              // add a new booking
        Task UpdateBookingAsync(Booking booking);           // update existing booking
        Task DeleteBookingAsync(int id);                    // delete a booking by id

        // appointments
        Task<IEnumerable<Appointment>> GetUpcomingAppointmentsAsync();
        Task<Appointment?> GetAppointmentByIdAsync(int id);
        Task AddAppointmentAsync(Appointment appointment);
        Task UpdateAppointmentAsync(Appointment appointment);
        Task DeleteAppointmentAsync(int id);
        Task UpdateTimeSlotAsync(TimeSlot timeSlot);

        // supporting data
        Task<IEnumerable<AvailableDate>> GetAvailableDatesAsync();
        Task<IEnumerable<Category>> GetCategoriesAsync();
        Task<TimeSlot?> GetAvailableTimeSlotAsync(int timeSlotId);
        Task<Category?> GetCategoryByIdAsync(int categoryId);

        Task SaveChangesAsync();
    }
}
