using System;
using System.Collections.Generic;
using System.Linq;
using HomeCare.Models;

namespace HomeCare.Data
{
    public static class DbInitializer
    {
        public static void Seed(AppDbContext context)
        {
            // Sørg for at databasen er opprettet
            context.Database.EnsureCreated();

            // Hvis vi allerede har data, ikke seed på nytt
            if (context.AvailableDates.Any())
                return;

            // AvailableDate
            var dates = new List<AvailableDate>
            {
                new AvailableDate { Id = 1, Date = new DateTime(2025, 12, 15) },
                new AvailableDate { Id = 2, Date = new DateTime(2025, 12, 17) },
                new AvailableDate { Id = 3, Date = new DateTime(2025, 12, 20) }
            };
            context.AvailableDates.AddRange(dates);
            context.SaveChanges();

            // TimeSlot
            var slots = new List<TimeSlot>
            {
                new TimeSlot { Id = 1, Slot = "09:00–10:00", AvailableDateId = 1, IsBooked = false },
                new TimeSlot { Id = 2, Slot = "10:00–11:00", AvailableDateId = 1, IsBooked = false },
                new TimeSlot { Id = 3, Slot = "11:00–12:00", AvailableDateId = 2, IsBooked = false },
                new TimeSlot { Id = 4, Slot = "13:00–14:00", AvailableDateId = 3, IsBooked = false },
                new TimeSlot { Id = 5, Slot = "15:00–16:00", AvailableDateId = 3, IsBooked = false }
            };
            context.TimeSlots.AddRange(slots);
            context.SaveChanges();

            // Category
            var categories = new List<Category>
            {
                new Category { Id = 1, Name = "Cleaning" },
                new Category { Id = 2, Name = "Nursing" },
                new Category { Id = 3, Name = "Cooking" },
                new Category { Id = 4, Name = "Other" }
            };
            context.Categories.AddRange(categories);
            context.SaveChanges();
        }
    }
}
