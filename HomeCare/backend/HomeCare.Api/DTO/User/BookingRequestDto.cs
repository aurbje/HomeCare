using System;
using System.ComponentModel.DataAnnotations;

namespace HomeCare.Api.DTO.User
{
    public class BookingRequestDto
    {
        public DateTime SelectedDate { get; set; }

        [Required(ErrorMessage = "Vennligst velg et tidspunkt.")]
        public int? TimeSlotId { get; set; }

        [Required(ErrorMessage = "Vennligst velg en kategori.")]
        public int CategoryId { get; set; }

        public string? Notes { get; set; }

        [Required(ErrorMessage = "Vennligst velg en ansatt.")]
        public int? SelectedCaregiverId { get; set; }

        // 0 for new booking, >0 for editing existing
        public int BookingId { get; set; }
    }
}