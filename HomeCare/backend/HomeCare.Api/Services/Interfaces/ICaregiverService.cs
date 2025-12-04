using HomeCare.Api.DTO;
using HomeCare.Api.Models;

namespace HomeCare.Api.Services.Interfaces
{
    public interface ICaregiverService
    {
        /// Gets dashboard data including today's visits and available dates.
        Task<CaregiverDashboardDto> GetDashboardAsync(int caregiverId, int? year, int? month);


        Task RegisterAvailabilityAsync(int caregiverId, DateTime date);

        Task RegisterMultipleAvailabilityAsync(int caregiverId, List<DateTime> dates);


        Task DeleteAvailabilityAsync(int caregiverId, DateTime date);

        /// Requests deletion of availability (admin workflow).
        Task<bool> RequestAvailabilityDeletionAsync(int caregiverId, DateTime date);

        Task<List<Booking>> GetTodayVisitsAsync(int caregiverId);


        Task<List<Booking>> GetUpcomingBookingsAsync(int caregiverId);
    }
}
