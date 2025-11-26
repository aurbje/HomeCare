using HomeCare.Api.Models;
using HomeCare.Api.Repositories.Interfaces;
using System.Security.Claims;

namespace HomeCare.Api.Services;

public class CaregiverService
{
    private readonly ICaregiverRepository _repo;

    public CaregiverService(ICaregiverRepository repo)
    {
        _repo = repo;
    }

    public Task<IEnumerable<ApplicationUser>> GetClientsAsync(ClaimsPrincipal user)
    {
        var caregiverId = user.FindFirstValue(ClaimTypes.NameIdentifier)!;
        return _repo.GetClientsForCaregiverAsync(caregiverId);
    }

    public Task<IEnumerable<Appointment>> GetScheduleAsync(ClaimsPrincipal user)
    {
        var caregiverId = user.FindFirstValue(ClaimTypes.NameIdentifier)!;
        return _repo.GetAppointmentsForCaregiverAsync(caregiverId);
    }
}
