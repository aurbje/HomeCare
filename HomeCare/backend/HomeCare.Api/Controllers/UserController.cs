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
    /// <summary>
    /// Controller for client/user operations.
    /// Merged from: my ClientController + group's _UserController
    /// </summary>
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

        // GET: /api/user/dashboard?year=2025&month=11
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

/* ============================================================
 * ORIGINAL CODE - Commented out for reference
 * Reason: Replaced with merged version above that uses IUserService 
 * instead of direct database access, following the Repository pattern.
 * The old code used AppDbContext directly which violated separation of concerns.
 * ============================================================
 
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using HomeCare.Services.Interfaces;
using HomeCare.Enums;

namespace HomeCare.Controllers
{
    [ApiController]
    [Route("api/user")]
    [Authorize(Roles = UserRoleExtensions.Roles.Client)]
    public class ClientController : AuthorizedControllerBase
    {
        private readonly IClientService _clientService;
        private readonly ILogger<ClientController> _logger;

        public ClientController(IClientService clientService, ILogger<ClientController> logger)
        {
            _clientService = clientService;
            _logger = logger;
        }

        [HttpGet("dashboard")]
        public async Task<IActionResult> GetDashboard([FromQuery] int? year, [FromQuery] int? month)
        {
            _logger.LogInformation("Dashboard requested. User claims: {Claims}",
                string.Join(", ", User.Claims.Select(c => $"{c.Type}={c.Value}")));

            var UserId = GetCurrentUserId();
            _logger.LogInformation("Client ID: {UserId}", UserId);

            var dashboard = await _clientService.GetDashboardAsync(UserId);

            if (dashboard == null)
                return NotFound();

            return Ok(dashboard);
        }
    }
}

*/