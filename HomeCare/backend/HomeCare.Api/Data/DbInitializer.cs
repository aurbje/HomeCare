using System.Linq;
namespace HomeCare.Api.Data
{
    public static class DbInitializer
    {
        public static void Seed(AppDbContext context)
        {
            // make sure the database exists
            context.Database.EnsureCreated();

            // if there is already data, we skip seeding here
            if (context.AvailableDates.Any())
            {
                return;
            }

            // seeding is handled in onmodelcreating
        }
    }
}

