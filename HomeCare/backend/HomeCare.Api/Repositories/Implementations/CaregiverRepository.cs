using HomeCare.Api.Data;
using HomeCare.Api.Models;
using HomeCare.Api.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace HomeCare.Api.Repositories.Implementations;

public class CaregiverRepository : ICaregiverRepository
{
    private readonly AppDbContext _context;

    public CaregiverRepository(AppDbContext context)
    {
        _context = context;
    }

    public async Task<IEnumerable<ApplicationUser>> GetClientsForCaregiverAsync(string caregiverId)
    {
        return await _context.Users
            .Where(u => u.CaregiverId == caregiverId)
            .ToListAsync();
    }

    public async Task<IEnumerable<Appointment>> GetAppointmentsForCaregiverAsync(string caregiverId)
    {
        return await _context.Appointments
            .Include(a => a.TimeSlot)
            .Include(a => a.Category)
            .Where(a => a.CaregiverId == caregiverId)
            .OrderBy(a => a.DateTime)
            .ToListAsync();
    }
}
