using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Moq;
using Xunit;
using HomeCare.Controllers;
using HomeCare.Repositories;
using HomeCare.ViewModels.Account;
using HomeCare.Models;

namespace HomeCare.Tests.Controllers
{
    public class AccountControllerTests
    {
        private readonly Mock<IUserRepository> _userRepoMock;
        private readonly Mock<ILogger<AccountController>> _loggerMock;
        private readonly AccountController _controller;

        public AccountControllerTests()
        {
            _userRepoMock = new Mock<IUserRepository>();
            _loggerMock = new Mock<ILogger<AccountController>>();
            _controller = new AccountController(_userRepoMock.Object, _loggerMock.Object);
        }

        // Testing - GET Signin returns the view
        [Fact]
        public void SignIn_Get_ReturnsView()
        {
            // Act
            var result = _controller.SignIn();

            // Assert
            var viewResult = Assert.IsType<ViewResult>(result);
            Assert.Null(viewResult.ViewName); // null => uses default view "SignIn"
        }
        //Testing - Post Signin with invalid model returns same view + model
        [Fact]
        public async Task SignIn_Post_InvalidModel_ReturnsViewWithModel()
        {
            // Arrange
            var model = new SignInViewModel
            {
                Email = "test@example.com",
                Password = ""
            };
            _controller.ModelState.AddModelError("Password", "Required");

            // Act
            var result = await _controller.SignIn(model);

            // Assert
            var viewResult = Assert.IsType<ViewResult>(result);
            Assert.Same(model, viewResult.Model);
        }

        // Testing - POST SignIn with wrong password returns view + error
        [Fact]
        public async Task SignIn_Post_WrongPassword_ReturnsViewWithModelError()
        {
            // Arrange
            var model = new SignInViewModel
        {
            Email = "user@example.com",
            Password = "wrongpassword"
        };

            var user = new User
        {
            Email = model.Email,
            PasswordHash = BCrypt.Net.BCrypt.HashPassword("correctpassword")
        };

            _userRepoMock
            .Setup(r => r.GetByEmailAsync(model.Email))
            .ReturnsAsync(user);

            // Act
            var result = await _controller.SignIn(model);

            // Assert
            var viewResult = Assert.IsType<ViewResult>(result);
            Assert.Same(model, viewResult.Model);
            Assert.False(_controller.ModelState.IsValid);
            Assert.True(_controller.ModelState.ErrorCount > 0);
        }
        [Fact]
        public async Task SignIn_Post_CorrectCredentials_RedirectsToDashboard()
        {
            // Arrange
            var model = new SignInViewModel
            {
                Email = "user@example.com",
                Password = "correctpassword"
            };

            var user = new User
            {
                Email = model.Email,
                PasswordHash = BCrypt.Net.BCrypt.HashPassword(model.Password)
            };

            _userRepoMock
                .Setup(r => r.GetByEmailAsync(model.Email))
                .ReturnsAsync(user);

            // Act
            var result = await _controller.SignIn(model);

            // Assert
            var redirect = Assert.IsType<RedirectToActionResult>(result);
            Assert.Equal("Dashboard", redirect.ActionName);
            Assert.Equal("User", redirect.ControllerName);
        }

    }
}
