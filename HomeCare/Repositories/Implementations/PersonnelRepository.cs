using HomeCare.Data; // AppDbContext
using HomeCare.ViewModels.Personnel;
using HomeCare.Models;
using Microsoft.EntityFrameworkCore;
using System.Linq;



public class PersonnelRepository : IPersonnelRepository
{
    private readonly AppDbContext _context;

    public PersonnelRepository(AppDbContext context)
    {
        _context = context;
    }

    public async Task<PersonnelDashboardViewModel> GetDashboardViewModelAsync(int personnelId)
    {
        var user = await _context.Users.FindAsync(personnelId);
        if (user == null)
        {
            throw new Exception($"User with ID {personnelId} not found."); // <!-- Added null check to prevent crash if user is missing -->
        }

        var availabilities = await _context.PersonnelAvailabilities
            .Where(appointment => appointment.PersonnelId == personnelId)
            .Select(a => a.Date)
            .ToListAsync();

        var todayVisits = await _context.PersonnelAppointments
            .Include(a => a.Client) // <!-- Added Include to load related Client entity -->
            .Include(a => a.Category) // <!-- Added Include to load related Category entity -->
            .Where(appointment => appointment.PersonnelId == personnelId && appointment.DateTime.Date == DateTime.Today)
            .Select(appointment => new VisitInfo
            {

                Time = appointment.DateTime,
                ClientName = appointment.Client.FullName, // <!-- Direct access is now safe -->
                Address = appointment.Client.Address,
                Phone = appointment.Client.TlfNumber,
                Tasks = new List<string> { appointment.Category.Name }
            })
            .ToListAsync();

        return new PersonnelDashboardViewModel
        {
            PersonnelName = user.FullName,
            AvailableDates = availabilities,
            TodayVisits = todayVisits,
            CalendarEvents = new List<CalendarEvent>()  // Later
        };
    }

    public async Task AddAvailabilityAsync(int personnelId, DateTime date)
    {
        Console.WriteLine($"Availability registered: Personnel ID {personnelId} is available on {date:yyyy-MM-dd}");

        // Check if the date is already registered for this personnel
        bool alreadyExists = await _context.PersonnelAvailabilities
            .AnyAsync(a => a.PersonnelId == personnelId && a.Date.Date == date.Date);

        if (alreadyExists)
        {
            return; // you cannot register same date twice !
        }

        // Register PersonnelAvailability
        _context.PersonnelAvailabilities.Add(new PersonnelAvailability
        {
            PersonnelId = personnelId,
            Date = date
        });

        // Also register AvailableDate if not already present
        var existingDate = await _context.AvailableDates
            .FirstOrDefaultAsync(d => d.Date.Date == date.Date);

        if (existingDate == null)
        {
            var newDate = new AvailableDate { Date = date.Date };
            _context.AvailableDates.Add(newDate); // <!-- Added AvailableDate creation -->

            // Add default TimeSlots for the new AvailableDate
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
                }); // <!-- Added TimeSlot creation -->
            }
        }

        await _context.SaveChangesAsync(); // <!-- Save all changes including AvailableDate and TimeSlots -->
    }

    public async Task DeleteAvailabilityAsync(int personnelId, DateTime date)
    {
        var availability = await _context.PersonnelAvailabilities
.FirstOrDefaultAsync(a => a.PersonnelId == personnelId && a.Date.Date == date.Date);


        if (availability != null)
        {
            _context.PersonnelAvailabilities.Remove(availability);
            await _context.SaveChangesAsync();
        }
    }

}
