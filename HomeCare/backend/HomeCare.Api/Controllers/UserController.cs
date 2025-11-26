using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using HomeCare.Api.Models;
using HomeCare.Api.Services;

namespace HomeCare.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class UserController : ControllerBase
{
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly UserService _userService;

    public UserController(UserManager<ApplicationUser> userManager, UserService userService)
    {
        _userManager = userManager;
        _userService = userService;
    }

    [Authorize]
    [HttpGet("me")]
    public async Task<IActionResult> Me()
    {
        var user = await _userManager.GetUserAsync(User);
        var roles = await _userManager.GetRolesAsync(user);

        return Ok(new {
            email = user.Email,
            fullName = user.FullName,
            roles
        });
    }

    [Authorize(Roles = "Admin")]
    [HttpGet]
    public async Task<IActionResult> GetAll()
    {
        var users = await _userService.GetAllUsersAsync();
        return Ok(users);
    }

    [Authorize(Roles = "Admin")]
    [HttpPatch("{id}/role")]
    public async Task<IActionResult> UpdateRole(string id, [FromBody] string role)
    {
        var user = await _userManager.FindByIdAsync(id);

        if (user == null)
            return NotFound();

        await _userManager.AddToRoleAsync(user, role);

        return Ok(new { message = $"Role {role} added to user" });
    }
}
