using System;
using System.Collections.Generic;
using HomeCare.Api.Models;

namespace HomeCare.Api.DTO.User
{
    // DTO used when creating or editing a booking from the frontend
    public class CreateBookingDto
    {
        // Date selected by the user
        public required DateTime SelectedDate { get; set; }

        // Selected time slot ID
        public int TimeSlotId { get; set; } = 0;

        // Selected service category ID
        public int CategoryId { get; set; } = 0;

        // Optional notes provided by the user
        public string? Notes { get; set; }

        // Data returned to help populate dropdowns / lists
        public List<AvailableDate> AvailableDates { get; set; } = new();
        public List<Category> Categories { get; set; } = new();

        // Used when editing an existing booking
        public int BookingId { get; set; }
    }
}
