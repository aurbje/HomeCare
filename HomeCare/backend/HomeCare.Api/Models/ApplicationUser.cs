using Microsoft.AspNetCore.Identity;

namespace HomeCare.Api.Models;

public class ApplicationUser : IdentityUser
{
    public string FullName { get; set; } = "";
    public string? Address { get; set; }
    public string? TlfNumber { get; set; }

    // for caregiving relationships
    public string? CaregiverId { get; set; }
}
