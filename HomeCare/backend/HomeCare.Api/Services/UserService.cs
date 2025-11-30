using HomeCare.Api.Models;
using HomeCare.Api.Repositories.Interfaces;

namespace HomeCare.Api.Services
{
    public class UserService
    {
        private readonly IUserRepository _repo;
        private readonly ILogger<UserService> _logger;

        public UserService(IUserRepository repo, ILogger<UserService> logger)
        {
            _repo = repo;
            _logger = logger;
        }

        public async Task<ServiceResponse<ApplicationUser?>> GetByEmailAsync(string email)
        {
            try
            {
                var user = await _repo.GetByEmailAsync(email);

                if (user == null)
                {
                    _logger.LogWarning("No user found with email {Email}", email);
                    return ServiceResponse<ApplicationUser?>.Fail("User not found");
                }

                _logger.LogInformation("User {Email} retrieved successfully", email);
                return ServiceResponse<ApplicationUser?>.Success(user);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving user by email {Email}", email);
                return ServiceResponse<ApplicationUser?>.Fail("Error retrieving user");
            }
        }

        public async Task<ServiceResponse<ApplicationUser?>> GetByIdAsync(string id)
        {
            try
            {
                var user = await _repo.GetByIdAsync(id);

                if (user == null)
                {
                    _logger.LogWarning("No user found with ID {Id}", id);
                    return ServiceResponse<ApplicationUser?>.Fail("User not found");
                }

                _logger.LogInformation("User {Id} retrieved successfully", id);
                return ServiceResponse<ApplicationUser?>.Success(user);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving user by ID {Id}", id);
                return ServiceResponse<ApplicationUser?>.Fail("Error retrieving user");
            }
        }
    }

    // Reuse the same ServiceResponse<T> helper from BookingService & CaregiverService
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
