namespace HomeCare.Api.Models
{
    // represents an available date for booking home care services
    public class AvailableDate
    {
        public int Id { get; set; }
        public DateTime Date { get; set; }
        public ICollection<TimeSlot> TimeSlots { get; set; } = new List<TimeSlot>();
        public int RemainingSlots { get; set; }
    }
}