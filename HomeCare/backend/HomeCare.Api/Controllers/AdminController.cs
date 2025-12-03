using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using HomeCare.Api.Data;
using Microsoft.EntityFrameworkCore;
using HomeCare.Api.Models;

namespace HomeCare.Api.Controllers
{
    [Authorize(Roles = "Admin")]
    [ApiController]
    [Route("api/[controller]")]
    public class AdminController : ControllerBase
    {
        private readonly AppDbContext _context;
        private readonly ILogger<AdminController> _logger;
        
        public AdminController(AppDbContext context, ILogger<AdminController> logger)
        {
            _context = context;
            _logger = logger;
        }

        // ========== USERS ENDPOINTS ==========

        // GET: api/admin/users?q=search
        [HttpGet("users")]
        public async Task<ActionResult<IEnumerable<User>>> GetUsers(string? q)
        {
            try
            {
                IQueryable<User> query = _context.Users;

                if (!string.IsNullOrWhiteSpace(q))
                {
                    var term = q.Trim().ToLower();
                    query = query.Where(u =>
                        u.Email.ToLower().Contains(term) ||
                        u.FullName.ToLower().Contains(term));
                }

                var users = await query
                    .OrderBy(u => u.Id)
                    .Take(500)
                    .ToListAsync();

                return Ok(users);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error fetching users");
                return StatusCode(500, new { message = "Error fetching users" });
            }
        }
        
        // GET: api/admin/users/{id}
        [HttpGet("users/{id}")]
        public async Task<ActionResult<User>> GetUser(int id)
        {
            try
            {
                var user = await _context.Users.FindAsync(id);
                if (user == null)
                {
                    return NotFound(new { message = "Bruker ikke funnet" });
                }
                return Ok(user);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error fetching user {Id}", id);
                return StatusCode(500, new { message = "Error fetching user" });
            }
        }

        // PUT: api/admin/users/{id}
        [HttpPut("users/{id}")]
        public async Task<IActionResult> UpdateUser(int id, [FromBody] User model)
        {
            try
            {
                var user = await _context.Users.FindAsync(id);
                if (user == null)
                {
                    return NotFound(new { message = "Bruker ikke funnet" });
                }

                user.FullName = model.FullName;
                user.Email = model.Email;
                user.TlfNumber = model.TlfNumber;
                user.Address = model.Address;
                user.Role = model.Role;

                await _context.SaveChangesAsync();
                return Ok(new { message = $"Bruker {user.FullName} ble oppdatert", user });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error updating user {Id}", id);
                return StatusCode(500, new { message = "Error updating user" });
            }
        }
        
        // DELETE: api/admin/users/{id}
        [HttpDelete("users/{id}")]
        public async Task<IActionResult> DeleteUser(int id)
        {
            try
            {
                var user = await _context.Users.FindAsync(id);
                if (user == null)
                {
                    return NotFound(new { message = "Bruker ikke funnet" });
                }

                // safetyguard - Do not delete the last admin
                if (user.Role == "Admin" && await _context.Users.CountAsync(u => u.Role == "Admin") <= 1)
                {
                    return BadRequest(new { message = "Kan ikke slette den siste admin-brukeren" });
                }

                // check references
                var hasBookings = await _context.Bookings.AnyAsync(b => b.UserId == id);
                if (hasBookings)
                {
                    return BadRequest(new { message = "Kan ikke slette bruker som er knyttet til bookinger" });
                }

                _context.Users.Remove(user);
                await _context.SaveChangesAsync();

                return Ok(new { message = $"Bruker {user.FullName} ble slettet" });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error deleting user {Id}", id);
                return StatusCode(500, new { message = "Error deleting user" });
            }
        }

        // ========== PERSONNEL ENDPOINTS ==========

        // GET: api/admin/personnel?q=search
        [HttpGet("personnel")]
        public async Task<ActionResult<IEnumerable<User>>> GetPersonnel(string? q)
        {
            try
            {
                IQueryable<User> query = _context.Users.Where(u => u.Role == "Caregiver");

                if (!string.IsNullOrWhiteSpace(q))
                {
                    var term = q.Trim().ToLower();
                    query = query.Where(u =>
                        u.Email.ToLower().Contains(term) ||
                        u.FullName.ToLower().Contains(term));
                }

                var users = await query
                    .OrderBy(u => u.Id)
                    .Take(500)
                    .ToListAsync();

                return Ok(users);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error fetching personnel");
                return StatusCode(500, new { message = "Error fetching personnel" });
            }
        }

        // GET: api/admin/personnel/{id}
        [HttpGet("personnel/{id}")]
        public async Task<ActionResult<User>> GetPersonnelById(int id)
        {
            try
            {
                var user = await _context.Users.FindAsync(id);
                
                if (user == null || user.Role != "Caregiver")
                {
                    return NotFound(new { message = "Personell ikke funnet" });
                }

                return Ok(user);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error fetching personnel {Id}", id);
                return StatusCode(500, new { message = "Error fetching personnel" });
            }
        }

        // PUT: api/admin/personnel/{id}
        [HttpPut("personnel/{id}")]
        public async Task<IActionResult> UpdatePersonnel(int id, [FromBody] User model)
        {
            try
            {
                var user = await _context.Users.FindAsync(id);
                if (user == null || user.Role != "Caregiver")
                {
                    return NotFound(new { message = "Personell ikke funnet" });
                }

                user.FullName = model.FullName;
                user.Email = model.Email;
                user.TlfNumber = model.TlfNumber;
                user.Address = model.Address;

                await _context.SaveChangesAsync();
                return Ok(new { message = $"Personell {user.FullName} ble oppdatert", user });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error updating personnel {Id}", id);
                return StatusCode(500, new { message = "Error updating personnel" });
            }
        }

        // DELETE: api/admin/personnel/{id}
        [HttpDelete("personnel/{id}")]
        public async Task<IActionResult> DeletePersonnel(int id)
        {
            try
            {
                var user = await _context.Users.FindAsync(id);
                if (user == null)
                {
                    return NotFound(new { message = "Personell ikke funnet" });
                }

                // Checks references - CaregiverId is int? now
                var hasBookings = await _context.Bookings.AnyAsync(b => b.CaregiverId == id);
                if (hasBookings)
                {
                    return BadRequest(new { message = "Kan ikke slette personell som er knyttet til bookinger" });
                }

                _context.Users.Remove(user);
                await _context.SaveChangesAsync();

                return Ok(new { message = $"Personell {user.FullName} ble slettet" });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error deleting personnel {Id}", id);
                return StatusCode(500, new { message = "Error deleting personnel" });
            }
        }

        // ========== BOOKINGS ENDPOINTS ==========

        // GET: api/admin/bookings?q=search
        [HttpGet("bookings")]
        public async Task<ActionResult<IEnumerable<Booking>>> GetBookings(string? q)
        {
            try
            {
                IQueryable<Booking> query = _context.Bookings
                    .Include(b => b.User)
                    .Include(b => b.Category);

                if (!string.IsNullOrWhiteSpace(q))
                {
                    var term = q.Trim().ToLower();
                    query = query.Where(b =>
                        (b.User != null && (
                            b.User.FullName.ToLower().Contains(term) ||
                            b.User.Email.ToLower().Contains(term))) ||
                        (b.Category != null && b.Category.Name.ToLower().Contains(term)));
                }

                var bookings = await query
                    .OrderByDescending(b => b.DateTime)
                    .Take(500)
                    .ToListAsync();

                return Ok(bookings);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error fetching bookings");
                return StatusCode(500, new { message = "Error fetching bookings" });
            }
        }

        // GET: api/admin/bookings/{id}
        [HttpGet("bookings/{id}")]
        public async Task<ActionResult<Booking>> GetBooking(int id)
        {
            try
            {
                var booking = await _context.Bookings
                    .Include(b => b.User)
                    .FirstOrDefaultAsync(b => b.Id == id);

                if (booking == null)
                {
                    return NotFound(new { message = "Booking ikke funnet" });
                }

                return Ok(booking);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error fetching booking {Id}", id);
                return StatusCode(500, new { message = "Error fetching booking" });
            }
        }

        // GET: api/admin/booking-data (for dropdowns)
        [HttpGet("booking-data")]
        public async Task<IActionResult> GetBookingData()
        {
            try
            {
                var clients = await _context.Users.Where(u => u.Role == "User").ToListAsync();
                var personnel = await _context.Users.Where(u => u.Role == "Caregiver").ToListAsync();

                return Ok(new { clients, personnel });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error fetching booking data");
                return StatusCode(500, new { message = "Error fetching booking data" });
            }
        }

        // PUT: api/admin/bookings/{id}
        [HttpPut("bookings/{id}")]
        public async Task<IActionResult> UpdateBooking(int id, [FromBody] Booking model)
        {
            try
            {
                var booking = await _context.Bookings.FindAsync(id);
                if (booking == null)
                {
                    return NotFound(new { message = "Booking ikke funnet" });
                }

                // Updated to use new Booking model properties
                booking.DateTime = model.DateTime;
                booking.CategoryId = model.CategoryId;
                booking.Notes = model.Notes;
                booking.CaregiverId = model.CaregiverId;
                booking.UserId = model.UserId;

                await _context.SaveChangesAsync();
                return Ok(new { message = $"Booking {booking.Id} ble oppdatert", booking });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error updating booking {Id}", id);
                return StatusCode(500, new { message = "Error updating booking" });
            }
        }

        // DELETE: api/admin/bookings/{id}
        [HttpDelete("bookings/{id}")]
        public async Task<IActionResult> DeleteBooking(int id)
        {
            try
            {
                var booking = await _context.Bookings.FindAsync(id);
                if (booking == null)
                {
                    return NotFound(new { message = "Booking ikke funnet" });
                }

                // Optional safety: block deletion of past bookings
                if (booking.DateTime.Date < DateTime.Today)
                {
                    return BadRequest(new { message = "Kan ikke slette tidligere booking" });
                }

                _context.Bookings.Remove(booking);
                await _context.SaveChangesAsync();
                
                return Ok(new { message = $"Booking {booking.Id} ble slettet" });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error deleting booking {Id}", id);
                return StatusCode(500, new { message = "Error deleting booking" });
            }
        }
    }
}
