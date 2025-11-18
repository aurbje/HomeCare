namespace HomeCare.Models
{
    public class BookingOption
    {
        public int Id { get; set; }
        public DateTime AvailableDate { get; set; } // date
        public string TimeSlot { get; set; } = string.Empty; // e.g. "Morning"
        public string Category { get; set; } = string.Empty; // eg. "Medication"
    }
}