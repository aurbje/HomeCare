using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using HomeCare.Api.Enums;

namespace HomeCare.Api.Models
{
    public class Booking
    {
        public int Id { get; set; }
        public required DateTime DateTime { get; set; }
        public string? Notes { get; set; }
        public BookingStatus Status { get; set; } = BookingStatus.Pending;

        public int TimeSlotId { get; set; }
        public TimeSlot TimeSlot { get; set; } = null!;

        public int CategoryId { get; set; }
        public Category Category { get; set; } = null!;
        public int? CaregiverId { get; set; }
        public User? Caregiver { get; set; }
        public int UserId { get; set; }
        public User User { get; set; } = null!;

        // Alias property for backward compatibility with code using "Client" instead of "User"
        [NotMapped]
        public User? Client
        {
            get => User;
            set => User = value!;
        }
    }
}
