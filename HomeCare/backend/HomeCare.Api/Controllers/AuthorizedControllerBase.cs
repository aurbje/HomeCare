using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace HomeCare.Api.Controllers
{
    /// <summary>
    /// Base controller providing authentication helper methods.
    /// All controllers requiring user authentication should inherit from this.
    /// </summary>
    public abstract class AuthorizedControllerBase : ControllerBase
    {
        /// <summary>
        /// Gets the current user's ID from claims.
        /// </summary>
        /// <returns>User ID as integer</returns>
        /// <exception cref="UnauthorizedAccessException">If user is not authenticated</exception>
        protected int GetCurrentUserId()
        {
            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(userIdClaim) || !int.TryParse(userIdClaim, out var userId))
            {
                throw new UnauthorizedAccessException("User not authenticated or invalid user ID.");
            }
            return userId;
        }

        /// <summary>
        /// Checks if current user is in specified role.
        /// </summary>
        /// <param name="role">Role to check</param>
        /// <returns>True if user is in role</returns>
        protected bool IsInRole(string role)
        {
            return User.IsInRole(role);
        }

        /// <summary>
        /// Gets the current user's email from claims.
        /// </summary>
        protected string? GetCurrentUserEmail()
        {
            return User.FindFirst(ClaimTypes.Email)?.Value;
        }

        /// <summary>
        /// Gets the current user's name from claims.
        /// </summary>
        protected string? GetCurrentUserName()
        {
            return User.FindFirst(ClaimTypes.Name)?.Value;
        }

        /// <summary>
        /// Gets the current user's role from claims.
        /// </summary>
        protected string? GetCurrentUserRole()
        {
            return User.FindFirst(ClaimTypes.Role)?.Value;
        }
    }
}
