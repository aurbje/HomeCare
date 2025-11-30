using HomeCare.Api.Models;

namespace HomeCare.Api.DAL.Interfaces
{
    // Handles all data access for caregiver-related functionality
    public interface ICaregiverRepository
    {
        // Gets all clients assigned to a specific caregiver
        Task<IEnumerable<ApplicationUser>> GetClientsForCaregiverAsync(string caregiverId);

        // Gets all appointments scheduled for a specific caregiver
        Task<IEnumerable<Appointment>> GetAppointmentsForCaregiverAsync(string caregiverId);
    }
}
