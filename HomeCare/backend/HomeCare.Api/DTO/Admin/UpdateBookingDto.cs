using System.ComponentModel.DataAnnotations;

namespace HomeCare.Api.DTO.Admin
{
    public class UpdateBookingDto
    {
        [Required]
        public int UserId { get; set; }

        public int? CaregiverId { get; set; }

        [Required]
        public DateTime DateTime { get; set; }

        [Required]
        public int TimeSlotId { get; set; }

        [Required]
        public int CategoryId { get; set; }

        public string? Notes { get; set; }
    }
}