using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using Microsoft.EntityFrameworkCore;
using HomeCare.Api.Data;
using HomeCare.Api.Models;
using HomeCare.Api.DAL.Interfaces;
using HomeCare.Api.Services.Interfaces;
using HomeCare.Api.DTO;
using HomeCare.Api.Enums;
using System.Security.Claims;

namespace HomeCare.Api.Controllers
{
    // controller for client/user operations
    [ApiController]
    [Route("api/[controller]")]
    [Authorize(Roles = UserRoleExtensions.Roles.User)]
    public class UserController : AuthorizedControllerBase
    {
        private readonly IUserService _userService;
        private readonly ILogger<UserController> _logger;

        public UserController(IUserService userService, ILogger<UserController> logger)
        {
            _userService = userService;
            _logger = logger;
        }

        // GET: /api/user/dashboard
        [HttpGet("dashboard")]
        public async Task<IActionResult> Dashboard([FromQuery] int? year, [FromQuery] int? month)
        {
            _logger.LogInformation("Dashboard requested. User claims: {Claims}",
                string.Join(", ", User.Claims.Select(c => $"{c.Type}={c.Value}")));

            var userId = GetCurrentUserId();
            _logger.LogInformation("User ID: {UserId}", userId);

            var dashboard = await _userService.GetDashboardAsync(userId);

            if (dashboard == null)
                return NotFound();

            return Ok(dashboard);
        }
    }
}