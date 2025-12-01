using System.Linq;
using HomeCare.Api.Models;

namespace HomeCare.Api.Data
{
    // makes sure the db exists
    public static class DbInitializer
    {
        public static void Seed(AppDbContext context)
        {
            // just making sure the database file is created
            context.Database.EnsureCreated();

            if (context.AvailableDates.Any())
            {
                // db already has seed data, so nothing more to do
                return;
            }

        }
    }
}