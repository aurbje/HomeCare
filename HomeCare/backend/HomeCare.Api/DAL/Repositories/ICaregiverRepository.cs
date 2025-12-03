/* ============================================================
 * DUPLICATE FILE - Commented out to avoid compilation conflicts
 * Reason: This file is a duplicate of DAL/Interfaces/ICaregiverRepository.cs
 * The correct interface is defined in the Interfaces folder with the proper
 * namespace HomeCare.Api.DAL.Interfaces. This file was incorrectly placed
 * in the Repositories folder and uses old namespaces.
 * ============================================================

using HomeCare.DTOs;
using HomeCare.Models;

namespace HomeCare.DAL.Interfaces
{
    public interface ICaregiverRepository
    {
        Task<CaregiverDashboardDto> GetDashboardAsync(int CaregiverId);
        Task<User?> GetCaregiverByIdAsync(int CaregiverId);
        Task AddAvailabilityAsync(int CaregiverId, DateTime date);
        Task DeleteAvailabilityAsync(int CaregiverId, DateTime date);
        Task<bool> HasBookingOnDateAsync(int CaregiverId, DateTime date);
        Task<bool> TryDeleteAvailabilityWithCheckAsync(int CaregiverId, DateTime date);
        Task<List<Booking>> GetBookingsForCaregiverAsync(int CaregiverId);
        Task AddAdminNotificationAsync(string message);
    }
}

*/
