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
        public async Task<IActionResult> UpdateUser(int id, [FromBody] UpdateUserDto input)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var existingUser = await _adminRepo.GetUserByIdAsync(id);
            if (existingUser == null)
            {
                return NotFound();
            }

            // Map the changes from the DTO to the existing user entity
            existingUser.FullName = input.FullName;
            existingUser.Email = input.Email;
            existingUser.TlfNumber = input.TlfNumber;
            existingUser.Address = input.Address;

            // Now, update the user with the complete, valid entity
            var success = await _adminRepo.UpdateUserAsync(existingUser);

            if (!success)
            {
                return StatusCode(500, "A problem happened while handling your request.");
            }

            return Ok(new { message = "User updated successfully." });
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
            var bookings = await _adminRepo.GetBookingsAsync(q);
            var shaped = bookings.Select(b => new
            {
                id = b.Id,
                date = b.DateTime,
                time = b.TimeSlot?.Slot,
                status = b.Status.ToString(),
                clientId = b.UserId,
                clientName = b.User?.FullName,
                caregiverId = b.CaregiverId,
                caregiverName = _context.Users.FirstOrDefault(u => u.Id == b.CaregiverId)?.FullName,
                categoryId = b.CategoryId,
                categoryName = b.Category?.Name
            });
            return Ok(shaped);
        }

        [HttpGet("bookings/{id:int}")]
        public async Task<IActionResult> GetBookingById(int id)
        {
            var booking = await _adminRepo.GetBookingByIdAsync(id);
            if (booking == null) return NotFound();
            return Ok(new
            {
                id = booking.Id,
                date = booking.DateTime,
                timeSlotId = booking.TimeSlotId,
                status = booking.Status.ToString(),
                clientId = booking.UserId,
                caregiverId = booking.CaregiverId,
                categoryId = booking.CategoryId
            });
        }

        public class UpdateBookingDto
        {
            public int ClientId { get; set; }
            public int? CaregiverId { get; set; }
            public DateTime Date { get; set; }
            public int TimeSlotId { get; set; }
            public int CategoryId { get; set; }
            public string? Notes { get; set; }
            public string Status { get; set; } = string.Empty;
        }

        [HttpPut("bookings/{id:int}")]
        public async Task<IActionResult> UpdateBooking(int id, [FromBody] UpdateBookingDto input)
        {
            var existing = await _adminRepo.GetBookingByIdAsync(id);
            if (existing == null) return NotFound();

            // Update properties from DTO
            existing.UserId = input.ClientId;
            existing.CaregiverId = input.CaregiverId;
            existing.DateTime = input.Date;
            existing.TimeSlotId = input.TimeSlotId;
            existing.CategoryId = input.CategoryId;
            existing.Notes = input.Notes;
            
            if (Enum.TryParse<HomeCare.Api.Enums.BookingStatus>(input.Status, true, out var statusEnum))
            {
                existing.Status = statusEnum;
            }

            var success = await _adminRepo.UpdateBookingAsync(existing);
            if (!success) return StatusCode(500, "Failed to update booking");

            return Ok(new { message = "Booking updated" });
        }

        [HttpDelete("bookings/{id:int}")]
        public async Task<IActionResult> DeleteBooking(int id)
        {
            var ok = await _adminRepo.DeleteBookingAsync(id);
            return ok ? Ok(new { message = "Booking deleted" }) : NotFound(new { message = "Booking not found" });
        }

        // Dropdown data for editing bookings
        [HttpGet("booking-data")]
        public async Task<IActionResult> GetBookingCreationData()
        {
            var clients = await _context.Users
                .Where(u => u.Role.ToLower() != "admin" && u.Role.ToLower() != "caregiver")
                .Select(u => new { id = u.Id, name = u.FullName }).ToListAsync();

            var caregivers = await _context.Users
                .Where(u => u.Role.ToLower() == "admin" || u.Role.ToLower() == "caregiver")
                .Select(u => new { id = u.Id, name = u.FullName }).ToListAsync();
            
            var categories = await _context.Categories.Select(c => new { id = c.Id, name = c.Name }).ToListAsync();

            return Ok(new { clients, caregivers, categories });
        }
    }
}