namespace HomeCare.Api.DTOs;

public class CreateBookingDto
{
    public DateTime SelectedDate { get; set; }

    // chosen time slot (ex: "09:00–10:00")
    public int TimeSlotId { get; set; }

    // category (Cleaning / Nursing / Cooking / Other)
    public int CategoryId { get; set; }

    // optional notes (required if Category = Other)
    public string? Notes { get; set; }
}
