using Microsoft.EntityFrameworkCore;
using HomeCare.Models;

namespace HomeCare.Data
{
    public class HomeCareDbContext : DbContext
    {
        // Konstruktør som lar dependency injection gi oss konfigurasjonen fra appsettings.json
        public HomeCareDbContext(DbContextOptions<HomeCareDbContext> options)
            : base(options)
        {
        }

        // === TABELLER (DB Sets) ===
        public DbSet<User> Users { get; set; } = null!;
        public DbSet<Reminder> Reminders { get; set; } = null!;
        public DbSet<Appointment> Appointments { get; set; } = null!;
        public DbSet<Service> Services { get; set; } = null!;
        public DbSet<Booking> Bookings { get; set; } = null!;

        // Eventuell tilpasning av tabellnavn eller relasjoner
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // Eksempel: gi tabellene enklere navn
            modelBuilder.Entity<User>().ToTable("Users");
            modelBuilder.Entity<Reminder>().ToTable("Reminders");
            modelBuilder.Entity<Appointment>().ToTable("Appointments");
            modelBuilder.Entity<Service>().ToTable("Services");
            modelBuilder.Entity<Booking>().ToTable("Bookings");
        }
    }
}
