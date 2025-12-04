namespace HomeCare.Api.DTO
{
    public class AvailableDateDto // represents a date with available time slots for a user
    {
        public int Id { get; set; }
        public DateTime Date { get; set; }
        public List<TimeSlotDto> TimeSlots { get; set; } = new(); // list of available time slots for the date
    }
}
