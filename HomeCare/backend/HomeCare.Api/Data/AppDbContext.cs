using Microsoft.EntityFrameworkCore;
using HomeCare.Api.Models;

namespace HomeCare.Api.Data
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

        // USERS
        public DbSet<User> Users { get; set; }

        // APPOINTMENTS
        public DbSet<Booking> Bookings { get; set; }
        public DbSet<AvailableDate> AvailableDates { get; set; }
        public DbSet<TimeSlot> TimeSlots { get; set; }
        public DbSet<Category> Categories { get; set; }
        public DbSet<BookingOption> BookingOptions { get; set; }

        // CAREGIVER
        public DbSet<CaregiverAvailability> CaregiverAvailabilities { get; set; }

        // CLIENT
        public DbSet<Reminder> Reminders { get; set; }

        // ADMIN
        public DbSet<AdminNotification> AdminNotifications { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            // INDEXES
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

            // RELATIONSHIPS
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

            // SEED USER DATA
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
            // SEED CATEGORY DATA
            modelBuilder.Entity<Category>().HasData(
                new Category { Id = 1, Name = "Vask" },
                new Category { Id = 2, Name = "Omsorg" },
                new Category { Id = 3, Name = "Mating" },
                new Category { Id = 4, Name = "Medisinering" },
                new Category { Id = 5, Name = "Stell" },
                new Category { Id = 6, Name = "Annet" }
            );
        }
    }
}
