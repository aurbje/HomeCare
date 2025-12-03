using Microsoft.EntityFrameworkCore;
using HomeCare.Api.Models;

namespace HomeCare.Api.Data
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

        // === USERS ===
        public DbSet<User> Users { get; set; }

        // === APPOINTMENTS ===
        public DbSet<Booking> Bookings { get; set; }
        public DbSet<AvailableDate> AvailableDates { get; set; }
        public DbSet<TimeSlot> TimeSlots { get; set; }
        public DbSet<Category> Categories { get; set; }
        public DbSet<BookingOption> BookingOptions { get; set; }

        // === Caregiver ===
        public DbSet<CaregiverAvailability> CaregiverAvailabilities { get; set; }

        // === CLIENT ===
        public DbSet<Reminder> Reminders { get; set; }

        // === ADMIN ===
        public DbSet<AdminNotification> AdminNotifications { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            // === INDEXES ===
            modelBuilder.Entity<User>()
                .HasIndex(u => u.Email)
                .IsUnique();

            modelBuilder.Entity<Booking>()
                .HasIndex(a => a.DateTime);

            modelBuilder.Entity<Booking>()
                .HasIndex(a => a.UserId);

            modelBuilder.Entity<Booking>()
                .HasIndex(a => a.CaregiverId);

            modelBuilder.Entity<CaregiverAvailability>()
                .HasIndex(pa => pa.Date);

            modelBuilder.Entity<CaregiverAvailability>()
                .HasIndex(pa => new { pa.CaregiverId, pa.Date })
                .IsUnique();

            modelBuilder.Entity<AvailableDate>()
                .HasIndex(ad => ad.Date)
                .IsUnique();

            // === RELATIONSHIPS ===
            modelBuilder.Entity<Booking>()
                .HasOne(a => a.User)
                .WithMany()
                .HasForeignKey(a => a.UserId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<Booking>()
                .HasOne(a => a.Caregiver)
                .WithMany()
                .HasForeignKey(a => a.CaregiverId)
                .OnDelete(DeleteBehavior.SetNull);

            modelBuilder.Entity<Reminder>()
                .HasOne(r => r.User)
                .WithMany()
                .HasForeignKey(r => r.UserId)
                .OnDelete(DeleteBehavior.Cascade);

            // === SEED DATA ===
            modelBuilder.Entity<Category>().HasData(
                new Category { Id = 1, Name = "Vask" },
                new Category { Id = 2, Name = "Omsorg" },
                new Category { Id = 3, Name = "Matstell" },
                new Category { Id = 4, Name = "Annet" }
            );
        }
    }
}
