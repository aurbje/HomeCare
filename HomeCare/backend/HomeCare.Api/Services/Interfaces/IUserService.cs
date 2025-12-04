using HomeCare.Api.DTO.User;
using HomeCare.Api.Models;

namespace HomeCare.Api.Services.Interfaces
{
    // Service interface for user dashboard operations.
    public interface IUserService
    {
        // Gets dashboard data for a user including reminders, today's bookings, and upcoming bookings.
        // <param name="userId">The user's ID</param>
        // <returns>Dashboard DTO with all user data</returns>
        Task<UserDashboardDto?> GetDashboardAsync(int userId);

        // Gets reminders for a specific client.
        Task<List<Reminder>> GetRemindersAsync(int clientId);

        // Gets today's bookings for a client.
        Task<List<Booking>> GetTodayBookingsAsync(int clientId);

        // Gets upcoming bookings for a client.
        Task<List<Booking>> GetUpcomingBookingsAsync(int clientId, int limit = 5);
    }
}
