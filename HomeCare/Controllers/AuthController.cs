using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using HomeCare.Data;
using HomeCare.Models;
using System.Security.Cryptography;
using System.Text;


namespace HomeCare.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AuthController : ControllerBase
    {
        private readonly AppDbContext _context;

        public AuthController(AppDbContext context)
        {
            _context = context;
        }

        // Sign up
        [HttpPost("signup")]
        public async Task<IActionResult> SignUp(User user)
        {
            if (await _context.Users.AnyAsync(u => u.Email == user.Email))
                return BadRequest("Email already exists");

            user.PasswordHash = HashPassword(user.PasswordHash); // hash password
            _context.Users.Add(user);
            await _context.SaveChangesAsync();
            return Ok("User registered successfully");
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
            using var sha256 = SHA256.Create();
            var bytes = Encoding.UTF8.GetBytes(password);
            var hash = sha256.ComputeHash(bytes);
            return Convert.ToBase64String(hash);
        }
    }
}