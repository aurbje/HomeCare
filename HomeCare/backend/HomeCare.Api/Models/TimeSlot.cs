using System.ComponentModel.DataAnnotations;

namespace HomeCare.Api.Models
{
    // a single slot of time the user can book ("09:00-10:00")
    public class TimeSlot
    {
        public int Id { get; set; }

        // the time range shown to the user
        [Required]
        public string Slot { get; set; } = string.Empty;

        // date this slot belongs to
        [Required]
        public int AvailableDateId { get; set; }
        public AvailableDate? AvailableDate { get; set; }

        // if someone already booked this slot
        public bool IsBooked { get; set; } = false;
    }
}
