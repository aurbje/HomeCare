using HomeCare.Models;


namespace HomeCare.Data
{
    public static class DbInitializer
    {
        public static void Seed(AppDbContext context)
        {
            // Skip if date already exists
            if (context.AvailableDates.Any()) return;

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
                new TimeSlot { Slot = "09:00–10:00", AvailableDateId = 1 },
                new TimeSlot { Slot = "10:00–11:00", AvailableDateId = 1 },
                new TimeSlot { Slot = "11:00–12:00", AvailableDateId = 2 },
                new TimeSlot { Slot = "13:00–14:00", AvailableDateId = 3 },
                new TimeSlot { Slot = "15:00–16:00", AvailableDateId = 3 }
            };
            context.TimeSlots.AddRange(slots);
            context.SaveChanges();

            // Category
            var categories = new List<Category>
            {
                new Category { Name = "Cleaning" },
                new Category { Name = "Nursing" },
                new Category { Name = "Cooking" },
                new Category { Name = "Other" }
            };
            context.Categories.AddRange(categories);
            context.SaveChanges();
        }
    }
}