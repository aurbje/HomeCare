/* ============================================================
 * ORIGINAL GROUP CODE - Commented out
 * Replaced with merged version in Reminder.cs
 * This file kept for reference purposes
 * ============================================================

using System.ComponentModel.DataAnnotations;

namespace HomeCare.Api.Models
{
    // small reminder item shown on the user dashboard
    public class Reminder
    {
        // time the reminder should pop up ("08:00")
        [Required]
        public string Time { get; set; } = string.Empty;

        // optional message describing the reminder
        public string? Message { get; set; }
    }
}

*/