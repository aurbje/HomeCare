using HomeCare.Api.DTO;
using HomeCare.Api.Models;

namespace HomeCare.Api.Services.Interfaces
{
    public interface IUserService
    {
        Task<UserDashboardDto?> GetDashboardAsync(int userId);

        // Task<List<Reminder>> GetRemindersAsync(int clientId);

        Task<List<Booking>> GetTodayBookingsAsync(int clientId);

        Task<List<Booking>> GetUpcomingBookingsAsync(int clientId, int limit = 5);
    }
}
