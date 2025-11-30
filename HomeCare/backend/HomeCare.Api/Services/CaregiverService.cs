using HomeCare.Api.Models;
using HomeCare.Api.Repositories.Interfaces;
using System.Security.Claims;

namespace HomeCare.Api.Services
{
    public class CaregiverService
    {
        private readonly ICaregiverRepository _repo;
        private readonly ILogger<CaregiverService> _logger;

        public CaregiverService(ICaregiverRepository repo, ILogger<CaregiverService> logger)
        {
            _repo = repo;
            _logger = logger;
        }

        public async Task<ServiceResponse<IEnumerable<ApplicationUser>>> GetClientsAsync(ClaimsPrincipal user)
        {
            try
            {
                var caregiverId = user.FindFirstValue(ClaimTypes.NameIdentifier);
                if (caregiverId == null)
                    return ServiceResponse<IEnumerable<ApplicationUser>>.Fail("User not authenticated");

                var clients = await _repo.GetClientsForCaregiverAsync(caregiverId);
                _logger.LogInformation("Loaded {Count} clients for caregiver {CaregiverId}", clients.Count(), caregiverId);

                return ServiceResponse<IEnumerable<ApplicationUser>>.Success(clients);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error fetching clients for caregiver");
                return ServiceResponse<IEnumerable<ApplicationUser>>.Fail("Failed to load clients for caregiver");
            }
        }

        public async Task<ServiceResponse<IEnumerable<Appointment>>> GetScheduleAsync(ClaimsPrincipal user)
        {
            try
            {
                var caregiverId = user.FindFirstValue(ClaimTypes.NameIdentifier);
                if (caregiverId == null)
                    return ServiceResponse<IEnumerable<Appointment>>.Fail("User not authenticated");

                var schedule = await _repo.GetAppointmentsForCaregiverAsync(caregiverId);
                _logger.LogInformation("Loaded {Count} appointments for caregiver {CaregiverId}", schedule.Count(), caregiverId);

                return ServiceResponse<IEnumerable<Appointment>>.Success(schedule);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error fetching schedule for caregiver");
                return ServiceResponse<IEnumerable<Appointment>>.Fail("Failed to load caregiver schedule");
            }
        }
    }

    // Same helper class as in BookingService
    public class ServiceResponse<T>
    {
        public bool Success { get; set; }
        public string Message { get; set; } = string.Empty;
        public T? Data { get; set; }

        public static ServiceResponse<T> Success(T data, string message = "") =>
            new() { Success = true, Data = data, Message = message };

        public static ServiceResponse<T> Fail(string message) =>
            new() { Success = false, Message = message };
    }
}
