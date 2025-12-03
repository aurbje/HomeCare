using HomeCare.Api.Models;

namespace HomeCare.Api.DAL.Interfaces
{
    // Admin data access: list and basic operations on users, caregivers and bookings
    public interface IAdminRepository
    {
        // Users
        Task<IEnumerable<User>> GetUsersAsync(string? searchTerm);
        Task<User?> GetUserByIdAsync(int id);
        Task<User> AddUserAsync(User user);
        Task<bool> UpdateUserAsync(User user);
        Task<bool> DeleteUserAsync(int id);

        // Caregivers
        Task<IEnumerable<User>> GetCaregiversAsync(string? searchTerm);
        Task<bool> DeleteCaregiverAsync(int id);

        // Bookings
        Task<IEnumerable<Booking>> GetBookingsAsync(string? searchTerm);
        Task<Booking?> GetBookingByIdAsync(int id);
        Task<Booking> AddBookingAsync(Booking booking);
        Task<bool> UpdateBookingAsync(Booking booking);
        Task<bool> DeleteBookingAsync(int id);

        // Helpers for constraints
        Task<bool> HasClientBookingsAsync(int userId);
        Task<bool> HasCaregiverBookingsAsync(int caregiverUserId);
        Task<int> CountAdminsAsync();
    }
}