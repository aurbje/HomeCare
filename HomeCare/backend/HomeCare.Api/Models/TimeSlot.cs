namespace HomeCare.Api.Models
{
    /// <summary>
    /// Represents a time slot within an available date (e.g., "09:00-10:00").
    /// </summary>
    public class TimeSlot
    {
        public int Id { get; set; }
        public string Slot { get; set; } = string.Empty; // "09:00-10:00"

        public int AvailableDateId { get; set; }
        public AvailableDate? AvailableDate { get; set; }

        public bool IsBooked { get; set; } = false;
    }
}