// only the dates specified on the Booking page can be selected
public class AvailableDate
{
    public int Id { get; set; }
    public DateTime Date { get; set; } // "2025-11-18"
    public ICollection<TimeSlot> TimeSlots { get; set; } = new List<TimeSlot>(); // navigation property

}