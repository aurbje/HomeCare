using HomeCare.Data;
using HomeCare.Models;
using Microsoft.EntityFrameworkCore;

namespace HomeCare.Services
{
    public class PersonnelAvailabilityService
    {
        private readonly AppDbContext _context;

        public PersonnelAvailabilityService(AppDbContext context)
        {
            _context = context;
        }

        public async Task RegisterAvailabilityAsync(int personnelId, DateTime date)
        {
            // Check if already registered
            var exists = await _context.PersonnelAvailabilities
                .AnyAsync(a => a.PersonnelId == personnelId && a.Date.Date == date.Date);

            if (exists)
                return;

            // Create PersonnelAvailability
            var availability = new PersonnelAvailability
            {
                PersonnelId = personnelId,
                Date = date.Date,
                StartTime = new TimeSpan(8, 0, 0),  // Fixed start time
                EndTime = new TimeSpan(15, 0, 0)    // Fixed end time
            };
            _context.PersonnelAvailabilities.Add(availability);

            // Create AvailableDate for client booking
            var availableDate = new AvailableDate
            {
                Date = date.Date,
                RemainingSlots = 7 // 08:00–15:00 → 7 slots
            };
            _context.AvailableDates.Add(availableDate);

            // Create hourly TimeSlots
            for (int hour = 8; hour < 15; hour++)
            {
                var slot = new TimeSlot
                {
                    AvailableDate = availableDate,
                    Slot = $"{hour:00}:00 - {hour + 1:00}:00",
                    IsBooked = false
                };
                _context.TimeSlots.Add(slot);
            }

            await _context.SaveChangesAsync();
        }
    }
}
