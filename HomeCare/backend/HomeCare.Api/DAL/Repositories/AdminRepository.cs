using HomeCare.Api.Data;
using HomeCare.Api.Models;
using HomeCare.Api.DAL.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace HomeCare.Api.DAL.Repositories
{
    public class AdminRepository : IAdminRepository
    {
        private readonly AppDbContext _context;
        private readonly ILogger<AdminRepository> _logger;

        public AdminRepository(AppDbContext context, ILogger<AdminRepository> logger)
        {
            _context = context;
            _logger = logger;
        }

        //Users
        public async Task<IEnumerable<User>> GetUsersAsync(string? searchTerm)
        {
            try
            {
                IQueryable<User> q = _context.AppUsers;
                if (!string.IsNullOrWhiteSpace(searchTerm))
                {
                    var term = searchTerm.Trim().ToLower();
                    q = q.Where(u =>
                        u.FullName.ToLower().Contains(term) ||
                        u.Email.ToLower().Contains(term) ||
                        u.Address.ToLower().Contains(term) ||
                        u.TlfNumber.ToLower().Contains(term) ||
                        u.Role.ToLower().Contains(term));
                }
                return await q.OrderBy(u => u.Id).ToListAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error fetching users with term {Term}", searchTerm);
                throw;
            }
        }

        public async Task<User?> GetUserByIdAsync(int id)
        {
            try
            {
                return await _context.AppUsers.FirstOrDefaultAsync(u => u.Id == id);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error fetching user {Id}", id);
                throw;
            }
        }

        public async Task<User> AddUserAsync(User user)
        {
            try
            {
                _context.AppUsers.Add(user);
                await _context.SaveChangesAsync();
                return user;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error adding user {Email}", user.Email);
                throw;
            }
        }

        public async Task<bool> UpdateUserAsync(User user)
        {
            try
            {
                _context.AppUsers.Update(user);
                return await _context.SaveChangesAsync() > 0;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error updating user {Id}", user.Id);
                throw;
            }
        }

        public async Task<bool> DeleteUserAsync(int id)
        {
            try
            {
                var user = await _context.AppUsers.FindAsync(id);
                if (user == null) return false;

                // Block deleting the last admin
                if (user.Role == "Admin" && await CountAdminsAsync() <= 1) return false;

                // Block if referenced in bookings (as client or caregiver)
                if (await HasClientBookingsAsync(id)) return false;
                if (await HasCaregiverBookingsAsync(id)) return false;

                _context.AppUsers.Remove(user);
                return await _context.SaveChangesAsync() > 0;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error deleting user {Id}", id);
                throw;
            }
        }

        //Caregiver
        public async Task<IEnumerable<User>> GetCaregiversAsync(string? searchTerm)
        {
            try
            {
                IQueryable<User> q = _context.AppUsers.Where(u => u.Role == "Caregiver" || u.Role == "Personnel");
                if (!string.IsNullOrWhiteSpace(searchTerm))
                {
                    var term = searchTerm.Trim().ToLower();
                    q = q.Where(u =>
                        u.FullName.ToLower().Contains(term) ||
                        u.Email.ToLower().Contains(term));
                }
                return await q.OrderBy(u => u.Id).ToListAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error fetching personnel with term {Term}", searchTerm);
                throw;
            }
        }

        public async Task<bool> DeleteCaregiverAsync(int id)
        {
            try
            {
                var caregiver = await _context.AppUsers.FirstOrDefaultAsync(u => u.Id == id && u.Role == "Caregiver");
                if (caregiver == null) return false;

                if (await HasCaregiverBookingsAsync(id)) return false;

                _context.AppUsers.Remove(caregiver);
                return await _context.SaveChangesAsync() > 0;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error deleting caregiver {Id}", id);
                throw;
            }
        }

        //Bookings
        public async Task<IEnumerable<Booking>> GetBookingsAsync(string? searchTerm)
        {
            try
            {
                IQueryable<Booking> q = _context.Bookings
                    .Include(b => b.User)
                    .Include(b => b.TimeSlot)
                    .Include(b => b.Category);

                if (!string.IsNullOrWhiteSpace(searchTerm))
                {
                    var term = searchTerm.Trim().ToLower();
                    q = q.Where(b =>
                        (b.ServiceType != null && b.ServiceType.ToLower().Contains(term)) ||
                        (b.TimeSlot != null && b.TimeSlot.Slot.ToLower().Contains(term)) ||
                        (b.Category != null && b.Category.Name.ToLower().Contains(term)) ||
                        (b.CaregiverId != null && b.CaregiverId.ToLower().Contains(term)) ||
                        (b.User != null && (
                            b.User.FullName.ToLower().Contains(term) ||
                            b.User.Email.ToLower().Contains(term))));
                }

                return await q
                    .OrderByDescending(b => b.Date)
                    .ThenBy(b => b.Time)
                    .ToListAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error fetching bookings with term {Term}", searchTerm);
                throw;
            }
        }

        public async Task<Booking?> GetBookingByIdAsync(int id)
        {
            try
            {
                return await _context.Bookings
                    .Include(b => b.User)
                    .Include(b => b.TimeSlot)
                    .Include(b => b.Category)
                    .FirstOrDefaultAsync(b => b.Id == id);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error fetching booking {Id}", id);
                throw;
            }
        }

        public async Task<Booking> AddBookingAsync(Booking booking)
        {
            try
            {
                _context.Bookings.Add(booking);
                await _context.SaveChangesAsync();
                return booking;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error adding booking for user {UserId}", booking.UserId);
                throw;
            }
        }

        public async Task<bool> UpdateBookingAsync(Booking booking)
        {
            try
            {
                _context.Bookings.Update(booking);
                return await _context.SaveChangesAsync() > 0;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error updating booking {Id}", booking.Id);
                throw;
            }
        }

        public async Task<bool> DeleteBookingAsync(int id)
        {
            try
            {
                var booking = await _context.Bookings.FindAsync(id);
                if (booking == null) return false;

                _context.Bookings.Remove(booking);
                return await _context.SaveChangesAsync() > 0;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error deleting booking {Id}", id);
                throw;
            }
        }

        // ---------- Helpers ----------
        public async Task<int> CountAdminsAsync()
        {
            try
            {
                return await _context.AppUsers.CountAsync(u => u.Role == "Admin");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error counting admins");
                throw;
            }
        }

        public async Task<bool> HasClientBookingsAsync(int userId)
        {
            try
            {
                return await _context.Bookings.AnyAsync(b => b.UserId == userId);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error checking client bookings for user {UserId}", userId);
                throw;
            }
        }

        public async Task<bool> HasCaregiverBookingsAsync(int caregiverId)
        {
            try
            {
                var cid = caregiverId.ToString();
                return await _context.Bookings.AnyAsync(b => b.CaregiverId == cid);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error checking caregiver bookings for user {CaregiverId}", caregiverId);
                throw;
            }
        }

       
    }
}