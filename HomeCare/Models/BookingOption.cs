using System;
using System.ComponentModel.DataAnnotations;

namespace HomeCare.Models
{
    // simple option used for showing quick booking choices
    public class BookingOption
    {
        public int Id { get; set; }

        // date this option belongs to
        [Required]
        public DateTime AvailableDate { get; set; }

        // name of the time slot ("Morning","Evening" etc.)
        [Required]
        public string TimeSlot { get; set; } = string.Empty;

        // type of service offered for this option
        [Required]
        public string Category { get; set; } = string.Empty;
    }
}
