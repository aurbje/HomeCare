using HomeCare.Api.Models;

namespace HomeCare.Api.DAL.Interfaces
{
    public interface IAdminRepository
    {
        // Users
        Task<IEnumerable<User>> GetUsersAsync(string? searchTerm);
        Task<User?> GetUserByIdAsync(int id);
        Task<User> AddUserAsync(User user);
        Task<bool> UpdateUserAsync(User user);
        Task<bool> DeleteUserAsync(int id);

        // Personnel (users with role = Caregiver)
        Task<IEnumerable<User>> GetPersonnelAsync(string? searchTerm);
        Task<bool> DeletePersonnelAsync(int id);

        // Bookings
        Task<IEnumerable<Booking>> GetBookingsAsync(string? searchTerm);
        Task<Booking?> GetBookingByIdAsync(int id);
        Task<Booking> AddBookingAsync(Booking booking);
        Task<bool> UpdateBookingAsync(Booking booking);
        Task<bool> DeleteBookingAsync(int id);

        // Helpers for constraints
        Task<bool> HasClientBookingsAsync(int userId);
        Task<bool> HasPersonnelBookingsAsync(int personnelId);
        Task<int> CountAdminsAsync();
    }
}