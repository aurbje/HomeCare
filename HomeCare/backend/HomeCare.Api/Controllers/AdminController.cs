using Microsoft.AspNetCore.Mvc;
using HomeCare.Api.Services; // Use the service
using HomeCare.Api.DTO.Admin;
using Microsoft.AspNetCore.Authorization;

namespace HomeCare.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize(Roles = "Admin")] // Secure the entire controller for Admins
    public class AdminController : ControllerBase
    {
        private readonly AdminService _adminService; // Inject the service
        private readonly ILogger<AdminController> _logger;

        // --- CONSTRUCTOR UPDATED to use AdminService ---
        public AdminController(AdminService adminService, ILogger<AdminController> logger)
        {
            _adminService = adminService;
            _logger = logger;
        }

        // GET: api/admin/users
        [HttpGet("users")]
        public async Task<IActionResult> GetUsers([FromQuery(Name = "q")] string? q)
        {
            // --- Use the service to get users ---
            var response = await _adminService.GetUsersAsync(q);
            if (!response.Success)
            {
                return StatusCode(500, new { message = response.Message });
            }

            // The service returns the full User model, so we shape it for the frontend
            var shaped = response.Data.Select(u => new
            {
                id = u.Id,
                fullName = u.FullName,
                email = u.Email,
                address = u.Address,
                tlfNumber = u.TlfNumber,
                role = u.Role
            });

            return Ok(shaped);
        }

        // DELETE: api/admin/users/5
        [HttpDelete("users/{id:int}")]
        public async Task<IActionResult> DeleteUser(int id)
        {
            // --- Use the service to delete user, which includes safety checks ---
            var response = await _adminService.DeleteUserAsync(id);
            if (!response.Success)
            {
                // The service provides a user-friendly error message
                return BadRequest(new { message = response.Message });
            }
            return Ok(new { message = response.Data });
        }

        // GET: api/admin/caregivers
        [HttpGet("caregivers")]
        public async Task<IActionResult> GetCaregivers([FromQuery(Name = "q")] string? q)
        {
            var response = await _adminService.GetCaregiversAsync(q);
            if (!response.Success)
            {
                return StatusCode(500, new { message = response.Message });
            }
            var shaped = response.Data.Select(u => new
            {
                id = u.Id,
                fullName = u.FullName,
                email = u.Email,
                address = u.Address,
                tlfNumber = u.TlfNumber,
                role = u.Role
            });
            return Ok(shaped);
        }

        // DELETE: api/admin/caregivers/5
        [HttpDelete("caregivers/{id:int}")]
        public async Task<IActionResult> DeleteCaregiver(int id)
        {
            var response = await _adminService.DeleteCaregiverAsync(id);
            if (!response.Success)
            {
                return BadRequest(new { message = response.Message });
            }
            return Ok(new { message = response.Data });
        }

        // GET: api/admin/bookings
        [HttpGet("bookings")]
        public async Task<IActionResult> GetBookings([FromQuery(Name = "q")] string? q)
        {
            var response = await _adminService.GetBookingsAsync(q);
            if (!response.Success)
            {
                return StatusCode(500, new { message = response.Message });
            }
            // You can add shaping logic here if needed, similar to GetUsers
            return Ok(response.Data);
        }

        // DELETE: api/admin/bookings/5
        [HttpDelete("bookings/{id:int}")]
        public async Task<IActionResult> DeleteBooking(int id)
        {
            var response = await _adminService.DeleteBookingAsync(id);
            if (!response.Success)
            {
                return BadRequest(new { message = response.Message });
            }
            return Ok(new { message = response.Data });
        }

        // NOTE: The other endpoints (GetUserById, UpdateUser, etc.) were not in your AdminService.
        // If you need them, they should be added to AdminService first and then called from here.
    }
}