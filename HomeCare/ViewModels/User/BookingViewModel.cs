namespace HomeCare.ViewModels
{
    public class BookingViewModel
    {
        public required DateTime SelectedDate { get; set; } // = DateTime.Today;

        public int TimeSlotId { get; set; } = 0; //= "08:00-9:00"; <--Booking()POST

        public int CategoryId { get; set; } = 0;
        public string? Notes { get; set; } // If Cateory == "OTHER", required


        // to display
        public List<AvailableDate> AvailableDates { get; set; } = new();
        public List<Category> Categories { get; set; } = new();

        public int AppointmentId { get; set; } // For edit tracking
    }
}