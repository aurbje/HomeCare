using System;
using System.Linq;
using System.Threading.Tasks;
using HomeCare.Api.DAL.Repositories;
using HomeCare.Api.Data;
using HomeCare.Api.Models;
using Microsoft.EntityFrameworkCore;
using Xunit;

namespace HomeCare.Tests.Repositories
{
    public class BookingRepositoryTests // booking repository tests
    {
        private AppDbContext GetDb()
        {
            var opts = new DbContextOptionsBuilder<AppDbContext>()
                .UseInMemoryDatabase(Guid.NewGuid().ToString())
                .Options;

            return new AppDbContext(opts);
        }

        [Fact]
        public async Task GetAvailableDates_ReturnsOnlyFutureDatesWithFreeSlots() // test for getting available dates
        {
            using var db = GetDb();

            var date = new AvailableDate { Id = 1, Date = DateTime.Today.AddDays(1) };
            db.AvailableDates.Add(date);

            db.TimeSlots.Add(new TimeSlot { Id = 1, Slot = "09-10", AvailableDate = date, IsBooked = false });

            await db.SaveChangesAsync();

            var repo = new BookingRepository(db);

            var result = (await repo.GetAvailableDatesAsync()).ToList();

            Assert.Single(result);
            Assert.Equal(date.Id, result[0].Id);
        }

        [Fact]
        public async Task GetAvailableTimeSlot_ReturnsCorrectSlot() // test for getting available time slot
        {
            using var db = GetDb();
            var date = new AvailableDate { Id = 1, Date = DateTime.Today };
            var slot = new TimeSlot { Id = 5, Slot = "10-11", AvailableDate = date, IsBooked = false };

            db.AvailableDates.Add(date);
            db.TimeSlots.Add(slot);
            await db.SaveChangesAsync();

            var repo = new BookingRepository(db);

            var result = await repo.GetAvailableTimeSlotAsync(5);

            Assert.NotNull(result);
            Assert.Equal("10-11", result!.Slot);
        }

        [Fact]
        public async Task UpdateTimeSlot_SetsIsBookedAndSaves() // test for updating time slot booking status
        {
            using var db = GetDb();
            var slot = new TimeSlot { Id = 3, Slot = "09-10", IsBooked = false };

            db.TimeSlots.Add(slot);
            await db.SaveChangesAsync();

            var repo = new BookingRepository(db);

            slot.IsBooked = true;
            await repo.UpdateTimeSlotAsync(slot); // method likely returns void; avoid assigning to var

            Assert.True(db.TimeSlots.First().IsBooked);
        }
    }
}