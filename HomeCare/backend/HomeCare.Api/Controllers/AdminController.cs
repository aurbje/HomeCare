using Microsoft.AspNetCore.Mvc;
using HomeCare.Api.Services;
using HomeCare.Api.DTO.Admin;
using Microsoft.AspNetCore.Authorization;

namespace HomeCare.Api.Controllers
{
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

        // GET: api/admin/users
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

        // GET: api/admin/users/5
        [HttpGet("users/{id:int}")]
        public async Task<IActionResult> GetUserById(int id)
        {
            var response = await _adminService.GetUserByIdAsync(id);
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

        // PUT: api/admin/users/5
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

        // DELETE: api/admin/users/5
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

        // GET: api/admin/caregivers/5
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

        // PUT: api/admin/caregivers/5
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
            return Ok(response.Data);
        }
        // GET: api/admin/bookings/5
        [HttpGet("bookings/{id:int}")]
        public async Task<IActionResult> GetBookingById(int id)
        {
            var response = await _adminService.GetBookingByIdAsync(id);
            if (!response.Success)
            {
                return NotFound(new { message = response.Message });
            }
            // Return the full booking object with related data
            return Ok(response.Data);
        }

        // PUT: api/admin/bookings/5
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
    }
}