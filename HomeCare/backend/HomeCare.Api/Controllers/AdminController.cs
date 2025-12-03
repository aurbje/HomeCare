using Microsoft.AspNetCore.Mvc;
using HomeCare.Api.DAL.Interfaces;
using HomeCare.Api.Data;
using HomeCare.Api.Models;
using HomeCare.Api.DTO.Admin;
using Microsoft.EntityFrameworkCore;

namespace HomeCare.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AdminController : ControllerBase
    {
        private readonly IAdminRepository _adminRepo;
        private readonly AppDbContext _context;
        private readonly ILogger<AdminController> _logger;

        public AdminController(IAdminRepository adminRepo, AppDbContext context, ILogger<AdminController> logger)
        {
            _adminRepo = adminRepo;
            _context = context;
            _logger = logger;
        }

        // Users
        [HttpGet("users")]
        public async Task<IActionResult> GetUsers([FromQuery(Name = "q")] string? q)
        {
            var users = await _adminRepo.GetUsersAsync(q);
            var shaped = users.Select(u => new
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

        [HttpGet("users/{id:int}")]
        public async Task<IActionResult> GetUserById(int id)
        {
            var user = await _adminRepo.GetUserByIdAsync(id);
            if (user == null) return NotFound();
            return Ok(new
            {
                id = user.Id,
                fullName = user.FullName,
                email = user.Email,
                address = user.Address,
                tlfNumber = user.TlfNumber,
                role = user.Role
            });
        }

        [HttpPut("users/{id:int}")]
        public async Task<IActionResult> UpdateUser(int id, [FromBody] User input)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var existing = await _adminRepo.GetUserByIdAsync(id);
            if (existing == null) return NotFound();

            existing.FullName = input.FullName;
            existing.Email = input.Email;
            existing.TlfNumber = input.TlfNumber ?? existing.TlfNumber;
            existing.Address = input.Address ?? existing.Address;

            var success = await _adminRepo.UpdateUserAsync(existing);
            if (!success) return StatusCode(500, new { message = "Failed to update user" });

            return Ok(new { message = "User updated" });
        }

        [HttpDelete("users/{id:int}")]
        public async Task<IActionResult> DeleteUser(int id)
        {
            var ok = await _adminRepo.DeleteUserAsync(id);
            return ok ? Ok(new { message = "User deleted" }) : NotFound(new { message = "User not found or cannot delete" });
        }

        //Caregivers
        [HttpGet("caregivers")]
        public async Task<IActionResult> GetCaregivers([FromQuery(Name = "q")] string? q)
        {
            var list = await _adminRepo.GetCaregiversAsync(q);
            var shaped = list.Select(u => new
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

        [HttpGet("caregivers/{id:int}")]
        public async Task<IActionResult> GetCaregiverById(int id)
        {
            var user = await _adminRepo.GetUserByIdAsync(id);
            if (user == null) return NotFound();
            
            // Allow Caregiver or Admin
            if (!(string.Equals(user.Role, "Caregiver", StringComparison.OrdinalIgnoreCase) || 
                string.Equals(user.Role, "Admin", StringComparison.OrdinalIgnoreCase)))
                return NotFound();
                
            return Ok(new
            {
                id = user.Id,
                fullName = user.FullName,
                email = user.Email,
                address = user.Address,
                tlfNumber = user.TlfNumber,
                role = user.Role
            });
        }

        [HttpPut("caregivers/{id:int}")]
        public async Task<IActionResult> UpdateCaregiver(int id, [FromBody] UpdateUserDto input)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var existing = await _adminRepo.GetUserByIdAsync(id);
            if (existing == null) return NotFound();

            // Verify it's actually a caregiver or admin
            if (!(string.Equals(existing.Role, "Caregiver", StringComparison.OrdinalIgnoreCase) || 
                  string.Equals(existing.Role, "Admin", StringComparison.OrdinalIgnoreCase)))
                return NotFound();

            existing.FullName = input.FullName;
            existing.Email = input.Email;
            existing.TlfNumber = input.TlfNumber ?? existing.TlfNumber;
            existing.Address = input.Address ?? existing.Address;

            var success = await _adminRepo.UpdateUserAsync(existing);
            if (!success) return StatusCode(500, new { message = "Failed to update caregiver" });

            return Ok(new { message = "Caregiver updated" });
        }

        // Bookings
        [HttpGet("bookings")]
        public async Task<IActionResult> GetBookings([FromQuery(Name = "q")] string? q)
        {
            var list = await _adminRepo.GetBookingsAsync(q);
            // Map to shape expected by frontend if needed
            var shaped = list.Select(b => new
            {
                id = b.Id,
                clientId = b.UserId,
                caregiverId = b.CaregiverId,
                date = b.Date,
                time = b.Time,
                serviceType = b.ServiceType,
                notes = b.Notes,
                status = b.Status,
                timeSlot = b.TimeSlot?.Slot,
                category = b.Category?.Name
            });
            return Ok(shaped);
        }

        [HttpGet("bookings/{id:int}")]
        public async Task<IActionResult> GetBookingById(int id)
        {
            var b = await _adminRepo.GetBookingByIdAsync(id);
            if (b == null) return NotFound();
            return Ok(new
            {
                id = b.Id,
                clientId = b.UserId,
                caregiverId = b.CaregiverId,
                date = b.Date,
                time = b.Time,
                serviceType = b.ServiceType,
                notes = b.Notes,
                status = b.Status,
                timeSlot = b.TimeSlot?.Slot,
                category = b.Category?.Name
            });
        }

        public class UpdateBookingDto
        {
            public int? ClientId { get; set; }
            public string? CaregiverId { get; set; }
            public DateTime Date { get; set; }
            public string Time { get; set; } = string.Empty;
            public string ServiceType { get; set; } = string.Empty;
            public string? Notes { get; set; }
            public string? Status { get; set; }
        }

        [HttpPut("bookings/{id:int}")]
        public async Task<IActionResult> UpdateBooking(int id, [FromBody] UpdateBookingDto input)
        {
            var existing = await _adminRepo.GetBookingByIdAsync(id);
            if (existing == null) return NotFound();

            existing.UserId = input.ClientId;
            if (!string.IsNullOrWhiteSpace(input.CaregiverId)) existing.CaregiverId = input.CaregiverId!;
            existing.Date = input.Date;
            existing.Time = input.Time;
            existing.ServiceType = input.ServiceType;
            existing.Notes = input.Notes;
            if (!string.IsNullOrWhiteSpace(input.Status)) existing.Status = input.Status!;

            var ok = await _adminRepo.UpdateBookingAsync(existing);
            return ok ? Ok(new { message = "Booking updated" }) : StatusCode(500, new { message = "Failed to update booking" });
        }

        [HttpDelete("bookings/{id:int}")]
        public async Task<IActionResult> DeleteBooking(int id)
        {
            var ok = await _adminRepo.DeleteBookingAsync(id);
            return ok ? Ok(new { message = "Booking deleted" }) : NotFound(new { message = "Booking not found" });
        }

        // Dropdown data for editing bookings
        [HttpGet("booking-data")]
        public async Task<IActionResult> GetBookingData()
        {
            var clients = await _context.AppUsers
                .Where(u => u.Role == "User")
                .Select(u => new { id = u.Id, fullName = u.FullName, email = u.Email })
                .ToListAsync();

            var caregivers = await _context.AppUsers
                .Where(u => u.Role == "Caregiver")
                .Select(u => new { id = u.Id, fullName = u.FullName, email = u.Email })
                .ToListAsync();

            return Ok(new { clients, caregivers });
        }
    }
}