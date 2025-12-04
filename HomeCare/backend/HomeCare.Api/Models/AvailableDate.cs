namespace HomeCare.Api.Models
{

    /// Represents a date that is available for booking bookings.

    public class AvailableDate
    {
        public int Id { get; set; }
        public DateTime Date { get; set; }
        public ICollection<TimeSlot> TimeSlots { get; set; } = new List<TimeSlot>();
        public int RemainingSlots { get; set; }
    }
}