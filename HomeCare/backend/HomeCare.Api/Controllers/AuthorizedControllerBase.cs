using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace HomeCare.Api.Controllers
{

    public abstract class AuthorizedControllerBase : ControllerBase
    {

        protected int GetCurrentUserId()
        {
            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(userIdClaim) || !int.TryParse(userIdClaim, out var userId))
            {
                throw new UnauthorizedAccessException("User not authenticated or invalid user ID.");
            }
            return userId;
        }


        protected bool IsInRole(string role)
        {
            return User.IsInRole(role);
        }


        protected string? GetCurrentUserEmail()
        {
            return User.FindFirst(ClaimTypes.Email)?.Value;
        }


        protected string? GetCurrentUserName()
        {
            return User.FindFirst(ClaimTypes.Name)?.Value;
        }

        protected string? GetCurrentUserRole()
        {
            return User.FindFirst(ClaimTypes.Role)?.Value;
        }
    }
}
