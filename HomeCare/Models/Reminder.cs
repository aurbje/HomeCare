namespace HomeCare.Models
{
    public class Reminder
    {
        public required string Time { get; set; }
        public string? Message { get; set; } // message for the reminder
    }
}