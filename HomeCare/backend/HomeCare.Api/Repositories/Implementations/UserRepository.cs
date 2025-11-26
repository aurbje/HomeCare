using HomeCare.Api.Data;
using HomeCare.Api.Models;
using HomeCare.Api.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace HomeCare.Api.Repositories.Implementations;

public class UserRepository : IUserRepository
{
    private readonly AppDbContext _context;

    public UserRepository(AppDbContext context)
    {
        _context = context;
    }

    public async Task<ApplicationUser?> GetByIdAsync(string id)
    {
        return await _context.Users
            .FirstOrDefaultAsync(u => u.Id == id);
    }

    public async Task<ApplicationUser?> GetByEmailAsync(string email)
    {
        return await _context.Users
            .FirstOrDefaultAsync(u => u.Email == email);
    }

    public async Task<IEnumerable<ApplicationUser>> GetAllAsync()
    {
        return await _context.Users
            .OrderBy(u => u.FullName)
            .ToListAsync();
    }
}
