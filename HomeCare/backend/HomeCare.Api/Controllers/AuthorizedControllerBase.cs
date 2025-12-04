using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace HomeCare.Api.Controllers
{
    // base controller for authorized endpoints
    public abstract class AuthorizedControllerBase : ControllerBase
    {
        // gets the current users ID from claims
        protected int GetCurrentUserId()
        {
            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(userIdClaim) || !int.TryParse(userIdClaim, out var userId))
            {
                throw new UnauthorizedAccessException("User not authenticated or invalid user ID.");
            }
            return userId;
        }

        // checks if the current user is in the specified role
        protected bool IsInRole(string role)
        {
            return User.IsInRole(role);
        }

        // gets the current users email from claims
        protected string? GetCurrentUserEmail()
        {
            return User.FindFirst(ClaimTypes.Email)?.Value;
        }

        // gets the current users names
        protected string? GetCurrentUserName()
        {
            return User.FindFirst(ClaimTypes.Name)?.Value;
        }

        // gets the current users role 
        protected string? GetCurrentUserRole()
        {
            return User.FindFirst(ClaimTypes.Role)?.Value;
        }
    }
}