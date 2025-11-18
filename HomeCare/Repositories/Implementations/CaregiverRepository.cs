using HomeCare.Data;
using HomeCare.Models;
using HomeCare.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace HomeCare.Repositories.Implementations
{
    public class CaregiverRepository : ICaregiverRepository
    {
        private readonly AppDbContext _context;

        public CaregiverRepository(AppDbContext context)
        {
            _context = context;
        }

        public async Task<Caregiver?> GetCaregiverByIdAsync(string id)
        {
            // Bruker Set<Caregiver>() i stedet for _context.Caregivers 
            // siden Caregiver kommer fra IdentityDbContext
            return await _context.Set<Caregiver>()
                .Include(c => c.Tasks)
                .Include(c => c.Visits)
                .FirstOrDefaultAsync(c => c.Id == id);
        }

        public async Task<IEnumerable<Visit>> GetTodayVisitsAsync(string caregiverId)
        {
            var today = System.DateTime.Today;

            return await _context.Visits
                .Where(v => v.CaregiverId == caregiverId &&
                            v.StartTime.Date == today)
                .Include(v => v.User)
                .ToListAsync();
        }

        public async Task<Visit?> GetVisitByIdAsync(int id)
        {
            return await _context.Visits
                .Include(v => v.User)
                .FirstOrDefaultAsync(v => v.VisitId == id);
        }

        public async Task UpdateVisitStatusAsync(int id, bool isCompleted)
        {
            var visit = await _context.Visits.FindAsync(id);
            if (visit != null)
            {
                visit.IsCompleted = isCompleted;
                await SaveChangesAsync();
            }
        }

        public async Task<IEnumerable<CareTask>> GetCareTasksAsync(string caregiverId)
        {
            return await _context.CareTasks
                .Where(t => t.CaregiverId == caregiverId)
                .Include(t => t.User)
                .ToListAsync();
        }

        public async Task<CareTask?> GetCareTaskByIdAsync(int id)
        {
            return await _context.CareTasks
                .Include(t => t.User)
                .FirstOrDefaultAsync(t => t.CareTaskId == id);
        }

        public async Task UpdateCareTaskAsync(CareTask task)
        {
            _context.CareTasks.Update(task);
            await SaveChangesAsync();
        }

        public async Task SaveChangesAsync()
        {
            await _context.SaveChangesAsync();
        }
    }
}
