using HomeCare.Api.DTO.User;
using HomeCare.Api.Models;
using System.Collections.Generic;
using System.Threading.Tasks;
using System;

namespace HomeCare.Api.DAL.Interfaces
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
