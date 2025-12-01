using HomeCare.Api.Models;

namespace HomeCare.Api.DAL.Interfaces
{
    // Handles all data access for caregiver-related functionality
    public interface ICaregiverRepository
    {
        // Gets all clients assigned to a specific caregiver
        Task<IEnumerable<User>> GetClientsForCaregiverAsync(string caregiverId);

        // Gets all bookings scheduled for a specific caregiver
        Task<IEnumerable<Booking>> GetBookingsForCaregiverAsync(string caregiverId);
    }
}
