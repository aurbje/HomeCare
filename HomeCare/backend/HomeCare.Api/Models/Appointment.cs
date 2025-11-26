using HomeCare.Api.Models;

namespace HomeCare.Api.Models;

public class Appointment
{
    public int Id { get; set; }

    public DateTime DateTime { get; set; }

    // Relationship to timeslot
    public int TimeSlotId { get; set; }
    public TimeSlot TimeSlot { get; set; } = null!;

    // Relationship to category
    public int CategoryId { get; set; }
    public Category Category { get; set; } = null!;

    public string? Notes { get; set; }

    // The user (client) who booked the appointment
    public string UserId { get; set; } = null!;
    public ApplicationUser User { get; set; } = null!;

    // The assigned caregiver
    public string? CaregiverId { get; set; }       // <-- THE ONE YOU WERE MISSING
    public ApplicationUser? Caregiver { get; set; }

    // Status tracking
    public string Status { get; set; } = "Pending"; 
}
