using HomeCare.Api.Models;

namespace HomeCare.Api.DAL.Interfaces
{
    public interface IUserRepository
    {
        // basic user operations
        Task<User?> GetByIdAsync(int id);
        Task<User?> GetByEmailAsync(string email);
        Task<bool> EmailExistsAsync(string email);
        Task AddAsync(User user);
        Task UpdateAsync(User user);
        Task DeleteAsync(User user);
        Task SaveChangesAsync();

        // client-specific dashboard operations
        Task<List<Reminder>> GetRemindersAsync(int UserId);
        Task<List<Booking>> GetTodayBookingsAsync(int UserId);
        Task<List<Booking>> GetUpcomingBookingsAsync(int UserId, int limit = 5);
        Task<List<Booking>> GetCalendarBookingsAsync(int UserId);
    }
}
