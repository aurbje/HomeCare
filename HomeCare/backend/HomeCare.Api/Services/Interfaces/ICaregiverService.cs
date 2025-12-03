using HomeCare.Api.DTO.User;
using HomeCare.Api.Models;

namespace HomeCare.Api.Services.Interfaces
{
    /// <summary>
    /// Service interface for caregiver operations.
    /// </summary>
    public interface ICaregiverService
    {
        /// <summary>
        /// Gets dashboard data for a caregiver including today's visits and available dates.
        /// </summary>
        Task<CaregiverDashboardDto> GetDashboardAsync(int caregiverId, int? year, int? month);

        /// <summary>
        /// Registers an available day for a caregiver.
        /// </summary>
        Task RegisterAvailabilityAsync(int caregiverId, DateTime date);

        /// <summary>
        /// Registers multiple available days for a caregiver.
        /// </summary>
        Task RegisterMultipleAvailabilityAsync(int caregiverId, List<DateTime> dates);

        /// <summary>
        /// Deletes an available day for a caregiver.
        /// </summary>
        Task DeleteAvailabilityAsync(int caregiverId, DateTime date);

        /// <summary>
        /// Requests deletion of availability (admin workflow).
        /// </summary>
        Task<bool> RequestAvailabilityDeletionAsync(int caregiverId, DateTime date);

        /// <summary>
        /// Gets today's visits for a caregiver.
        /// </summary>
        Task<List<Booking>> GetTodayVisitsAsync(int caregiverId);

        /// <summary>
        /// Gets upcoming bookings for a caregiver.
        /// </summary>
        Task<List<Booking>> GetUpcomingBookingsAsync(int caregiverId);
    }
}
