using System.ComponentModel.DataAnnotations;

namespace HomeCare.Api.DTO.Admin
{
    public class UpdateBookingDto // DTO for updating a booking
    {
        [Required]
        public int UserId { get; set; } // ID of the user making the booking

        public int? CaregiverId { get; set; } // ID of the caregiver assigned to the booking (optional)

        [Required]
        public DateTime DateTime { get; set; } // date and time of the booking

        [Required]
        public int TimeSlotId { get; set; } // ID of the time slot for the booking

        [Required]
        public int CategoryId { get; set; } // ID of the service category for the booking

        public string? Notes { get; set; } // additional notes for the booking (optional)
    }
}