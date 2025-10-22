using System;

using System.ComponentModel.DataAnnotations;


namespace HomeCare.Models
{
    public class Appointment
    {
        public int Id { get; set; }
        public required DateTime DateTime { get; set; } // AvailableDate.Date + TimeSlot.Slot
        public string? Notes { get; set; }
        public int TimeSlotId { get; set; }
        public TimeSlot TimeSlot { get; set; } = null!; // never null

        public int CategoryId { get; set; }
        public Category Category { get; set; } = null!; // never null

        // =======
        //         [Key]
        //         public int Id { get; set; }

        //         [Required(ErrorMessage = "Dato og tid må fylles ut")]
        //         public DateTime DateTime { get; set; } // Kombinasjon av AvailableDate.Date + TimeSlot.Slot

        //         [Required(ErrorMessage = "Tjenestetype må fylles ut")]
        //         public string ServiceType { get; set; } = string.Empty;

        //         public string? Notes { get; set; }

        //         // Relasjoner til tid og kategori
        //         [Required]
        //         public int TimeSlotId { get; set; }
        //         public TimeSlot TimeSlot { get; set; } = null!; // aldri null

        //         [Required]
        //         public int CategoryId { get; set; }
        //         public Category Category { get; set; } = null!; // aldri null
        // >>>>>>> main
    }
}
