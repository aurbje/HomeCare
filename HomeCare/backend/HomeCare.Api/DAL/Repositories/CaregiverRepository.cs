using HomeCare.Api.Data;
using HomeCare.Api.Models;
using HomeCare.Api.DAL.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace HomeCare.Api.DAL.Repositories
{
    public class CaregiverRepository : ICaregiverRepository
    {
        private readonly AppDbContext _context;
        private readonly ILogger<CaregiverRepository> _logger;

        public CaregiverRepository(AppDbContext context, ILogger<CaregiverRepository> logger)
        {
            _context = context;
            _logger = logger;
        }

        /// <summary>
        /// Gets all clients assigned to a specific caregiver.
        /// </summary>
        public async Task<IEnumerable<ApplicationUser>> GetClientsForCaregiverAsync(string caregiverId)
        {
            try
            {
                _logger.LogInformation("Fetching clients for caregiver {CaregiverId}", caregiverId);

                return await _context.Users
                    .Where(u => u.CaregiverId == caregiverId)
                    .ToListAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error fetching clients for caregiver {CaregiverId}", caregiverId);
                throw;
            }
        }

        /// <summary>
        /// Gets all scheduled appointments for a caregiver.
        /// </summary>
        public async Task<IEnumerable<Appointment>> GetAppointmentsForCaregiverAsync(string caregiverId)
        {
            try
            {
                _logger.LogInformation("Fetching appointments for caregiver {CaregiverId}", caregiverId);

                return await _context.Appointments
                    .Include(a => a.TimeSlot)
                    .Include(a => a.Category)
                    .Where(a => a.CaregiverId == caregiverId)
                    .OrderBy(a => a.DateTime)
                    .ToListAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error fetching appointments for caregiver {CaregiverId}", caregiverId);
                throw;
            }
        }
    }
}
