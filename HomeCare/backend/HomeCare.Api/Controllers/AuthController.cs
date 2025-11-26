using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using HomeCare.Api.DTOs;
using HomeCare.Api.Models;
using HomeCare.Api.Services;

namespace HomeCare.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class AuthController : ControllerBase
{
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly SignInManager<ApplicationUser> _signInManager;
    private readonly UserService _userService;

    public AuthController(
        UserManager<ApplicationUser> userManager,
        SignInManager<ApplicationUser> signInManager,
        UserService userService)
    {
        _userManager = userManager;
        _signInManager = signInManager;
        _userService = userService;
    }

    [HttpPost("register")]
    public async Task<IActionResult> Register(RegisterDto dto)
    {
        var user = new ApplicationUser
        {
            FullName = dto.FullName,
            Email = dto.Email,
            UserName = dto.Email
        };

        var result = await _userManager.CreateAsync(user, dto.Password);

        if (!result.Succeeded)
            return BadRequest(result.Errors);

        await _userManager.AddToRoleAsync(user, "User");

        return Ok(new { message = "User created successfully" });
    }

    [HttpPost("login")]
    public async Task<IActionResult> Login(LoginDto dto)
    {
        var result = await _signInManager.PasswordSignInAsync(dto.Email, dto.Password, true, false);

        if (!result.Succeeded)
            return Unauthorized(new { message = "Invalid email or password" });

        var user = await _userService.GetUserByEmailAsync(dto.Email);
        var roles = await _userManager.GetRolesAsync(user);

        return Ok(new { 
            email = user.Email, 
            fullName = user.FullName, 
            roles 
        });
    }
}
