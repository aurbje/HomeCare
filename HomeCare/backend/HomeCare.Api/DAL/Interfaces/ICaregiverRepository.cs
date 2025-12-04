using HomeCare.Api.DTO;
using HomeCare.Api.Models;

namespace HomeCare.Api.DAL.Interfaces
{
    // interface for caregiver repository
    public interface ICaregiverRepository
    {
        Task<CaregiverDashboardDto> GetDashboardAsync(int CaregiverId); // get caregiver dashboard data
        Task<User?> GetCaregiverByIdAsync(int CaregiverId); //  get caregiver by ID
        Task AddAvailabilityAsync(int CaregiverId, DateTime date); // add availability for caregiver
        Task DeleteAvailabilityAsync(int CaregiverId, DateTime date);
        Task<bool> HasBookingOnDateAsync(int CaregiverId, DateTime date); // check if caregiver has bookings on a specific date
        Task<bool> TryDeleteAvailabilityWithCheckAsync(int CaregiverId, DateTime date); // try to delete availability after checking for bookings
        Task<List<Booking>> GetBookingsForCaregiverAsync(int CaregiverId);
        Task AddAdminNotificationAsync(string message); // add admin notification
    }
}
