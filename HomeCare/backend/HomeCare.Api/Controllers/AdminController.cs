using HomeCare.Api.DAL.Interfaces;
using HomeCare.Api.Data;
using HomeCare.Api.Models;
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

        // Users
        public async Task<IEnumerable<User>> GetUsersAsync(string? searchTerm)
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

        public async Task<User?> GetUserByIdAsync(int id) =>
            await _context.AppUsers.FirstOrDefaultAsync(u => u.Id == id);

        public async Task<User> AddUserAsync(User user)
        {
            _context.AppUsers.Add(user);
            await _context.SaveChangesAsync();
            return user;
        }

        public async Task<bool> UpdateUserAsync(User user)
        {
            _context.AppUsers.Update(user);
            return await _context.SaveChangesAsync() > 0;
        }

        public async Task<bool> DeleteUserAsync(int id)
        {
            var user = await _context.AppUsers.FindAsync(id);
            if (user == null) return false;

            // block deleting last admin
            if (user.Role == "Admin" && await CountAdminsAsync() <= 1) return false;

            // block if referenced in bookings
            if (await HasClientBookingsAsync(id)) return false;
            if (await HasCaregiverBookingsAsync(id)) return false;

            _context.AppUsers.Remove(user);
            return await _context.SaveChangesAsync() > 0;
        }

        // Caregivers
        public async Task<IEnumerable<User>> GetCaregiversAsync(string? searchTerm)
        {
            IQueryable<User> q = _context.AppUsers.Where(u => u.Role == "Caregiver");
            if (!string.IsNullOrWhiteSpace(searchTerm))
            {
                var term = searchTerm.Trim().ToLower();
                q = q.Where(u =>
                    u.FullName.ToLower().Contains(term) ||
                    u.Email.ToLower().Contains(term));
            }
            return await q.OrderBy(u => u.Id).ToListAsync();
        }

        public async Task<bool> DeleteCaregiverAsync(int id)
        {
            var caregiver = await _context.AppUsers.FirstOrDefaultAsync(u => u.Id == id && u.Role == "Caregiver");
            if (caregiver == null) return false;

            if (await HasCaregiverBookingsAsync(id)) return false;

            _context.AppUsers.Remove(caregiver);
            return await _context.SaveChangesAsync() > 0;
        }

        // Bookings
        public async Task<IEnumerable<Booking>> GetBookingsAsync(string? searchTerm)
        {
            IQueryable<Booking> q = _context.Bookings
                .Include(b => b.TimeSlot);

            if (!string.IsNullOrWhiteSpace(searchTerm))
            {
                var term = searchTerm.Trim().ToLower();
                q = q.Where(b =>
                    b.ServiceType.ToLower().Contains(term) ||
                    (b.TimeSlot != null && b.TimeSlot.Label.ToLower().Contains(term)) ||
                    b.CaregiverId.ToLower().Contains(term) ||
                    b.ClientId.ToString().Contains(term));
            }

            return await q
                .OrderByDescending(b => b.Date)
                .ThenBy(b => b.TimeSlotId)
                .ToListAsync();
        }

        public async Task<Booking?> GetBookingByIdAsync(int id) =>
            await _context.Bookings
                .Include(b => b.TimeSlot)
                .FirstOrDefaultAsync(b => b.Id == id);

        public async Task<Booking> AddBookingAsync(Booking booking)
        {
            _context.Bookings.Add(booking);
            await _context.SaveChangesAsync();
            return booking;
        }

        public async Task<bool> UpdateBookingAsync(Booking booking)
        {
            _context.Bookings.Update(booking);
            return await _context.SaveChangesAsync() > 0;
        }

        public async Task<bool> DeleteBookingAsync(int id)
        {
            var booking = await _context.Bookings.FindAsync(id);
            if (booking == null) return false;

            _context.Bookings.Remove(booking);
            return await _context.SaveChangesAsync() > 0;
        }

        // helpers
        public async Task<int> CountAdminsAsync() =>
            await _context.AppUsers.CountAsync(u => u.Role == "Admin");

        public async Task<bool> HasClientBookingsAsync(int userId) =>
            await _context.Bookings.AnyAsync(b => b.ClientId == userId);

        public async Task<bool> HasCaregiverBookingsAsync(int caregiverUserId) =>
            await _context.Bookings.AnyAsync(b => b.CaregiverId == caregiverUserId.ToString());
    }
}