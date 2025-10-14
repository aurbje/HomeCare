public class BookingViewModel
{
    public required DateTime SelectedDate { get; set; } = DateTime.Today;
    // public String SelectedDate { get; set; }

    public required string Time { get; set; } = "08:00";

    public required string Category { get; set; } = "Shopping";

    public string? CustomCategory { get; set; } = null;
    public string? Notes { get; set; }
}
