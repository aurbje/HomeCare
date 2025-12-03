namespace HomeCare.Api.DTO
{
    public class AvailableDateDto
    {
        public int Id { get; set; }
        public DateTime Date { get; set; }
        public List<TimeSlotDto> TimeSlots { get; set; } = new();
    }
}
