using System;
using System.Collections.Generic;
using HomeCare.Models;

namespace HomeCare.ViewModels
{
    // view model used when creating or editing a booking
    public class BookingViewModel
    {
        // date selected by the user
        public required DateTime SelectedDate { get; set; }

        // time slot id selected
        public int TimeSlotId { get; set; } = 0;

        // selected category id
        public int CategoryId { get; set; } = 0;

        // extra notes the user can write
        public string? Notes { get; set; }

        // data we show in the dropdowns / lists
        public List<AvailableDate> AvailableDates { get; set; } = new();
        public List<Category> Categories { get; set; } = new();

        // used when editing an existing appointment
        public int AppointmentId { get; set; }
    }
}