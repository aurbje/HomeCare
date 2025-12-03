using System;
using System.Linq;
using System.Threading.Tasks;
using HomeCare.Api.Controllers;
using HomeCare.Api.DAL.Interfaces;
using HomeCare.Api.DTO.User;
using HomeCare.Api.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Moq;
using Xunit;

namespace HomeCare.Tests.Controllers
{
    public class BookingControllerTests
    {
        private readonly Mock<IBookingRepository> _repo;
        private readonly Mock<ILogger<BookingController>> _logger;
        private readonly BookingController _controller;

        public BookingControllerTests()
        {
            _repo = new Mock<IBookingRepository>();
            _logger = new Mock<ILogger<BookingController>>();
            _controller = new BookingController(_repo.Object, _logger.Object);
        }

        // 1️⃣ GET booking page
        [Fact]
        public async Task GetBookingPage_ReturnsDtoWithData()
        {
            _repo.Setup(r => r.GetAvailableDatesAsync())
                 .ReturnsAsync(new[] { new AvailableDate { Id = 1, Date = DateTime.Today } });

            _repo.Setup(r => r.GetCategoriesAsync())
                 .ReturnsAsync(new[] { new Category { Id = 1, Name = "Cleaning" } });

            _repo.Setup(r => r.GetAllBookingsAsync())
                 .ReturnsAsync(Array.Empty<Booking>());

            var result = await _controller.GetBookingPage();
            var ok = Assert.IsType<OkObjectResult>(result.Result);
            var dto = Assert.IsType<BookingPageDto>(ok.Value);

            Assert.Single(dto.AvailableDates);
            Assert.Single(dto.Categories);
            Assert.Empty(dto.Bookings);
            Assert.Equal(1, dto.CategoryId);
        }

        // 2️⃣ Invalid model
        [Fact]
        public async Task CreateOrUpdateBooking_InvalidModel_ReturnsBadRequest()
        {
            var dto = new CreateBookingDto
            {
                CategoryId = 0,
                TimeSlotId = 0,
                SelectedDate = DateTime.Today
            };

            _controller.ModelState.AddModelError("CategoryId", "Required");

            var result = await _controller.CreateOrUpdateBooking(dto);
            var bad = Assert.IsType<BadRequestObjectResult>(result);

            Assert.Equal(400, bad.StatusCode);
        }

        // 3️⃣ Valid booking → Created
        [Fact]
        public async Task CreateBooking_Valid_ReturnsCreated()
        {
            var dto = new CreateBookingDto
            {
                CategoryId = 1,
                TimeSlotId = 2,
                SelectedDate = DateTime.Today,
                Notes = "Test"
            };

            var category = new Category { Id = 1, Name = "Cleaning" };
            var date = new AvailableDate { Id = 1, Date = DateTime.Today };
            var slot = new TimeSlot { Id = 2, Slot = "09:00-10:00", IsBooked = false, AvailableDate = date };

            _repo.Setup(r => r.GetCategoryByIdAsync(1)).ReturnsAsync(category);
            _repo.Setup(r => r.GetAvailableTimeSlotAsync(2)).ReturnsAsync(slot);

            var result = await _controller.CreateOrUpdateBooking(dto);
            var created = Assert.IsType<ObjectResult>(result);

            Assert.Equal(201, created.StatusCode);
        }

        // 4️⃣ Cancel booking
        [Fact]
        public async Task CancelBooking_ReturnsOk()
        {
            var booking = new Booking { Id = 10, TimeSlotId = 2 };

            _repo.Setup(r => r.GetBookingByIdAsync(10)).ReturnsAsync(booking);
            _repo.Setup(r => r.GetAvailableTimeSlotAsync(2))
                 .ReturnsAsync(new TimeSlot { Id = 2, IsBooked = true });

            var result = await _controller.CancelBooking(10);
            var ok = Assert.IsType<OkObjectResult>(result);

            Assert.Equal(200, ok.StatusCode);
        }
    }
}