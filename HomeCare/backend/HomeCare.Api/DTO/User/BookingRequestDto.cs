using System.ComponentModel.DataAnnotations;

namespace HomeCare.Api.DTO
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
        public int BookingId { get; set; }
    }
}
