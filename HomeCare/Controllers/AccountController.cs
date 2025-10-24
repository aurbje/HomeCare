using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using HomeCare.ViewModels.Account; // der SignInViewModel og SignUpViewModel ligger
using HomeCare.Models; 
using HomeCare.Data;
using System.Threading.Tasks;
using HomeCare.Repositories;

namespace HomeCare.Controllers
{
    public class AccountController : Controller
    {
        private readonly IUserRepository _userRepo;
        private readonly ILogger<UserController> _logger;

        public AccountController(IUserRepository userRepo, ILogger<UserController> logger)
        {
            _userRepo = userRepo;
            _logger = logger;
        }

        [HttpGet]
        public IActionResult SignIn()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> SignIn(SignInViewModel model)
        {
            if (!ModelState.IsValid)
            {
                _logger.LogWarning("Login atempt failed validation for email {Email}", model.Email);
                return View(model);
            }

            try
            {
                var user = await _userRepo.GetByEmailAsync(model.Email);

                if (user == null || !BCrypt.Net.BCrypt.Verify(model.Password, user.PasswordHash))
                {
                    _logger.LogWarning("Invalid login atempt for email {Email}", model.Email);
                    ModelState.AddModelError("", "Ugyldig e-post eller passord");
                    return View(model);
                }

                _logger.LogInformation("User {Email} logged inn successfully", model.Email);

                // if login success, sent to homepage, can change this later
                return RedirectToAction("Dashboard", "User");
            }
            catch (Exception e)
            {
                _logger.LogError(e, "Unexpected error during login for email {Email}", model.Email);
                return View("Error"); 
            }
        }

        [HttpGet]
        public IActionResult SignUp()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> SignUp(SignUpViewModel model)
        {
            if (!ModelState.IsValid)
            {
                var errors = ModelState.Values.SelectMany(v => v.Errors).Select(e => e.ErrorMessage);
                _logger.LogWarning("Signup validation failed for email {Email}. Errors: {Errors}", model.Email, errors);
                return View(model);
            }

            try
            {
                if (await _userRepo.EmailExistsAsync(model.Email))
                {
                    _logger.LogInformation("Signup atempt with already reg email {Email}", model.Email);
                    ModelState.AddModelError("Email", "E-postadressen er allerede registrert");
                    return View(model);
                }

                var user = new User
                {
                    FullName = model.FullName,
                    Email = model.Email,
                    PasswordHash = HashPassword(model.Password),
                    Role = "user",
                    TlfNumber = model.TlfNumber,
                    Address = model.Address
                };

                await _userRepo.AddAsync(user);
                await _userRepo.SaveChangesAsync();

                _logger.LogInformation("New user registered with email {Email}", model.Email);

                return RedirectToAction("SignIn");
            }
            catch(Exception e)
            {
                _logger.LogError(e, "Error occurred while registering new user {Email}", model.Email);
                return View("Error"); // må lage egen error side/popup 
            }
        }

        private string HashPassword(string password)
        {
            return BCrypt.Net.BCrypt.HashPassword(password);
        }
    }
}