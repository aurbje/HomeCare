using HomeCare.Data;
public class BookingService
{
    private readonly AppDbContext _context;

    public BookingService(AppDbContext context)
    {
        _context = context;
    }

    //  between personnel's available dates and client user's calender
    public bool IsBookingTimeAvailable(DateTime date, string timeString)
    {
        if (!TimeSpan.TryParse(timeString, out var time)) return false;

        var availability = _context.PersonnelAvailabilities
            .FirstOrDefault(a => a.Date.Date == date.Date);

        if (availability == null) return false;

        return time >= availability.StartTime && time < availability.EndTime;
    }
}
