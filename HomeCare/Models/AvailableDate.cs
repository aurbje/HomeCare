// Only the dates specified on the Booking page can be selected
public class AvailableDate
{
    public int Id { get; set; }
    public DateTime Date { get; set; }
    public ICollection<TimeSlot> TimeSlots { get; set; } = new List<TimeSlot>();

}
