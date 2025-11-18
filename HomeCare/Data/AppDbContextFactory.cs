// design
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using HomeCare.Models;
using HomeCare.Data;

namespace HomeCare.Data
{
    public class AppDbContextFactory : IDesignTimeDbContextFactory<AppDbContext>
    {
        public AppDbContext CreateDbContext(string[] args)
        {
            var optionsBuilder = new DbContextOptionsBuilder<AppDbContext>();
            optionsBuilder.UseSqlite("Data Source=HomeCare.db"); // name database

            return new AppDbContext(optionsBuilder.Options);
        }

        public required DbSet<BookingOption> BookingOptions { get; set; }
    }
}