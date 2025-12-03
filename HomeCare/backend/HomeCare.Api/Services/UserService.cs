using HomeCare.Api.DAL.Interfaces;
using HomeCare.Api.DTO.User;
using HomeCare.Api.Services.Interfaces;

namespace HomeCare.Api.Services
{
    public class UserService : IUserService
    {
        private readonly IUserRepository _repo;

        public UserService(IUserRepository repo)
        {
            _repo = repo;
        }

        public async Task<UserDashboardDto?> GetDashboardAsync(int UserId)
        {
            var user = await _repo.GetByIdAsync(UserId);
            if (user == null)
                return null;

            var reminders = await GetRemindersForUserAsync(UserId);

            var todayBookings = (await _repo.GetTodayBookingsAsync(UserId))
                .Select(a => new BookingSummaryDto
                {
                    Id = a.Id,
                    DateTime = a.DateTime,
                    CategoryName = a.Category?.Name,
                    CaregiverName = a.Caregiver?.FullName,
                    Notes = a.Notes
                })
                .ToList();

            var upcomingBookings = (await _repo.GetUpcomingBookingsAsync(UserId))
                .Select(a => new BookingSummaryDto
                {
                    Id = a.Id,
                    DateTime = a.DateTime,
                    CategoryName = a.Category?.Name,
                    CaregiverName = a.Caregiver?.FullName,
                    Notes = a.Notes
                })
                .ToList();

            var calendarBookings = (await _repo.GetCalendarBookingsAsync(UserId))
                .Select(a => new CalendarBookingDto
                {
                    Id = a.Id,
                    DateTime = a.DateTime,
                    CategoryName = a.Category?.Name,
                    CaregiverName = a.Caregiver?.FullName,
                    Notes = a.Notes
                })
                .ToList();

            return new UserDashboardDto
            {
                UserName = user.FullName,
                Reminders = reminders,
                TodayBookings = todayBookings,
                UpcomingBookings = upcomingBookings,
                CalendarBookings = calendarBookings
            };
        }

        private async Task<List<ReminderDto>> GetRemindersForUserAsync(int UserId)
        {
            try
            {
                var reminders = await _repo.GetRemindersAsync(UserId);

                if (reminders.Any())
                {
                    return reminders.Select(r => new ReminderDto
                    {
                        Id = r.Id,
                        Time = r.Time.ToString("HH:mm"),
                        Message = r.Message,
                        IsCompleted = r.IsCompleted
                    }).ToList();
                }
            }
            catch
            {
                // Reminders table may not exist yet
            }

            // Return sample reminders if no real data exists
            return new List<ReminderDto>
            {
                new ReminderDto { Id = 0, Time = "08:00", Message = "Ta medisin", IsCompleted = false },
                new ReminderDto { Id = 0, Time = "12:00", Message = "Spis lunsj", IsCompleted = false },
                new ReminderDto { Id = 0, Time = "18:00", Message = "Ta kveldsmedisinen", IsCompleted = false }
            };
        }

        /// <summary>
        /// Gets reminders for a specific user (implements IUserService).
        /// </summary>
        public async Task<List<Models.Reminder>> GetRemindersAsync(int userId)
        {
            return await _repo.GetRemindersAsync(userId);
        }

        /// <summary>
        /// Gets today's bookings for a user (implements IUserService).
        /// </summary>
        public async Task<List<Models.Booking>> GetTodayBookingsAsync(int userId)
        {
            return await _repo.GetTodayBookingsAsync(userId);
        }

        /// <summary>
        /// Gets upcoming bookings for a user (implements IUserService).
        /// </summary>
        public async Task<List<Models.Booking>> GetUpcomingBookingsAsync(int userId, int limit = 5)
        {
            return await _repo.GetUpcomingBookingsAsync(userId, limit);
        }
    }
}
