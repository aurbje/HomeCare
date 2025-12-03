using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Moq;
using HomeCare.Api.Controllers;
using HomeCare.Api.Services.Interfaces;
using HomeCare.Api.DTO;
using HomeCare.Api.Models;
using System.Security.Claims;

namespace HomeCare.Tests.Controllers;

/// <summary>
/// Unit tests for CaregiverController - Caregiver's perspective for availability CRUD operations.
/// Tests cover: Create (Register), Read (Dashboard), Delete availability operations.
/// Following the pattern from course reference: Moq for mocking dependencies.
/// </summary>
public class CaregiverControllerTests
{
    private readonly Mock<ICaregiverService> _mockCaregiverService;
    private readonly CaregiverController _controller;

    public CaregiverControllerTests()
    {
        _mockCaregiverService = new Mock<ICaregiverService>();
        _controller = new CaregiverController(_mockCaregiverService.Object);

        // Setup authenticated caregiver context (Caregiver ID = 10)
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
        var claimsPrincipal = new ClaimsPrincipal(identity);

        _controller.ControllerContext = new ControllerContext
        {
            HttpContext = new DefaultHttpContext { User = claimsPrincipal }
        };
    }

    // ========== POSITIVE TESTS ==========

    /// <summary>
    /// Test 1: GetDashboard returns OK with dashboard data
    /// </summary>
    [Fact]
    public async Task GetDashboard_ReturnsOkWithDashboardData()
    {
        // Arrange
        var dashboardDto = new CaregiverDashboardDto
        {
            CaregiverName = "Test Caregiver",
            AvailableDates = new List<DateTime>
            {
                DateTime.Today.AddDays(1),
                DateTime.Today.AddDays(2)
            },
            UpcomingBookings = new List<BookingSummaryDto>()
        };
        _mockCaregiverService.Setup(s => s.GetDashboardAsync(10, null, null)).ReturnsAsync(dashboardDto);

        // Act
        var result = await _controller.GetDashboard(null, null);

        // Assert
        var okResult = Assert.IsType<OkObjectResult>(result);
        Assert.NotNull(okResult.Value);
    }

    /// <summary>
    /// Test 2: RegisterAvailability with future date returns NoContent
    /// </summary>
    [Fact]
    public async Task RegisterAvailability_FutureDate_ReturnsNoContent()
    {
        // Arrange
        var futureDate = DateTime.Today.AddDays(7);
        _mockCaregiverService.Setup(s => s.RegisterAvailabilityAsync(10, futureDate)).Returns(Task.CompletedTask);

        // Act
        var result = await _controller.RegisterAvailability(futureDate);

        // Assert
        Assert.IsType<NoContentResult>(result);
        _mockCaregiverService.Verify(s => s.RegisterAvailabilityAsync(10, futureDate), Times.Once);
    }

    /// <summary>
    /// Test 3: RegisterMultipleAvailability with valid dates returns NoContent
    /// </summary>
    [Fact]
    public async Task RegisterMultipleAvailability_ValidDates_ReturnsNoContent()
    {
        // Arrange
        var dates = new List<DateTime>
        {
            DateTime.Today.AddDays(1),
            DateTime.Today.AddDays(2),
            DateTime.Today.AddDays(3)
        };
        _mockCaregiverService.Setup(s => s.RegisterMultipleAvailabilityAsync(10, dates)).Returns(Task.CompletedTask);

        // Act
        var result = await _controller.RegisterMultipleAvailability(dates);

        // Assert
        Assert.IsType<NoContentResult>(result);
        _mockCaregiverService.Verify(s => s.RegisterMultipleAvailabilityAsync(10, dates), Times.Once);
    }

    /// <summary>
    /// Test 4: DeleteAvailability with no bookings returns NoContent
    /// </summary>
    [Fact]
    public async Task DeleteAvailability_NoBookings_ReturnsNoContent()
    {
        // Arrange
        var dateToDelete = DateTime.Today.AddDays(5);
        _mockCaregiverService.Setup(s => s.DeleteAvailabilityAsync(10, dateToDelete.Date)).Returns(Task.CompletedTask);

        // Act
        var result = await _controller.DeleteAvailability(dateToDelete);

        // Assert
        Assert.IsType<NoContentResult>(result);
    }

    /// <summary>
    /// Test 5: RequestAvailabilityDeletion with no booking returns NoContent (deletion successful)
    /// </summary>
    [Fact]
    public async Task RequestAvailabilityDeletion_NoBooking_ReturnsNoContent()
    {
        // Arrange
        var date = DateTime.Today.AddDays(3);
        _mockCaregiverService.Setup(s => s.RequestAvailabilityDeletionAsync(10, date.Date)).ReturnsAsync(true);

        // Act
        var result = await _controller.RequestAvailabilityDeletion(10, date);

        // Assert
        Assert.IsType<NoContentResult>(result);
    }

    // ========== NEGATIVE TESTS ==========

    /// <summary>
    /// Test 6: RegisterAvailability with past date returns BadRequest
    /// </summary>
    [Fact]
    public async Task RegisterAvailability_PastDate_ReturnsBadRequest()
    {
        // Arrange
        var pastDate = DateTime.Today.AddDays(-1);

        // Act
        var result = await _controller.RegisterAvailability(pastDate);

        // Assert
        var badRequestResult = Assert.IsType<BadRequestObjectResult>(result);
        // Verify service was NOT called since validation happens in controller
        _mockCaregiverService.Verify(s => s.RegisterAvailabilityAsync(It.IsAny<int>(), It.IsAny<DateTime>()), Times.Never);
    }

    /// <summary>
    /// Test 7: DeleteAvailability with existing bookings returns BadRequest
    /// </summary>
    [Fact]
    public async Task DeleteAvailability_WithBookings_ReturnsBadRequest()
    {
        // Arrange
        var dateWithBookings = DateTime.Today.AddDays(2);
        _mockCaregiverService
            .Setup(s => s.DeleteAvailabilityAsync(10, dateWithBookings.Date))
            .ThrowsAsync(new InvalidOperationException("Cannot delete availability with existing bookings"));

        // Act
        var result = await _controller.DeleteAvailability(dateWithBookings);

        // Assert
        var badRequestResult = Assert.IsType<BadRequestObjectResult>(result);
    }

    /// <summary>
    /// Test 8: RequestAvailabilityDeletion with existing booking returns Conflict
    /// </summary>
    [Fact]
    public async Task RequestAvailabilityDeletion_WithBooking_ReturnsConflict()
    {
        // Arrange
        var dateWithBooking = DateTime.Today.AddDays(1);
        _mockCaregiverService.Setup(s => s.RequestAvailabilityDeletionAsync(10, dateWithBooking.Date)).ReturnsAsync(false);

        // Act
        var result = await _controller.RequestAvailabilityDeletion(10, dateWithBooking);

        // Assert
        var conflictResult = Assert.IsType<ConflictObjectResult>(result);
    }
}
