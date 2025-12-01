using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;

namespace HomeCare.Api.Data
{
    public class AppDbContextFactory : IDesignTimeDbContextFactory<AppDbContext>
    {
        public AppDbContext CreateDbContext(string[] args)
        {
            // creating simple options builder just for design-time db creation
            var optionsBuilder = new DbContextOptionsBuilder<AppDbContext>();

            // using sqlite as the database, since the app also uses this normally
            optionsBuilder.UseSqlite("Data Source=HomeCare.db");

            // returning a context instance so EF can run migrations
            return new AppDbContext(optionsBuilder.Options);
        }
    }
}