using Microsoft.AspNetCore.Mvc;
using HomeCare.Api.Models;
using HomeCare.Api.DAL.Interfaces;
using HomeCare.Api.DTO.Account;
using Microsoft.Extensions.Logging;
using System.Linq;
using System.Security.Claims;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;

namespace HomeCare.Api.Controllers
{
    // Controller for handling user authentication operations
    [ApiController]
    [Route("api/[controller]")]
    public class AccountController : ControllerBase
    {
        private readonly IUserRepository _userRepo;
        private readonly ILogger<AccountController> _logger;

        public AccountController(IUserRepository userRepo, ILogger<AccountController> logger)
        {
            _userRepo = userRepo;
            _logger = logger;
        }

        // Validates user credentials and issues an authentication cookie
        [HttpPost("signin")]
        public async Task<ActionResult<AuthResponseDto>> SignIn([FromBody] LoginDto model)
        {
            if (!ModelState.IsValid)
            {
                // Returns validation errors for incorrect input
                var errors = ModelState
                    .Where(kvp => kvp.Value?.Errors.Any() == true)
                    .ToDictionary(
                        kvp => kvp.Key,
                        kvp => kvp.Value!.Errors.Select(e => e.ErrorMessage).ToArray()
                    );

                return BadRequest(new { message = "Validation failed", errors });
            }

            try
            {
                // Attempts to retrieve user by email
                var user = await _userRepo.GetByEmailAsync(model.Email);

                // Verifies email and password
                if (user == null || !BCrypt.Net.BCrypt.Verify(model.Password, user.PasswordHash))
                {
                    _logger.LogWarning("Invalid login attempt for email {Email}", model.Email);
                    return Unauthorized(new { message = "Invalid email or password" });
                }

                // Creates user identity with essential claims
                var claims = new List<Claim>
                {
                    new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
                    new Claim(ClaimTypes.Name, user.FullName),
                    new Claim(ClaimTypes.Email, user.Email),
                    new Claim(ClaimTypes.Role, user.Role)
                };

                var identity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
                var principal = new ClaimsPrincipal(identity);

                // Signs the user in with a persistent authentication cookie
                await HttpContext.SignInAsync(
                    CookieAuthenticationDefaults.AuthenticationScheme,
                    principal,
                    new AuthenticationProperties
                    {
                        IsPersistent = true,
                        ExpiresUtc = DateTimeOffset.UtcNow.AddHours(8)
                    });

                _logger.LogInformation("User {Email} logged in successfully", model.Email);

                // Builds response containing essential user details
                var response = new AuthResponseDto
                {
                    UserId = user.Id,
                    FullName = user.FullName,
                    Email = user.Email,
                    Role = user.Role
                };

                return Ok(new { message = "Login successful", user = response });
            }
            catch (Exception e)
            {
                // Logs unexpected errors during login
                _logger.LogError(e, "Unexpected error during login for email {Email}", model.Email);
                return StatusCode(500, new { message = "Unexpected error during login.", error = e.Message, stackTrace = e.StackTrace });
            }
        }

        // Retrieves the currently authenticated user's information
        [HttpGet("me")]
        public IActionResult GetCurrentUser()
        {
            if (!User.Identity?.IsAuthenticated ?? false)
                return Unauthorized(new { message = "Not logged in" });

            // Returns basic user identity claims
            return Ok(new {
                id = User.FindFirst(ClaimTypes.NameIdentifier)?.Value,
                fullName = User.FindFirst(ClaimTypes.Name)?.Value,
                email = User.FindFirst(ClaimTypes.Email)?.Value,
                role = User.FindFirst(ClaimTypes.Role)?.Value
            });
        }

        // Signs out the authenticated user by clearing authentication cookies
        [HttpPost("logout")]
        public async Task<IActionResult> Logout()
        {
            await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
            _logger.LogInformation("User logged out successfully");
            return Ok(new { message = "User logged out successfully" });
        }

        // Registers a new user account with hashed password and default role
        [HttpPost("signup")]
        public async Task<IActionResult> SignUp([FromBody] RegisterDto model)
        {
            if (!ModelState.IsValid)
            {
                // Returns validation error details
                var errors = ModelState
                    .Where(kvp => kvp.Value?.Errors.Any() == true)
                    .ToDictionary(
                        kvp => kvp.Key,
                        kvp => kvp.Value!.Errors.Select(e => e.ErrorMessage).ToArray()
                    );

                return BadRequest(new { message = "Validation failed", errors });
            }

            try
            {
                // Checks if email is already registered
                if (await _userRepo.EmailExistsAsync(model.Email))
                {
                    return Conflict(new { message = "Email already registered" });
                }

                // Creates new user entity with secure password hashing
                var user = new User
                {
                    FullName = model.FullName,
                    UserName = model.Email,
                    Email = model.Email,
                    PasswordHash = BCrypt.Net.BCrypt.HashPassword(model.Password),
                    Role = "User",
                    TlfNumber = model.TlfNumber,
                    Address = model.Address
                };

                // Saves the new user record
                await _userRepo.AddAsync(user);
                await _userRepo.SaveChangesAsync();

                // Builds response object with user details
                var response = new AuthResponseDto
                {
                    UserId = user.Id,
                    FullName = user.FullName,
                    Email = user.Email,
                    Role = user.Role
                };

                _logger.LogInformation("New user registered: {Email}", model.Email);

                return StatusCode(201, new { message = "User created successfully", user = response });
            }
            catch (Exception e)
            {
                // Logs unexpected errors during registration
                _logger.LogError(e, "Error during user registration for {Email}", model.Email);
                return StatusCode(500, new { message = "Unexpected error during registration.", error = e.Message, innerError = e.InnerException?.Message });
            }
        }
    }

    // Data structure returned after authentication events
    public class AuthResponseDto
    {
        public int UserId { get; set; }
        public string FullName { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string Role { get; set; } = string.Empty;
    }
}
