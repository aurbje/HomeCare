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
    /// <summary>
    /// Handles user authentication operations including login, logout, and registration.
    /// This controller manages cookie-based authentication for the HomeCare application.
    /// 
    /// Frontend files that use this controller:
    /// - frontend/src/api/authApi.js (API calls)
    /// - frontend/src/context/AuthContext.jsx (authentication state management)
    /// - frontend/src/pages/Account/LoginPage.jsx (login form)
    /// - frontend/src/pages/Account/RegisterPage.jsx (registration form)
    /// </summary>
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

        /// <summary>
        /// Authenticates a user and creates a session cookie.
        /// Used by: frontend/src/api/authApi.js -> loginUser()
        /// Called from: frontend/src/pages/Account/LoginPage.jsx
        /// </summary>
        /// <param name="model">Login credentials (email and password)</param>
        /// <returns>User data on success, error message on failure</returns>
        // POST: /api/account/signin
        [HttpPost("signin")]
        public async Task<ActionResult<AuthResponseDto>> SignIn([FromBody] LoginDto model)
        {
            if (!ModelState.IsValid)
            {
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
                var user = await _userRepo.GetByEmailAsync(model.Email);

                if (user == null || !BCrypt.Net.BCrypt.Verify(model.Password, user.PasswordHash))
                {
                    _logger.LogWarning("Invalid login attempt for email {Email}", model.Email);
                    return Unauthorized(new { message = "Invalid email or password" });
                }

                var claims = new List<Claim>
                {
                    new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
                    new Claim(ClaimTypes.Name, user.FullName),
                    new Claim(ClaimTypes.Email, user.Email),
                    new Claim(ClaimTypes.Role, user.Role)
                };

                var identity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
                var principal = new ClaimsPrincipal(identity);

                await HttpContext.SignInAsync(
                    CookieAuthenticationDefaults.AuthenticationScheme,
                    principal,
                    new AuthenticationProperties
                    {
                        IsPersistent = true,
                        ExpiresUtc = DateTimeOffset.UtcNow.AddHours(8)
                    });

                _logger.LogInformation("User {Email} logged in successfully", model.Email);

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
                _logger.LogError(e, "Unexpected error during login for email {Email}", model.Email);
                // DEBUG: Return detailed error for troubleshooting
                return StatusCode(500, new { message = "Unexpected error during login.", error = e.Message, stackTrace = e.StackTrace });
            }
        }

        [HttpGet("me")]
        public IActionResult GetCurrentUser()
        {
            if (!User.Identity?.IsAuthenticated ?? false)
                return Unauthorized(new { message = "Not logged in" });

            return Ok(new {
                id = User.FindFirst(ClaimTypes.NameIdentifier)?.Value,
                fullName = User.FindFirst(ClaimTypes.Name)?.Value,
                email = User.FindFirst(ClaimTypes.Email)?.Value,
                role = User.FindFirst(ClaimTypes.Role)?.Value
            });
        }

        
        
        /// <summary>
        /// Returns the currently authenticated user's information from claims.
        /// This endpoint is used by AuthContext to check if a user is logged in on app load.
        /// Used by: frontend/src/api/authApi.js -> getCurrentUser()
        /// Called from: frontend/src/context/AuthContext.jsx (on app initialization)
        /// </summary>
        /// <returns>User info (id, fullName, email, role) or 401 if not logged in</returns>
        // GET: /api/account/me
        // Added from group's Final_Alexander branch for AuthContext integration
        [HttpGet("me")]
        public IActionResult GetCurrentUser()
        {
            if (!(User.Identity?.IsAuthenticated ?? false))
            {
                return Unauthorized(new { message = "Not logged in" });
            }

            return Ok(new
            {
                id = User.FindFirst(ClaimTypes.NameIdentifier)?.Value,
                fullName = User.FindFirst(ClaimTypes.Name)?.Value,
                email = User.FindFirst(ClaimTypes.Email)?.Value,
                role = User.FindFirst(ClaimTypes.Role)?.Value
            });
        }

        /// <summary>
        /// Signs out the current user by removing the authentication cookie.
        /// Used by: frontend/src/api/authApi.js -> logoutUser()
        /// Called from: frontend/src/context/AuthContext.jsx -> logoutUser()
        /// </summary>
        /// <returns>Success message</returns>
        // POST: /api/account/logout
        [HttpPost("logout")]
        public async Task<IActionResult> Logout()
        {
            await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
            _logger.LogInformation("User logged out successfully");
            return Ok(new { message = "User logged out successfully" });
        }

        /// <summary>
        /// Registers a new user account with the provided information.
        /// Creates a new User with hashed password and default "User" role.
        /// Used by: frontend/src/api/authApi.js -> registerUser()
        /// Called from: frontend/src/pages/Account/RegisterPage.jsx
        /// </summary>
        /// <param name="model">Registration data (name, email, password, phone, address)</param>
        /// <returns>Created user data on success, error message on failure</returns>
        // POST: /api/account/signup
        [HttpPost("signup")]
        public async Task<IActionResult> SignUp([FromBody] RegisterDto model)
        {
            if (!ModelState.IsValid)
            {
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
                if (await _userRepo.EmailExistsAsync(model.Email))
                {
                    return Conflict(new { message = "Email already registered" });
                }

                var user = new User
                {
                    FullName = model.FullName,
                    UserName = model.Email, // Use email as username
                    Email = model.Email,
                    PasswordHash = BCrypt.Net.BCrypt.HashPassword(model.Password),
                    Role = "User",
                    TlfNumber = model.TlfNumber,
                    Address = model.Address
                };

                await _userRepo.AddAsync(user);
                await _userRepo.SaveChangesAsync();

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
                _logger.LogError(e, "Error during user registration for {Email}", model.Email);
                // Return detailed error for debugging
                return StatusCode(500, new { message = "Unexpected error during registration.", error = e.Message, innerError = e.InnerException?.Message });
            }
        }
    }

    /// <summary>
    /// Data transfer object for authentication responses.
    /// Contains basic user information returned after login/registration.
    /// Used in: AccountController.SignIn(), AccountController.SignUp()
    /// </summary>
    public class AuthResponseDto
    {
        public int UserId { get; set; }
        public string FullName { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string Role { get; set; } = string.Empty;
    }
}
