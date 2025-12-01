using HomeCare.Api.Models;

namespace HomeCare.Api.DAL.Interfaces
{
    public interface IBookingRepository
    {
        // ------------------------------
        // BOOKINGS
        // ------------------------------
        Task<IEnumerable<Booking>> GetAllBookingsAsync();
        Task<Booking?> GetBookingByIdAsync(int id);
        Task AddBookingAsync(Booking booking);
        Task<bool> UpdateBookingAsync(Booking booking);
        Task<bool> DeleteBookingAsync(int id);

        // ------------------------------
        // SUPPORT DATA
        // ------------------------------
        Task<IEnumerable<AvailableDate>> GetAvailableDatesAsync();
        Task<IEnumerable<Category>> GetCategoriesAsync();
        Task<TimeSlot?> GetAvailableTimeSlotAsync(int timeSlotId);
        Task<Category?> GetCategoryByIdAsync(int categoryId);
        Task<bool> UpdateTimeSlotAsync(TimeSlot timeSlot);

        // ------------------------------
        // DB OPERATIONS
        // ------------------------------
        Task SaveChangesAsync();
    }
}
