namespace HomeCare.Models
{
    public class BookingOption
    {
        public int Id { get; set; }
        public DateTime AvailableDate { get; set; }
        public string TimeSlot { get; set; } = string.Empty;
        public string Category { get; set; } = string.Empty;
    }
}