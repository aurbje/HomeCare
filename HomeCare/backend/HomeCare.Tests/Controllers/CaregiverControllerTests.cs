using System;
using System.Collections.Generic;
using System.Security.Claims;
using System.Threading.Tasks;
using Xunit;
using Moq;
using Microsoft.Extensions.Logging;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Http;
using HomeCare.Api.Services.Interfaces;
using HomeCare.Api.Controllers;
using HomeCare.Api.DTO.User;

namespace HomeCare.Tests.Controllers
{
    public class CaregiverControllerTests
    {
        private readonly Mock<ICaregiverService> _mockService;
        private readonly Mock<ILogger<CaregiverController>> _mockLogger;
        private readonly CaregiverController _controller;

        public CaregiverControllerTests()
        {
            _mockService = new Mock<ICaregiverService>();
            _mockLogger = new Mock<ILogger<CaregiverController>>();
            _controller = new CaregiverController(_mockService.Object, _mockLogger.Object);
            SetupCaregiverContext(10);
        }

        private void SetupCaregiverContext(int caregiverId)
        {
            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.NameIdentifier, caregiverId.ToString()),
                new Claim(ClaimTypes.Role, "Caregiver")
            };
            var identity = new ClaimsIdentity(claims, "TestAuth");
            var user = new ClaimsPrincipal(identity);

            _controller.ControllerContext = new ControllerContext
            {
                HttpContext = new DefaultHttpContext { User = user }
            };
        }

        [Fact]
        public async Task GetDashboard_WithYearMonth_ReturnsOk()
        {
            int caregiverId = 10;
            int year = DateTime.Today.Year;
            int month = DateTime.Today.Month;

            _mockService.Setup(s => s.GetDashboardAsync(caregiverId, year, month))
                        .ReturnsAsync(new CaregiverDashboardDto
                        {
                            CaregiverName = "Test",
                            AvailableDates = new List<DateTime> { DateTime.Today.AddDays(1) },
                            UpcomingBookings = new List<BookingSummaryDto>()
                        });

            var result = await _controller.GetDashboard(year, month);

            var ok = Assert.IsType<OkObjectResult>(result);
            Assert.NotNull(ok.Value);
        }

        [Fact]
        public async Task RegisterAvailability_FutureDate_ReturnsNoContent()
        {
            var futureDate = DateTime.Today.AddDays(7);
            _mockService.Setup(s => s.RegisterAvailabilityAsync(10, futureDate))
                        .Returns(Task.CompletedTask);

            var result = await _controller.RegisterAvailability(futureDate);

            Assert.IsType<NoContentResult>(result);
            _mockService.Verify(s => s.RegisterAvailabilityAsync(10, futureDate), Times.Once);
        }

        [Fact]
        public async Task RegisterAvailability_PastDate_ReturnsBadRequest()
        {
            var pastDate = DateTime.Today.AddDays(-1);

            var result = await _controller.RegisterAvailability(pastDate);

            Assert.IsType<BadRequestObjectResult>(result);
            _mockService.Verify(s => s.RegisterAvailabilityAsync(It.IsAny<int>(), It.IsAny<DateTime>()), Times.Never);
        }
    }
}