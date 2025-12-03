using HomeCare.Api.Models;

namespace HomeCare.Api.DAL.Interfaces
{
    public interface IBookingRepository
    {
        Task UpdateAvailableDateAsync(AvailableDate availableDate);
        Task<IEnumerable<User>> GetAvailableCaregiverByDateAsync(DateTime date);
        Task<IEnumerable<Booking>> GetUpcomingBookingsAsync();
        Task<Booking?> GetBookingByIdAsync(int id);
        Task AddBookingAsync(Booking booking);
        Task UpdateBookingAsync(Booking booking);
        Task DeleteBookingAsync(int id);
        Task UpdateTimeSlotAsync(TimeSlot timeSlot);
        Task<AvailableDate?> GetAvailableDateByDateAsync(DateTime date);
        Task<IEnumerable<AvailableDate>> GetAvailableDatesAsync();
        Task<IEnumerable<Category>> GetCategoriesAsync();
        Task<TimeSlot?> GetAvailableTimeSlotAsync(int timeSlotId);
        Task<Category?> GetCategoryByIdAsync(int categoryId);
        Task SaveChangesAsync();
        Task AddCaregiverAvailabilityAsync(int CaregiverId, DateTime date);
        Task<User?> GetUserByFullNameAsync(string fullName);
        Task<User?> GetUserByIdAsync(int selectedCaregiverId);
    }
}
