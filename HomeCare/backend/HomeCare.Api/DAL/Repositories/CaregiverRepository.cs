using HomeCare.Api.Data;
using HomeCare.Api.DAL.Interfaces;
using HomeCare.Api.DTO;
using HomeCare.Api.Models;
using Microsoft.EntityFrameworkCore;

namespace HomeCare.Api.DAL.Repositories
{
    public class CaregiverRepository : ICaregiverRepository
    {
        private readonly AppDbContext _context;

        public CaregiverRepository(AppDbContext context)
        {
            _context = context;
        }
// get caregiver dashboard data
        public async Task<CaregiverDashboardDto> GetDashboardAsync(int CaregiverId)
        {
            var user = await _context.Users.FindAsync(CaregiverId);
            if (user == null)
            {
                throw new Exception($"User with ID {CaregiverId} not found.");
            }

            var availabilities = await _context.CaregiverAvailabilities
                .Where(booking => booking.CaregiverId == CaregiverId)
                .Select(a => a.Date)
                .ToListAsync();

            // use date range comparison for SQLite compatibility
            var todayStart = DateTime.Today;
            var todayEnd = DateTime.Today.AddDays(1);

            var todayVisits = await _context.Bookings
                .Include(a => a.User)
                .Include(a => a.Category)
                .Where(a => a.CaregiverId == CaregiverId && a.DateTime >= todayStart && a.DateTime < todayEnd)
                .OrderBy(a => a.DateTime)
                .Select(a => new VisitInfoDto
                {
                    Time = a.DateTime,
                    ClientName = a.User != null ? a.User.FullName : "Ukjent",
                    Address = a.User != null ? a.User.Address : "",
                    Phone = a.User != null ? a.User.TlfNumber : "",
                    Tasks = new List<string> { a.Category != null ? a.Category.Name : "Ukjent" }
                })
                .ToListAsync();

            // get calendar events for the Caregiver (upcoming bookings)
            var calendarEvents = await _context.Bookings
                .Include(a => a.User)
                .Include(a => a.Category)
                .Where(a => a.CaregiverId == CaregiverId && a.DateTime >= todayStart)
                .OrderBy(a => a.DateTime)
                .Select(a => new CalendarEventDto
                {
                    StartTime = a.DateTime,
                    Title = a.Category != null ? a.Category.Name : "Avtale",
                    ClientName = a.User != null ? a.User.FullName : "Ukjent",
                    CategoryName = a.Category != null ? a.Category.Name : ""
                })
                .ToListAsync();

            return new CaregiverDashboardDto
            {
                CaregiverId = CaregiverId,
                CaregiverName = user.FullName,
                AvailableDates = availabilities,
                TodayVisits = todayVisits,
                CalendarEvents = calendarEvents
            };
        }
// add caregiver availability
        public async Task AddAvailabilityAsync(int CaregiverId, DateTime date)
        {
            bool alreadyExists = await _context.CaregiverAvailabilities
                .AnyAsync(a => a.CaregiverId == CaregiverId && a.Date.Date == date.Date);

            if (alreadyExists)
            {
                return;
            }

            _context.CaregiverAvailabilities.Add(new CaregiverAvailability
            {
                CaregiverId = CaregiverId,
                Date = date
            });
// ensure AvailableDate and TimeSlots exist
            var existingDate = await _context.AvailableDates
                .FirstOrDefaultAsync(d => d.Date.Date == date.Date);

            if (existingDate == null)
            {
                var newDate = new AvailableDate { Date = date.Date };
                _context.AvailableDates.Add(newDate);

                var defaultSlots = new List<string> {
                    "09:00-10:00",
                    "10:00-11:00",
                    "11:00-12:00",
                    "13:00-14:00",
                    "14:00-15:00"
                };
                foreach (var slot in defaultSlots)
                {
                    _context.TimeSlots.Add(new TimeSlot
                    {
                        Slot = slot,
                        AvailableDate = newDate,
                        IsBooked = false
                    });
                }
            }

            await _context.SaveChangesAsync();
        }
// delete caregiver availability with booking check
        public async Task DeleteAvailabilityAsync(int CaregiverId, DateTime date)
        {
            bool hasBooking = await _context.Bookings
                .AnyAsync(a => a.CaregiverId == CaregiverId && a.DateTime.Date == date.Date);

            if (hasBooking)
            {
                throw new InvalidOperationException("Denne datoen kan ikke slettes siden den allerede er booket. Ta kontakt med administratoren.");
            }
// remove caregiver availability
            var availability = await _context.CaregiverAvailabilities
                .FirstOrDefaultAsync(a => a.CaregiverId == CaregiverId && a.Date.Date == date.Date);

            if (availability != null)
            {
                _context.CaregiverAvailabilities.Remove(availability);
                await _context.SaveChangesAsync();
            }

            bool isDateStillUsed = await _context.CaregiverAvailabilities
                .AnyAsync(a => a.Date.Date == date.Date && a.CaregiverId != CaregiverId);
// if no other caregivers use the date, remove it and its timeslots
            if (!isDateStillUsed)
            {
                var availableDate = await _context.AvailableDates
                    .Include(d => d.TimeSlots)
                    .FirstOrDefaultAsync(d => d.Date.Date == date.Date);

                if (availableDate != null)
                {
                    _context.TimeSlots.RemoveRange(availableDate.TimeSlots);
                    _context.AvailableDates.Remove(availableDate);
                    await _context.SaveChangesAsync();
                }
            }

            await _context.SaveChangesAsync();
        }
// check if caregiver has booking on a specific date
        public async Task<bool> HasBookingOnDateAsync(int CaregiverId, DateTime date)
        {
            return await _context.Bookings
                .AnyAsync(a => a.CaregiverId == CaregiverId && a.DateTime.Date == date.Date);
        }

        public async Task<List<Booking>> GetBookingsForCaregiverAsync(int CaregiverId)
        {
            return await _context.Bookings
                .Include(a => a.TimeSlot)
                .Include(a => a.Category)
                .Include(a => a.User)
                .Where(a => a.CaregiverId == CaregiverId && a.DateTime.Date >= DateTime.Today)
                .OrderBy(a => a.DateTime)
                .ToListAsync();
        }
// try delete caregiver availability with booking check
        public async Task<bool> TryDeleteAvailabilityWithCheckAsync(int CaregiverId, DateTime date)
        {
            bool hasBooking = await _context.Bookings
                .AnyAsync(a => a.CaregiverId == CaregiverId && a.DateTime.Date == date.Date);

            if (hasBooking)
            {
                return false;
            }

            var availability = await _context.CaregiverAvailabilities
                .FirstOrDefaultAsync(a => a.CaregiverId == CaregiverId && a.Date.Date == date.Date);

            if (availability != null)
            {
                _context.CaregiverAvailabilities.Remove(availability);
            }
// check if other caregivers use the same date
            bool isDateStillUsed = await _context.CaregiverAvailabilities
                .AnyAsync(a => a.Date.Date == date.Date && a.CaregiverId != CaregiverId);

            if (!isDateStillUsed)
            {
                var availableDate = await _context.AvailableDates
                    .Include(d => d.TimeSlots)
                    .FirstOrDefaultAsync(d => d.Date.Date == date.Date);

                if (availableDate != null)
                {
                    _context.TimeSlots.RemoveRange(availableDate.TimeSlots);
                    _context.AvailableDates.Remove(availableDate);
                }
            }
// save changes
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<User?> GetCaregiverByIdAsync(int CaregiverId)
        {
            return await _context.Users.FindAsync(CaregiverId);
        }
// add admin notification
        public async Task AddAdminNotificationAsync(string message)
        {
            _context.AdminNotifications.Add(new AdminNotification
            {
                Message = message,
                CreatedAt = DateTime.Now,
                IsRead = false
            });
            await _context.SaveChangesAsync();
        }
    }
}
