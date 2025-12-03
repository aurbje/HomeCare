using Microsoft.AspNetCore.Mvc;
using HomeCare.Api.Services;
using HomeCare.Api.DTO.Admin;
using Microsoft.AspNetCore.Authorization;

namespace HomeCare.Api.Controllers
{
    // Controller for admin-only management operations
    [ApiController]
    [Route("api/[controller]")]
    [Authorize(Roles = "Admin")]
    public class AdminController : ControllerBase
    {
        private readonly AdminService _adminService;
        private readonly ILogger<AdminController> _logger;

        public AdminController(AdminService adminService, ILogger<AdminController> logger)
        {
            _adminService = adminService;
            _logger = logger;
        }

        // Retrieves all users with optional search filtering
        [HttpGet("users")]
        public async Task<IActionResult> GetUsers([FromQuery(Name = "q")] string? q)
        {
            var response = await _adminService.GetUsersAsync(q);
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

        // Retrieves a single user by ID
        [HttpGet("users/{id:int}")]
        public async Task<IActionResult> GetUserById(int id)
        {
            var response = await _adminService.GetUserByIdAsync(id);
            if (!response.Success)
            {
                return NotFound(new { message = response.Message });
            }

            var user = response.Data;

            // Returns user in a uniform response structure
            var shaped = new
            {
                id = user.Id,
                fullName = user.FullName,
                email = user.Email,
                address = user.Address,
                tlfNumber = user.TlfNumber,
                role = user.Role
            };

            return Ok(shaped);
        }

        // Updates an existing user record
        [HttpPut("users/{id:int}")]
        public async Task<IActionResult> UpdateUser(int id, [FromBody] UpdateUserDto userDto)
        {
            var response = await _adminService.UpdateUserAsync(id, userDto);
            if (!response.Success)
            {
                return BadRequest(new { message = response.Message });
            }

            return Ok(new { message = response.Data });
        }

        // Deletes a user account by ID
        [HttpDelete("users/{id:int}")]
        public async Task<IActionResult> DeleteUser(int id)
        {
            var response = await _adminService.DeleteUserAsync(id);
            if (!response.Success)
            {
                return BadRequest(new { message = response.Message });
            }

            return Ok(new { message = response.Data });
        }

        // Retrieves all caregivers with optional query filtering
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

        // Retrieves a single caregiver by ID
        [HttpGet("caregivers/{id:int}")]
        public async Task<IActionResult> GetCaregiverById(int id)
        {
            _logger.LogInformation("GetCaregiverById called with id: {Id}", id);
            _logger.LogInformation("User authenticated: {IsAuth}", User.Identity?.IsAuthenticated);
            _logger.LogInformation("User role: {Role}", User.FindFirst(System.Security.Claims.ClaimTypes.Role)?.Value);

            var response = await _adminService.GetCaregiverByIdAsync(id);

            _logger.LogInformation("Service response success: {Success}, message: {Message}", response.Success, response.Message);

            if (!response.Success)
            {
                return NotFound(new { message = response.Message });
            }

            var user = response.Data;
            
            var shaped = new
            {
                id = user.Id,
                fullName = user.FullName,
                email = user.Email,
                address = user.Address,
                tlfNumber = user.TlfNumber,
                role = user.Role
            };

            return Ok(shaped);
        }

        // Updates caregiver details by ID
        [HttpPut("caregivers/{id:int}")]
        public async Task<IActionResult> UpdateCaregiver(int id, [FromBody] UpdateCaregiverDto caregiverDto)
        {
            var response = await _adminService.UpdateCaregiverAsync(id, caregiverDto);
            if (!response.Success)
            {
                return BadRequest(new { message = response.Message });
            }

            return Ok(new { message = response.Data });
        }

        // Removes a caregiver from the system
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

        // Retrieves all bookings with optional filtering
        [HttpGet("bookings")]
        public async Task<IActionResult> GetBookings([FromQuery(Name = "q")] string? q)
        {
            var response = await _adminService.GetBookingsAsync(q);
            if (!response.Success)
            {
                return StatusCode(500, new { message = response.Message });
            }

            return Ok(response.Data);
        }

        // Retrieves detailed booking information by ID
        [HttpGet("bookings/{id:int}")]
        public async Task<IActionResult> GetBookingById(int id)
        {
            var response = await _adminService.GetBookingByIdAsync(id);
            if (!response.Success)
            {
                return NotFound(new { message = response.Message });
            }

            return Ok(response.Data);
        }

        // Updates booking information
        [HttpPut("bookings/{id:int}")]
        public async Task<IActionResult> UpdateBooking(int id, [FromBody] UpdateBookingDto bookingDto)
        {
            var response = await _adminService.UpdateBookingAsync(id, bookingDto);
            if (!response.Success)
            {
                return BadRequest(new { message = response.Message });
            }

            return Ok(new { message = response.Data });
        }

        // Deletes a booking from the system
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
    }
}
