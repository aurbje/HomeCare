namespace HomeCare.ViewModels
{
    public class BookingViewModel // ViewModel for booking an appointment
    {
        public required DateTime SelectedDate { get; set; } // date selected

        public int TimeSlotId { get; set; } = 0; // time slot selected

        public int CategoryId { get; set; } = 0;
        public string? Notes { get; set; } // additional notes to category


        // to display
        public List<AvailableDate> AvailableDates { get; set; } = new(); // available dates
        public List<Category> Categories { get; set; } = new();

        public int AppointmentId { get; set; } // appointment identifier
    }
}