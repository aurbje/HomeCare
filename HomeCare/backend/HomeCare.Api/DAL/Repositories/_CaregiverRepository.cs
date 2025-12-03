/* ============================================================
 * ORIGINAL GROUP CODE - Commented out
 * Replaced with merged version in CaregiverRepository.cs
 * This file kept for reference purposes
 * ============================================================

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
        public async Task<IEnumerable<User>> GetClientsForCaregiverAsync(string caregiverId)
        {
            try
            {
                _logger.LogInformation("Fetching clients for caregiver {CaregiverId}", caregiverId);

                return await _context.AppUsers
                    .Where(u => u.Role == "User") // Only fetch normal users
                    .ToListAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error fetching clients for caregiver {CaregiverId}", caregiverId);
                throw;
            }
        }

        /// <summary>
        /// Gets all scheduled bookings for a caregiver.
        /// </summary>
        public async Task<IEnumerable<Booking>> GetBookingsForCaregiverAsync(string caregiverId)
        {
            try
            {
                _logger.LogInformation("Fetching bookings for caregiver {CaregiverId}", caregiverId);

                return await _context.Bookings
                    .Include(b => b.TimeSlot)
                    .Where(b => b.CaregiverId == caregiverId)
                    .OrderBy(b => b.Date)
                    .ToListAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error fetching bookings for caregiver {CaregiverId}", caregiverId);
                throw;
            }
        }
    }
}

*/