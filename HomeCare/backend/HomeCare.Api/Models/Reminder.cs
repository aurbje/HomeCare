namespace HomeCare.Api.Models
{
    // represents a reminder set by a user
    public class Reminder
    {
        public int Id { get; set; }
        public int UserId { get; set; }
        public DateTime Time { get; set; }
        public string Message { get; set; } = string.Empty;
        public bool IsCompleted { get; set; }

        // Navigation property
        public User? User { get; set; }
    }
}