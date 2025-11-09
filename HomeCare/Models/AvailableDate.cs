// Only the dates specified on the Booking page can be selected
public class AvailableDate
{
    public int Id { get; set; }
    public DateTime Date { get; set; }
    public ICollection<TimeSlot> TimeSlots { get; set; } = new List<TimeSlot>();

    public int RemainingSlots { get; set; }     // added after the error CS1061 — Missing method or property : AvailableDate does not contain a definition for RemainingSlots


}