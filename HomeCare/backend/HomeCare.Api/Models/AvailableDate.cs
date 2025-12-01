using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace HomeCare.Api.Models
{
    // represents a day that can have available time slots for booking
    public class AvailableDate
    {
        public int Id { get; set; }

        // the actual date the user can pick
        [Required]
        public DateTime Date { get; set; }

        // all time slots connected to this date
        public ICollection<TimeSlot> TimeSlots { get; set; } = new List<TimeSlot>();

        // optional: which caregiver owns this availability
        public string? CaregiverId { get; set; }
       
    }
}
