using HomeCare.Api.DTO;
using HomeCare.Api.Models;

namespace HomeCare.Api.Services.Interfaces
{
    /// <summary>
    /// Service interface for user dashboard operations.
    /// </summary>
    public interface IUserService
    {
        /// <summary>
        /// Gets dashboard data for a user including reminders, today's bookings, and upcoming bookings.
        /// </summary>
        /// <param name="userId">The user's ID</param>
        /// <returns>Dashboard DTO with all user data</returns>
        Task<UserDashboardDto?> GetDashboardAsync(int userId);

        /// <summary>
        /// Gets reminders for a specific client.
        /// </summary>
        Task<List<Reminder>> GetRemindersAsync(int clientId);

        /// <summary>
        /// Gets today's bookings for a client.
        /// </summary>
        Task<List<Booking>> GetTodayBookingsAsync(int clientId);

        /// <summary>
        /// Gets upcoming bookings for a client.
        /// </summary>
        Task<List<Booking>> GetUpcomingBookingsAsync(int clientId, int limit = 5);
    }
}
