using System;

namespace HomeCare.Api.Models
{
    public class AdminNotification // notification for admin users
    {
        public int Id { get; set; } // primary key
        public string Message { get; set; } = string.Empty; // notification content (in Norwegian)
        public DateTime CreatedAt { get; set; } // timestamp
        public bool IsRead { get; set; } = false; // whether admin has read it
    }
}
