using HomeCare.Api.Models;

namespace HomeCare.Api.Repositories.Interfaces;

public interface ICaregiverRepository
{
    Task<IEnumerable<ApplicationUser>> GetClientsForCaregiverAsync(string caregiverId);
    Task<IEnumerable<Appointment>> GetAppointmentsForCaregiverAsync(string caregiverId);
}
