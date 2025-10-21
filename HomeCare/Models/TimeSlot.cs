public class TimeSlot
{
    public int Id { get; set; }
    public string Slot { get; set; } = string.Empty; // "09:00–10:00"

    public int AvailableDateId { get; set; }
    public AvailableDate? AvailableDate { get; set; } // nullable

    public bool IsBooked { get; set; } = false;
}
