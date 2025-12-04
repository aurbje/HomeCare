using System;
using System.ComponentModel.DataAnnotations;

// DTO for booking requests made by users
namespace HomeCare.Api.DTO.User
{
    public class BookingRequestDto
    {
        public DateTime SelectedDate { get; set; }

        [Required(ErrorMessage = "Vennligst velg et tidspunkt.")] // "Please select a time slot."
        public int? TimeSlotId { get; set; }

        [Required(ErrorMessage = "Vennligst velg en kategori.")] // "Please select a category."
        public int CategoryId { get; set; }

        public string? Notes { get; set; }

        [Required(ErrorMessage = "Vennligst velg en ansatt.")] // "Please select a caregiver."
        public int? SelectedCaregiverId { get; set; }

        // 0 for new booking, >0 for editing existing
        public int BookingId { get; set; }
    }
}