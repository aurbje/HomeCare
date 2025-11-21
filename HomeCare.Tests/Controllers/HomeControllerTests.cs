using Microsoft.AspNetCore.Mvc;
using Xunit;
using HomeCare.Controllers;  // adjust if your namespace is different

namespace HomeCare.Tests.Controllers
{
    public class HomeControllerTests
    {
        private readonly HomeController _controller;

        public HomeControllerTests()
        {
            _controller = new HomeController();
        }

        [Fact]
        public void Index_ReturnsView_WithTitleHome()
        {
            // Act
            var result = _controller.Index();

            // Assert
            var viewResult = Assert.IsType<ViewResult>(result);

            // Check ViewData["Title"]
            Assert.Equal("Home", _controller.ViewData["Title"]);
            // or: Assert.Equal("Home", viewResult.ViewData["Title"]);
        }

        [Fact]
        public void About_ReturnsView_WithTitleAboutUs()
        {
            // Act
            var result = _controller.About();

            // Assert
            var viewResult = Assert.IsType<ViewResult>(result);

            Assert.Equal("About Us", _controller.ViewData["Title"]);
        }

                [Fact]
        public void Contact_ReturnsView()
        {
            // Act
            var result = _controller.Contact();

            // Assert
            var viewResult = Assert.IsType<ViewResult>(result);
            // No title is set in Contact, so we just verify we got a view
            Assert.Null(_controller.ViewData["Title"]);
        }

    }
}
