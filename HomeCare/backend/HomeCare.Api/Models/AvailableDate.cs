namespace HomeCare.Api.Models
{
    /// <summary>
    /// Represents a date that is available for booking bookings.
    /// </summary>
    public class AvailableDate
    {
        public int Id { get; set; }
        public DateTime Date { get; set; }
        public ICollection<TimeSlot> TimeSlots { get; set; } = new List<TimeSlot>();
        public int RemainingSlots { get; set; }
    }
}