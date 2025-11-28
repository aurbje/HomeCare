using Microsoft.AspNetCore.Mvc;
using HomeCare.ViewModels.Account;
using HomeCare.Models;
using HomeCare.Repositories.Interfaces;
using Microsoft.Extensions.Logging;
using System;
using System.Linq;
using System.Security.Claims;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;

namespace HomeCare.Controllers
{
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

        /// POST: /api/account/signin
        [HttpPost("signin")]
        public async Task<ActionResult<AuthResponseDto>> SignIn([FromBody] SignInViewModel model)
        {
            // just double-checking that the incoming data follows the validation rules
            if (!ModelState.IsValid)
            {
                var errors = ModelState
                    .Where(kvp => kvp.Value?.Errors.Any() == true)
                    .ToDictionary(
                        kvp => kvp.Key,
                        kvp => kvp.Value!.Errors.Select(e => e.ErrorMessage).ToArray()
                    );

                _logger.LogWarning("Login attempt failed validation for email {Email}. Errors: {@Errors}", model.Email, errors);

                // frontend can show validation messages based on this
                return BadRequest(new
                {
                    message = "Validation failed",
                    errors
                });
            }

            try
            {
                // grab user from repo based on email
                var user = await _userRepo.GetByEmailAsync(model.Email);

                // if user doesn't exist or password is wrong, we just deny access
                if (user == null || !BCrypt.Net.BCrypt.Verify(model.Password, user.PasswordHash))
                {
                    _logger.LogWarning("Invalid login attempt for email {Email}", model.Email);
                    return Unauthorized(new
                    {
                        message = "Ugyldig e-post eller passord"
                    });
                }

                var claims = new List<Claim>
                {
                    new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
                    new Claim(ClaimTypes.Name, user.FullName),
                    new Claim(ClaimTypes.Email, user.Email),
                    new Claim(ClaimTypes.Role, user.Role)  // "Admin", "Caregiver", "User" etc.
                };

                var identity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
                var principal = new ClaimsPrincipal(identity);

                await HttpContext.SignInAsync(
                    CookieAuthenticationDefaults.AuthenticationScheme, principal,
                    new AuthenticationProperties
                    {
                        IsPersistent = true,            // remember after browser close (optional)
                        ExpiresUtc = DateTimeOffset.UtcNow.AddHours(8)

                    }
                );

                _logger.LogInformation("User {Email} logged in successfully", model.Email);

                if (user.Role == "Admin")
                {
                    return RedirectToAction("AdminDashboard", "Admin");
                }
                else if (user.Role == "Caregiver")
                {
                    return RedirectToAction("Dashboard", "Caregiver");
                }
                else // default -> regular user
                {
                    return RedirectToAction("Dashboard", "User");
                }
            }
            catch (Exception e)
            {
                // if something unexpected happens, we log it and send a generic error
                _logger.LogError(e, "Unexpected error during login for email {Email}", model.Email);
                return StatusCode(500, new
                {
                    message = "An unexpected error occurred during login."
                });
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Logout()
        {
            await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
            _logger.LogInformation("User logged out");
            return RedirectToAction("Index", "Home");
        }


        [HttpGet]
        public IActionResult SignUp()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> SignUp(SignUpViewModel model)
        {
            // again, just checking if the input matches the validation rules
            if (!ModelState.IsValid)
            {
                var errors = ModelState
                    .Where(kvp => kvp.Value?.Errors.Any() == true)
                    .ToDictionary(
                        kvp => kvp.Key,
                        kvp => kvp.Value!.Errors.Select(e => e.ErrorMessage).ToArray()
                    );

                _logger.LogWarning("Signup validation failed for email {Email}. Errors: {@Errors}", model.Email, errors);

                return BadRequest(new
                {
                    message = "Validation failed",
                    errors
                });
            }

            try
            {
                // we don't want duplicate accounts with the same email
                if (await _userRepo.EmailExistsAsync(model.Email))
                {
                    _logger.LogInformation("Signup attempt with already registered email {Email}", model.Email);
                    return Conflict(new
                    {
                        message = "E-postadressen er allerede registrert"
                    });
                }

                // creating a new user object based on the signup form
                var user = new User
                {
                    FullName = model.FullName,
                    Email = model.Email,
                    PasswordHash = HashPassword(model.Password), // hashing so we don't store plain text
                    Role = "user",
                    TlfNumber = model.TlfNumber,
                    Address = model.Address
                };

                // saving to the database through the repository
                await _userRepo.AddAsync(user);
                await _userRepo.SaveChangesAsync();

                _logger.LogInformation("New user registered with email {Email}", model.Email);

                // returning a simple DTO back to the frontend
                var response = new AuthResponseDto
                {
                    UserId = user.Id,
                    FullName = user.FullName,
                    Email = user.Email,
                    Role = user.Role
                };

                return StatusCode(201, new
                {
                    message = "User created successfully",
                    user = response
                });
            }
            catch (Exception e)
            {
                // catching any unexpected issues during signup
                _logger.LogError(e, "Error occurred while registering new user {Email}", model.Email);
                return StatusCode(500, new
                {
                    message = "An unexpected error occurred during registration."
                });
            }
        }

        // simple helper method for hashing passwords
        private string HashPassword(string password)
        {
            return BCrypt.Net.BCrypt.HashPassword(password);
        }
    }

    public class AuthResponseDto
    {
        // just the basic fields to know about the user
        public int UserId { get; set; }
        public string FullName { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string Role { get; set; } = string.Empty;
    }
}
