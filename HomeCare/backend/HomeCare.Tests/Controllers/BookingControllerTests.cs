using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Moq;
using HomeCare.Api.Controllers;
using HomeCare.Api.Services.Interfaces;
using HomeCare.Api.DTO;
using HomeCare.Api.Enums;
using HomeCare.Api.Models;
using System.Security.Claims;

namespace HomeCare.Tests.Controllers;

/// <summary>
/// Unit tests for BookingController - User's perspective for booking CRUD operations.
/// Tests cover: Create, Read, Update (via CreateOrUpdate), Delete (Cancel) operations.
/// Following the pattern from course reference: Moq for mocking dependencies.
/// </summary>
public class BookingControllerTests
{
    private readonly Mock<IBookingService> _mockBookingService;
    private readonly Mock<ILogger<BookingController>> _mockLogger;
    private readonly BookingController _controller;

    public BookingControllerTests()
    {
        _mockBookingService = new Mock<IBookingService>();
        _mockLogger = new Mock<ILogger<BookingController>>();
        _controller = new BookingController(_mockBookingService.Object, _mockLogger.Object);

        // Setup authenticated user context (User ID = 1)
        SetupUserContext(1, "User");
    }

    private void SetupUserContext(int userId, string role)
    {
        var claims = new List<Claim>
        {
            new Claim(ClaimTypes.NameIdentifier, userId.ToString()),
            new Claim(ClaimTypes.Role, role)
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
    /// Test 1: GetBookingInit returns OK with booking initialization data
    /// </summary>
    [Fact]
    public async Task GetBookingInit_ReturnsOkWithInitData()
    {
        // Arrange
        var initDto = new BookingInitDto
        {
            Model = new BookingFormDataDto
            {
                SelectedDate = DateTime.Today,
                Categories = new List<CategoryDto> { new CategoryDto { Id = 1, Name = "Vask" } },
                AvailableDates = new List<AvailableDateDto>()
            },
            ClientName = "Test User",
            Bookings = new List<BookingDto>()
        };
        _mockBookingService.Setup(s => s.GetBookingInitAsync(1)).ReturnsAsync(initDto);

        // Act
        var result = await _controller.GetBookingInit();

        // Assert
        var okResult = Assert.IsType<OkObjectResult>(result);
        var returnedDto = Assert.IsType<BookingInitDto>(okResult.Value);
        Assert.Equal("Test User", returnedDto.ClientName);
    }

    /// <summary>
    /// Test 2: CreateOrUpdateBooking with valid data returns OK
    /// </summary>
    [Fact]
    public async Task CreateBooking_ValidData_ReturnsOk()
    {
        // Arrange
        var request = new BookingRequestDto
        {
            SelectedDate = DateTime.Today.AddDays(1),
            TimeSlotId = 1,
            CategoryId = 1,
            SelectedCaregiverId = 2
        };
        var successResult = new BookingResult
        {
            Success = true,
            BookingId = 1,
            Message = "Booking created",
            ResultType = BookingResultType.Success
        };
        _mockBookingService.Setup(s => s.CreateOrUpdateBookingAsync(request, 1)).ReturnsAsync(successResult);

        // Act
        var result = await _controller.CreateOrUpdateBooking(request);

        // Assert
        var okResult = Assert.IsType<OkObjectResult>(result);
    }

    /// <summary>
    /// Test 3: GetBooking with existing ID returns the booking
    /// </summary>
    [Fact]
    public async Task GetBooking_ExistingId_ReturnsBooking()
    {
        // Arrange
        var bookingDto = new BookingDto
        {
            Id = 1,
            DateTime = DateTime.Today,
            Category = new CategoryDto { Id = 1, Name = "Vask" }
        };
        _mockBookingService.Setup(s => s.GetBookingAsync(1, 1, false)).ReturnsAsync(bookingDto);

        // Act
        var result = await _controller.GetBooking(1);

        // Assert
        var okResult = Assert.IsType<OkObjectResult>(result);
        var returnedBooking = Assert.IsType<BookingDto>(okResult.Value);
        Assert.Equal(1, returnedBooking.Id);
    }

    /// <summary>
    /// Test 4: CancelBooking for own booking returns NoContent
    /// </summary>
    [Fact]
    public async Task CancelBooking_OwnBooking_ReturnsNoContent()
    {
        // Arrange
        var cancelResult = new BookingResult
        {
            Success = true,
            ResultType = BookingResultType.Success,
            Message = "Booking cancelled"
        };
        _mockBookingService.Setup(s => s.CancelBookingAsync(1, 1, false)).ReturnsAsync(cancelResult);

        // Act
        var result = await _controller.CancelBooking(1);

        // Assert
        Assert.IsType<NoContentResult>(result);
    }

    // ========== NEGATIVE TESTS ==========

    /// <summary>
    /// Test 5: CreateOrUpdateBooking with invalid data returns validation error
    /// </summary>
    [Fact]
    public async Task CreateBooking_InvalidData_ReturnsValidationError()
    {
        // Arrange
        var request = new BookingRequestDto
        {
            SelectedDate = DateTime.Today.AddDays(1),
            TimeSlotId = null, // Missing required field
            CategoryId = 1
        };
        var validationResult = new BookingResult
        {
            Success = false,
            ResultType = BookingResultType.ValidationError,
            ValidationErrors = new Dictionary<string, string>
            {
                { "TimeSlotId", "Vennligst velg et tidspunkt." }
            }
        };
        _mockBookingService.Setup(s => s.CreateOrUpdateBookingAsync(request, 1)).ReturnsAsync(validationResult);

        // Act
        var result = await _controller.CreateOrUpdateBooking(request);

        // Assert
        Assert.IsType<ObjectResult>(result); // ValidationProblem returns ObjectResult
    }

    /// <summary>
    /// Test 6: GetBooking with non-existent ID returns NotFound
    /// </summary>
    [Fact]
    public async Task GetBooking_NonExistentId_ReturnsNotFound()
    {
        // Arrange
        _mockBookingService.Setup(s => s.GetBookingAsync(999, 1, false)).ReturnsAsync((BookingDto?)null);

        // Act
        var result = await _controller.GetBooking(999);

        // Assert
        Assert.IsType<NotFoundResult>(result);
    }

    /// <summary>
    /// Test 7: CancelBooking for another user's booking returns Forbid
    /// </summary>
    [Fact]
    public async Task CancelBooking_OtherUserBooking_ReturnsForbid()
    {
        // Arrange
        var forbidResult = new BookingResult
        {
            Success = false,
            ResultType = BookingResultType.Forbidden,
            Message = "Not authorized to cancel this booking"
        };
        _mockBookingService.Setup(s => s.CancelBookingAsync(2, 1, false)).ReturnsAsync(forbidResult);

        // Act
        var result = await _controller.CancelBooking(2);

        // Assert
        Assert.IsType<ForbidResult>(result);
    }

    /// <summary>
    /// Test 8: CancelBooking with non-existent ID returns NotFound
    /// </summary>
    [Fact]
    public async Task CancelBooking_NonExistentId_ReturnsNotFound()
    {
        // Arrange
        var notFoundResult = new BookingResult
        {
            Success = false,
            ResultType = BookingResultType.NotFound,
            Message = "Booking not found"
        };
        _mockBookingService.Setup(s => s.CancelBookingAsync(999, 1, false)).ReturnsAsync(notFoundResult);

        // Act
        var result = await _controller.CancelBooking(999);

        // Assert
        var result404 = Assert.IsType<NotFoundObjectResult>(result);
    }
}
