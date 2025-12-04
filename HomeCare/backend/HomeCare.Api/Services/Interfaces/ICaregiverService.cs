using HomeCare.Api.DTO.User;
using HomeCare.Api.Models;

namespace HomeCare.Api.Services.Interfaces
{
    // Service interface for caregiver operations.
    public interface ICaregiverService
    {
        // Gets dashboard data for a caregiver including today's visits and available dates.
        Task<CaregiverDashboardDto> GetDashboardAsync(int caregiverId, int? year, int? month);

        
        // Registers an available day for a caregiver.
        Task RegisterAvailabilityAsync(int caregiverId, DateTime date);

        // Registers multiple available days for a caregiver.
        Task RegisterMultipleAvailabilityAsync(int caregiverId, List<DateTime> dates);

        // Deletes an available day for a caregiver.
        
        Task DeleteAvailabilityAsync(int caregiverId, DateTime date);

        // Requests deletion of availability (admin workflow).
        Task<bool> RequestAvailabilityDeletionAsync(int caregiverId, DateTime date);

        // Gets today's visits for a caregiver.
        
        Task<List<Booking>> GetTodayVisitsAsync(int caregiverId);

        // Gets upcoming bookings for a caregiver.
        
        Task<List<Booking>> GetUpcomingBookingsAsync(int caregiverId);
    }
}
