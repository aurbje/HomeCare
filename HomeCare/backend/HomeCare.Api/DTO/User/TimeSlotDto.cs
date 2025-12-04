namespace HomeCare.Api.DTO.User
{
    public class TimeSlotDto // DTO for representing a time slot
    {
        public int Id { get; set; }
        public string Slot { get; set; } = string.Empty;
        public bool IsBooked { get; set; }
    }
}
