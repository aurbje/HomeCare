/* ============================================================
 * ORIGINAL GROUP CODE - Commented out
 * Replaced with merged version in UserService.cs
 * This file kept for reference purposes
 * ============================================================

using HomeCare.Api.Models;
using HomeCare.Api.DAL.Interfaces;
using HomeCare.Api.DTO.Shared;

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

        // ------------------------------
        // GET USER BY EMAIL
        // ------------------------------
        public async Task<ServiceResponse<User?>> GetByEmailAsync(string email)
        {
            try
            {
                var user = await _repo.GetByEmailAsync(email);

                if (user == null)
                {
                    _logger.LogWarning("No user found with email {Email}", email);
                    return ServiceResponse<User?>.FailResponse("User not found");
                }

                _logger.LogInformation("User {Email} retrieved successfully", email);
                return ServiceResponse<User?>.SuccessResponse(user);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving user by email {Email}", email);
                return ServiceResponse<User?>.FailResponse("Error retrieving user");
            }
        }

        // ------------------------------
        // GET USER BY ID
        // ------------------------------
        public async Task<ServiceResponse<User?>> GetByIdAsync(int id)
        {
            try
            {
                var user = await _repo.GetUserByIdAsync(id);

                if (user == null)
                {
                    _logger.LogWarning("No user found with ID {Id}", id);
                    return ServiceResponse<User?>.FailResponse("User not found");
                }

                _logger.LogInformation("User {Id} retrieved successfully", id);
                return ServiceResponse<User?>.SuccessResponse(user);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving user by ID {Id}", id);
                return ServiceResponse<User?>.FailResponse("Error retrieving user");
            }
        }
    }
}

*/