using System;

namespace HomeCare.Api.Models
{
    public class AdminNotification
    {
        public int Id { get; set; } // Primary key
        public string Message { get; set; } = string.Empty; // Notification content (in Norwegian)
        public DateTime CreatedAt { get; set; } // Timestamp
        public bool IsRead { get; set; } = false; // Whether admin has read it
    }
}
