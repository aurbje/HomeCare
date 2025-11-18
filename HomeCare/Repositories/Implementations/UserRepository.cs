using HomeCare.Data;
using HomeCare.Models;
using HomeCare.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace HomeCare.Repositories.Implementations
{
    public class UserRepository : IUserRepository
    {
        private readonly AppDbContext _context;

        public UserRepository(AppDbContext context)
        {
            _context = context;
        }

        public async Task<User?> GetUserByIdAsync(int id)
        {
            return await _context.AppUsers
                .Include(u => u.Visits)
                .Include(u => u.CareTasks)
                .FirstOrDefaultAsync(u => u.Id == id);
        }

        public async Task<User?> GetByEmailAsync(string email)
        {
            return await _context.AppUsers
                .FirstOrDefaultAsync(u => u.Email == email);
        }

        public async Task<bool> EmailExistsAsync(string email)
        {
            return await _context.AppUsers
                .AnyAsync(u => u.Email == email);
        }

        public async Task<IEnumerable<User>> GetAllUsersAsync()
        {
            return await _context.AppUsers
                .Include(u => u.Visits)
                .Include(u => u.CareTasks)
                .ToListAsync();
        }

        public async Task AddAsync(User user)
        {
            await _context.AppUsers.AddAsync(user);
        }

        public Task UpdateUserAsync(User user)
{
    _context.AppUsers.Update(user);
    return Task.CompletedTask;
}

        public async Task DeleteUserAsync(int id)
        {
            var user = await _context.AppUsers.FindAsync(id);
            if (user != null)
                _context.AppUsers.Remove(user);
        }

        public async Task SaveChangesAsync()
        {
            await _context.SaveChangesAsync();
        }
    }
}
