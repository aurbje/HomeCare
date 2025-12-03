using HomeCare.Api.DAL.Interfaces;
using HomeCare.Api.Services.Interfaces;
using HomeCare.Api.DTO.User;
using HomeCare.Api.Models;

namespace HomeCare.Api.Services
{
    public class CaregiverService : ICaregiverService
    {
        private readonly ICaregiverRepository _repo;

        public CaregiverService(ICaregiverRepository repo)
        {
            _repo = repo;
        }

        public async Task<CaregiverDashboardDto> GetDashboardAsync(int CaregiverId, int? year, int? month)
        {
            var model = await _repo.GetDashboardAsync(CaregiverId);

            // Add upcoming bookings
            var bookings = await _repo.GetBookingsForCaregiverAsync(CaregiverId);
            model.UpcomingBookings = bookings.Select(a => new BookingSummaryDto
            {
                Id = a.Id,
                DateTime = a.DateTime,
                CategoryName = a.Category?.Name,
                CaregiverName = null,
                Notes = a.Notes
            }).ToList();

            return model;
        }

        public Task RegisterAvailabilityAsync(int CaregiverId, DateTime date)
            => _repo.AddAvailabilityAsync(CaregiverId, date);

        public async Task RegisterMultipleAvailabilityAsync(int CaregiverId, List<DateTime> dates)
        {
            foreach (var date in dates)
            {
                if (date.Date >= DateTime.Today)
                {
                    await _repo.AddAvailabilityAsync(CaregiverId, date);
                }
            }
        }

        public Task DeleteAvailabilityAsync(int CaregiverId, DateTime date)
            => _repo.DeleteAvailabilityAsync(CaregiverId, date);

        public async Task<bool> RequestAvailabilityDeletionAsync(int CaregiverId, DateTime date)
        {
            var deleted = await _repo.TryDeleteAvailabilityWithCheckAsync(CaregiverId, date.Date);
            if (!deleted)
            {
                var Caregiver = await _repo.GetCaregiverByIdAsync(CaregiverId);
                var fullName = Caregiver?.FullName ?? $"ID {CaregiverId}";
                var message = $"Personell {CaregiverId}: {fullName} ønsker å slette sin tilgjengelighet for {date:dd.MM.yyyy}, men en kunde har allerede en bestilling på denne datoen.";
                await _repo.AddAdminNotificationAsync(message);
            }
            return deleted;
        }

        /// <summary>
        /// Gets today's visits for a caregiver (implements ICaregiverService).
        /// </summary>
        public async Task<List<Booking>> GetTodayVisitsAsync(int caregiverId)
        {
            var allBookings = await _repo.GetBookingsForCaregiverAsync(caregiverId);
            return allBookings.Where(b => b.DateTime.Date == DateTime.Today).ToList();
        }

        /// <summary>
        /// Gets upcoming bookings for a caregiver (implements ICaregiverService).
        /// </summary>
        public async Task<List<Booking>> GetUpcomingBookingsAsync(int caregiverId)
        {
            var allBookings = await _repo.GetBookingsForCaregiverAsync(caregiverId);
            return allBookings.Where(b => b.DateTime.Date >= DateTime.Today).OrderBy(b => b.DateTime).ToList();
        }
    }
}
