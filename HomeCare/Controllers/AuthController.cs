using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using HomeCare.Data;
using HomeCare.Models;
using System.Security.Cryptography;
using System.Text;
using HomeCare.ViewModels.Account;
using BCrypt.Net;
using Microsoft.EntityFrameworkCore.Metadata.Internal;


namespace HomeCare.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AuthController : Controller
    {
        private readonly AppDbContext _context;

        public AuthController(AppDbContext context)
        {
            _context = context;
        }

        [HttpGet("SignUp")]
        public IActionResult SignUp()
        {
            return View();
        }

        // handle form submission for sign up
        [HttpPost("signup")]
        public async Task<IActionResult> SignUp(SignUpViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View();
            }


            if (await _context.Users.AnyAsync(u => u.Email == model.Email))
            {
                return BadRequest("Email already exists");
            }

            var user = new User
            {
                FullName = model.FullName,
                Email = model.Email,
                Username = model.Email,
                PasswordHash = HashPassword(model.Password),
                Role = "user"
            };

            _context.Users.Add(user);
            await _context.SaveChangesAsync();

            return RedirectToAction("SignIn", "Auth");
        }

        // Login
        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] User login)
        {
            var user = await _context.Users.FirstOrDefaultAsync(u => u.Email == login.Email);
            if (user == null || user.PasswordHash != HashPassword(login.PasswordHash))
                return Unauthorized("Invalid email or password");

            return Ok("Login successful");
        }

        private string HashPassword(string password)
        {
            return BCrypt.Net.BCrypt.HashPassword(password);
        }
    }
}