using Microsoft.EntityFrameworkCore;
using HomeCare.Models;

namespace HomeCare.Data
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

        // === USER DATA ===
        public DbSet<User> Users { get; set; }

        // === BOOKING DATA ===
        public DbSet<BookingOption> BookingOptions { get; set; }
        public DbSet<Appointment> Appointments { get; set; }
        public DbSet<AvailableDate> AvailableDates { get; set; }
        public DbSet<TimeSlot> TimeSlots { get; set; }
        public DbSet<Category> Categories { get; set; }

        // === DATABASE SEEDING ===
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            // Available Dates
            modelBuilder.Entity<AvailableDate>().HasData(
                new AvailableDate { Id = 1, Date = new DateTime(2025, 12, 15) },
                new AvailableDate { Id = 2, Date = new DateTime(2025, 12, 16) },
                new AvailableDate { Id = 3, Date = new DateTime(2025, 12, 17) }
            );

            // Time Slots
            modelBuilder.Entity<TimeSlot>().HasData(
                new TimeSlot { Id = 1, Slot = "09:00-10:00", AvailableDateId = 1, IsBooked = false },
                new TimeSlot { Id = 2, Slot = "10:00-11:00", AvailableDateId = 1, IsBooked = false },
                new TimeSlot { Id = 3, Slot = "11:00-12:00", AvailableDateId = 1, IsBooked = false },
                new TimeSlot { Id = 4, Slot = "13:00-14:00", AvailableDateId = 1, IsBooked = false },
                new TimeSlot { Id = 5, Slot = "14:00-15:00", AvailableDateId = 2, IsBooked = false },
                new TimeSlot { Id = 6, Slot = "09:00-10:00", AvailableDateId = 3, IsBooked = false },
                new TimeSlot { Id = 7, Slot = "10:00-11:00", AvailableDateId = 3, IsBooked = false }
            );

            // Categories
            modelBuilder.Entity<Category>().HasData(
                new Category { Id = 1, Name = "Cleaning" },
                new Category { Id = 2, Name = "Nursing" },
                new Category { Id = 3, Name = "Cooking" },
                new Category { Id = 4, Name = "OTHER" }
            );
        }
    }
}
