using HomeCare.Api.Data;
using HomeCare.Api.DAL.Interfaces;
using HomeCare.Api.Models;
using Microsoft.EntityFrameworkCore;

namespace HomeCare.Api.DAL.Repositories
{
    public class UserRepository : IUserRepository
    {
        private readonly AppDbContext _context;

        public UserRepository(AppDbContext context)
        {
            _context = context;
        }

        // Basic user operations
        public async Task<User?> GetByIdAsync(int id)
        {
            return await _context.Users.FindAsync(id);
        }

        public async Task<User?> GetByEmailAsync(string email)
        {
            return await _context.Users.FirstOrDefaultAsync(u => u.Email == email);
        }

        public async Task<bool> EmailExistsAsync(string email)
        {
            return await _context.Users.AnyAsync(u => u.Email == email);
        }

        public async Task AddAsync(User user)
        {
            await _context.Users.AddAsync(user);
        }

        // Note: Update() is synchronous in EF Core - it only marks the entity as modified.
        // The actual DB operation happens in SaveChangesAsync(). 
        // We keep async signature for interface consistency.
        public Task UpdateAsync(User user)
        {
            _context.Users.Update(user);
            return Task.CompletedTask;
        }

        // Note: Remove() is synchronous in EF Core - it only marks the entity for deletion.
        // The actual DB operation happens in SaveChangesAsync().
        // We keep async signature for interface consistency.
        public Task DeleteAsync(User user)
        {
            _context.Users.Remove(user);
            return Task.CompletedTask;
        }

        public async Task SaveChangesAsync()
        {
            await _context.SaveChangesAsync();
        }

        // Client-specific dashboard operations (using UserId from Booking/Reminder models)
        public async Task<List<Reminder>> GetRemindersAsync(int UserId)
        {
            return await _context.Reminders
                .Where(r => r.UserId == UserId && !r.IsCompleted)
                .OrderBy(r => r.Time)
                .ToListAsync();
        }

        public async Task<List<Booking>> GetTodayBookingsAsync(int UserId)
        {
            var today = DateTime.Today;
            var todayEnd = today.AddDays(1);

            return await _context.Bookings
                .Include(b => b.Category)
                .Include(b => b.Caregiver)
                .Where(b => b.UserId == UserId && b.DateTime >= today && b.DateTime < todayEnd)
                .OrderBy(b => b.DateTime)
                .ToListAsync();
        }

        public async Task<List<Booking>> GetUpcomingBookingsAsync(int UserId, int limit = 5)
        {
            var todayEnd = DateTime.Today.AddDays(1);

            return await _context.Bookings
                .Include(b => b.Category)
                .Include(b => b.Caregiver)
                .Where(b => b.UserId == UserId && b.DateTime >= todayEnd)
                .OrderBy(b => b.DateTime)
                .Take(limit)
                .ToListAsync();
        }

        public async Task<List<Booking>> GetCalendarBookingsAsync(int UserId)
        {
            var startDate = DateTime.Today.AddMonths(-1);

            return await _context.Bookings
                .Include(b => b.Category)
                .Include(b => b.Caregiver)
                .Where(b => b.UserId == UserId && b.DateTime >= startDate)
                .OrderBy(b => b.DateTime)
                .ToListAsync();
        }
    }
}
