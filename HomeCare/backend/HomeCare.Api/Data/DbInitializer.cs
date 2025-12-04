using HomeCare.Api.Models;


namespace HomeCare.Api.Data
{
    public static class DbInitializer
    {
        public static void Seed(AppDbContext context)
        {

            if (!context.Categories.Any())
            {
                // Category
                var categories = new List<Category>
            {
                new Category { Name = "Vask" },
                new Category { Name = "Omsorg" },
                new Category { Name = "Mating" },
                new Category { Name = "Medisinering" },
                new Category { Name = "Stell" },
                new Category { Name = "Annet" }
            };
                context.Categories.AddRange(categories);
                context.SaveChanges();
            }
        }

    }
}