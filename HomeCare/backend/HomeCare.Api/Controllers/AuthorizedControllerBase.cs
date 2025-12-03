using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace HomeCare.Api.Controllers
{
    /// <summary>
    /// Base controller providing authentication helper methods for controllers
    /// that require user authentication. Inherit from this to access user info.
    /// 
    /// Used by:
    /// - BookingController (for client bookings)
    /// - UserController (for user dashboard)
    /// - CaregiverController (for caregiver dashboard)
    /// - AdminController (for admin operations)
    /// 
    /// Authentication is provided by cookie-based auth configured in Program.cs
    /// Frontend auth state is managed by: frontend/src/context/AuthContext.jsx
    /// </summary>
    public abstract class AuthorizedControllerBase : ControllerBase
    {
        /// <summary>
        /// Gets the current user's ID from JWT/Cookie claims.
        /// The ID is stored as ClaimTypes.NameIdentifier during login.
        /// See: AccountController.SignIn() for claim creation
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
        /// Checks if current user has the specified role.
        /// Roles are stored as ClaimTypes.Role during login.
        /// See: AccountController.SignIn() for claim creation
        /// </summary>
        /// <param name="role">Role to check (e.g., "Admin", "Caregiver", "User")</param>
        /// <returns>True if user has the role</returns>
        protected bool IsInRole(string role)
        {
            return User.IsInRole(role);
        }

        /// <summary>
        /// Gets the current user's email from claims.
        /// See: AccountController.SignIn() for claim creation
        /// </summary>
        /// <returns>Email address or null if not found</returns>
        protected string? GetCurrentUserEmail()
        {
            return User.FindFirst(ClaimTypes.Email)?.Value;
        }

        /// <summary>
        /// Gets the current user's full name from claims.
        /// See: AccountController.SignIn() for claim creation
        /// </summary>
        /// <returns>Full name or null if not found</returns>
        protected string? GetCurrentUserName()
        {
            return User.FindFirst(ClaimTypes.Name)?.Value;
        }

        /// <summary>
        /// Gets the current user's role from claims.
        /// Possible values: "Admin", "Caregiver", "User"
        /// See: AccountController.SignIn() for claim creation
        /// </summary>
        /// <returns>Role string or null if not found</returns>
        protected string? GetCurrentUserRole()
        {
            return User.FindFirst(ClaimTypes.Role)?.Value;
        }
    }
}
