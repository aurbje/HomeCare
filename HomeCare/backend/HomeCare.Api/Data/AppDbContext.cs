using System;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using HomeCare.Api.Models;

namespace HomeCare.Api.Data
{
    // main db context for both identity and app data
    public class AppDbContext : IdentityDbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

        // app users (custom user table so we don't collide with identity users table)
        public DbSet<User> AppUsers { get; set; } = default!;

        // visits and tasks for caregivers
        public DbSet<Visit> Visits { get; set; } = default!;

        // booking related data
        public DbSet<Booking> Bookings { get; set; } = default!;
        public DbSet<BookingOption> BookingOptions { get; set; } = default!;
        public DbSet<AvailableDate> AvailableDates { get; set; } = default!;
        public DbSet<TimeSlot> TimeSlots { get; set; } = default!;
        public DbSet<Category> Categories { get; set; } = default!;

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            // still calling base so identity config is applied
            base.OnModelCreating(modelBuilder);

            // seed user data
            modelBuilder.Entity<User>().HasData(
                new User 
                    {
                        Id = 1, 
                        FullName = "Admin", 
                        Email = "admin@oslomet.no", 
                        PasswordHash = "$2a$11$qvtrhsyDZ1MR9RdSIv11FeGSMZxOLUC4JizZMyBone4hRc1aA.WGm", // Admin123!
                        TlfNumber = "12345678", 
                        Address = "admingata 1", 
                        Role = "Admin"
                    },

                    new User
                    {
                        Id = 2, 
                        FullName = "Caregiver", 
                        Email = "caregiver@oslomet.no", 
                        PasswordHash = "$2a$11$/sLVDjSby9tYKkzocka7NOPOzHaJXsJK7naLEqkmfxw79hzPY07d.", // Caregiver123!
                        TlfNumber = "87654321", 
                        Address = "caregivergata 1", 
                        Role = "Caregiver"
                    },

                    new User
                    {
                        Id = 3, 
                        FullName = "User", 
                        Email = "user@oslomet.no", 
                        PasswordHash = "$2a$11$f73yjHkIJFhi05E.7lFjgOk7d2nlmDYKw3b7DIfKThnaBV3BSp3SK", // User123!
                        TlfNumber = "12348765", 
                        Address = "Usergata 1", 
                        Role = "User"
                    }

            );

            // available Dates
            modelBuilder.Entity<AvailableDate>().HasData(
                new AvailableDate { Id = 1, Date = new DateTime(2025, 12, 15) },
                new AvailableDate { Id = 2, Date = new DateTime(2025, 12, 16) },
                new AvailableDate { Id = 3, Date = new DateTime(2025, 12, 17) }
            );

            // seeding time slots for those dates
            modelBuilder.Entity<TimeSlot>().HasData(
                new TimeSlot { Id = 1, Slot = "09:00-10:00", AvailableDateId = 1, IsBooked = false },
                new TimeSlot { Id = 2, Slot = "10:00-11:00", AvailableDateId = 1, IsBooked = false },
                new TimeSlot { Id = 3, Slot = "11:00-12:00", AvailableDateId = 1, IsBooked = false },
                new TimeSlot { Id = 4, Slot = "13:00-14:00", AvailableDateId = 1, IsBooked = false },
                new TimeSlot { Id = 5, Slot = "14:00-15:00", AvailableDateId = 2, IsBooked = false },
                new TimeSlot { Id = 6, Slot = "09:00-10:00", AvailableDateId = 3, IsBooked = false },
                new TimeSlot { Id = 7, Slot = "10:00-11:00", AvailableDateId = 3, IsBooked = false }
            );

            // seeding some basic categories so users have something to pick
            modelBuilder.Entity<Category>().HasData(
                new Category { Id = 1, Name = "Cleaning" },
                new Category { Id = 2, Name = "Nursing" },
                new Category { Id = 3, Name = "Cooking" },
                new Category { Id = 4, Name = "OTHER" }
            );
        }
    }
}