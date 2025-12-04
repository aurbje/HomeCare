using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Xunit;
using Moq;
using Microsoft.Extensions.Logging;
using Microsoft.AspNetCore.Mvc;
using HomeCare.Api.Controllers;
using HomeCare.Api.Services.Interfaces;
using HomeCare.Api.DTO.User;

namespace HomeCare.Tests.Controllers
{
    // Unit tests for BookingController
    public class BookingControllerTests
    {
        [Fact]
        public async Task GetBookingInit_ReturnsOk()
        {
            var serviceMock = new Mock<IBookingService>();
            var loggerMock = new Mock<ILogger<BookingController>>();

            var initDto = new BookingInitDto
            {
                ClientName = "John Doe",
                Model = new BookingFormDataDto
                {
                    SelectedDate = DateTime.Today.AddDays(1),
                    Categories = new List<CategoryDto>(),
                    AvailableDates = new List<AvailableDateDto>(),
                    AvailableCaregiver = new List<UserSummaryDto>()
                },
                Bookings = new List<BookingDto>()
            };

            serviceMock.Setup(s => s.GetBookingInitAsync(It.IsAny<int>()))
                       .ReturnsAsync(initDto);

            var controller = new BookingController(serviceMock.Object, loggerMock.Object);

            var result = await controller.GetBookingInit();

            var ok = Assert.IsType<OkObjectResult>(result);
            Assert.Equal(initDto, ok.Value);
        }
// create unit test for CreateOrUpdateBooking method
        [Fact]
        public async Task CreateOrUpdateBooking_Success_ReturnsOk()
        {
            var serviceMock = new Mock<IBookingService>();
            var loggerMock = new Mock<ILogger<BookingController>>();
            var controller = new BookingController(serviceMock.Object, loggerMock.Object);

            var request = new BookingRequestDto
            {
                SelectedDate = DateTime.Today.AddDays(2),
                TimeSlotId = 1,
                CategoryId = 2,
                SelectedCaregiverId = 3,
                Notes = "Test",
                BookingId = 0
            };

            var svcResult = new BookingResultDto
            {
                ResultType = HomeCare.Api.Enums.BookingResultType.Success,
                Message = "Created",
                BookingId = 99,
                Success = true
            };

            serviceMock.Setup(s => s.CreateOrUpdateBookingAsync(request, It.IsAny<int>()))
                       .ReturnsAsync(svcResult);

            var result = await controller.CreateOrUpdateBooking(request);

            var ok = Assert.IsType<OkObjectResult>(result);
            var payload = Assert.IsType<Dictionary<string, object>>(ok.Value);
            Assert.Equal("Created", payload["message"]);
            Assert.Equal(99, payload["bookingId"]);
        }

        [Fact]
        public async Task CancelBooking_NotFound_ReturnsNotFound() // unit test for CancelBooking method when booking not found
        {
            var serviceMock = new Mock<IBookingService>();
            var loggerMock = new Mock<ILogger<BookingController>>();
            var controller = new BookingController(serviceMock.Object, loggerMock.Object);

            var svcResult = new BookingResultDto
            {
                ResultType = HomeCare.Api.Enums.BookingResultType.NotFound,
                Message = "Booking not found",
                Success = false
            };

            serviceMock.Setup(s => s.CancelBookingAsync(123, It.IsAny<int>(), It.IsAny<bool>())) // use booking id 123
                       .ReturnsAsync(svcResult);

            var result = await controller.CancelBooking(123);

            var notFound = Assert.IsType<NotFoundObjectResult>(result);
            var payload = Assert.IsType<Dictionary<string, string>>(notFound.Value);
            Assert.Equal("Booking not found", payload["message"]);
        }

        [Fact]
        public async Task GetBooking_Found_ReturnsOk() // unit test for GetBooking method when booking is found
        {
            var serviceMock = new Mock<IBookingService>();
            var loggerMock = new Mock<ILogger<BookingController>>();
            var controller = new BookingController(serviceMock.Object, loggerMock.Object);

            var booking = new BookingDto
            {
                Id = 10,
                DateTime = DateTime.Today.AddDays(1),
                Notes = "Note"
            };

            serviceMock.Setup(s => s.GetBookingAsync(10, It.IsAny<int>(), It.IsAny<bool>()))
                       .ReturnsAsync(booking);

            var result = await controller.GetBooking(10);

            var ok = Assert.IsType<OkObjectResult>(result);
            Assert.Equal(booking, ok.Value);
        }

        [Fact]
        public async Task SelectCaregiver_ValidDate_ReturnsOkList() // unit test for SelectCaregiver method with valid date
        {
            var serviceMock = new Mock<IBookingService>();
            var loggerMock = new Mock<ILogger<BookingController>>();
            var controller = new BookingController(serviceMock.Object, loggerMock.Object);

            var dateStr = DateTime.Today.AddDays(3).ToString("yyyy-MM-dd");

            var caregivers = new List<UserSummaryDto>
            {
                new UserSummaryDto { Id = 1, FullName = "A" },
                new UserSummaryDto { Id = 2, FullName = "B" }
            };

            serviceMock.Setup(s => s.GetAvailableCaregiverForSlotAsync(It.IsAny<DateTime>(), 1, null)) // timeSlotId 1
                       .ReturnsAsync(caregivers);

            var result = await controller.SelectCaregiver(dateStr, 1, null);

            var ok = Assert.IsType<OkObjectResult>(result);
            var list = Assert.IsType<List<object>>(ok.Value);
            Assert.Equal(2, list.Count);
        }
    }
}