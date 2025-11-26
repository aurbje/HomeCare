using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using HomeCare.Api.Models;

namespace HomeCare.Api.Data;

public class AppDbContext : IdentityDbContext<ApplicationUser>
{
    public AppDbContext(DbContextOptions<AppDbContext> options)
        : base(options)
    {
    }

    // domain models
    public DbSet<Appointment> Appointments => Set<Appointment>();
    public DbSet<AvailableDate> AvailableDates => Set<AvailableDate>();
    public DbSet<TimeSlot> TimeSlots => Set<TimeSlot>();
    public DbSet<Category> Categories => Set<Category>();


    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        //
        // RELATIONSHIPS
        //

        // appointment -> user
        modelBuilder.Entity<Appointment>()
            .HasOne(a => a.User)
            .WithMany()
            .HasForeignKey(a => a.UserId)
            .OnDelete(DeleteBehavior.Cascade);

        // appointment -> caregiver (optional)
        modelBuilder.Entity<Appointment>()
            .HasOne(a => a.Caregiver)
            .WithMany()
            .HasForeignKey(a => a.CaregiverId)
            .OnDelete(DeleteBehavior.SetNull);

        //
        // SEED DATA
        //

        modelBuilder.Entity<AvailableDate>().HasData(
            new AvailableDate { Id = 1, Date = new DateTime(2025, 12, 15) },
            new AvailableDate { Id = 2, Date = new DateTime(2025, 12, 16) },
            new AvailableDate { Id = 3, Date = new DateTime(2025, 12, 17) }
        );

        modelBuilder.Entity<TimeSlot>().HasData(
            new TimeSlot { Id = 1, Slot = "09:00-10:00", AvailableDateId = 1, IsBooked = false },
            new TimeSlot { Id = 2, Slot = "10:00-11:00", AvailableDateId = 1, IsBooked = false },
            new TimeSlot { Id = 3, Slot = "11:00-12:00", AvailableDateId = 1, IsBooked = false },
            new TimeSlot { Id = 4, Slot = "13:00-14:00", AvailableDateId = 1, IsBooked = false },
            new TimeSlot { Id = 5, Slot = "14:00-15:00", AvailableDateId = 2, IsBooked = false },
            new TimeSlot { Id = 6, Slot = "09:00-10:00", AvailableDateId = 3, IsBooked = false },
            new TimeSlot { Id = 7, Slot = "10:00-11:00", AvailableDateId = 3, IsBooked = false }
        );

        modelBuilder.Entity<Category>().HasData(
            new Category { Id = 1, Name = "Cleaning" },
            new Category { Id = 2, Name = "Nursing" },
            new Category { Id = 3, Name = "Cooking" },
            new Category { Id = 4, Name = "Other" }
        );
    }
}
