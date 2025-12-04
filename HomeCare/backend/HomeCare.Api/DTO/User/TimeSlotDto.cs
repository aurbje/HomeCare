namespace HomeCare.Api.DTO
{
    // DTO for representing a time slot
    public class TimeSlotDto
    {
        public int Id { get; set; }
        public string Slot { get; set; } = string.Empty;
        public bool IsBooked { get; set; }
    }
}
