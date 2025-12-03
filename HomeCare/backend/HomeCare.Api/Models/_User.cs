/* ============================================================
 * ORIGINAL GROUP CODE - Commented out
 * Replaced with merged version in User.cs
 * Key difference: This version has ICollection<Visit> Visits
 * which should be merged into User.cs if needed
 * ============================================================

using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace HomeCare.Api.Models
{
    // basic user model stored in our own user table (not identity)
    public class User
    {
        public int Id { get; set; }

        // person info
        [Required]
        public string FullName { get; set; } = string.Empty; // e.g. "Name Surname"

        [Required, EmailAddress]
        public string Email { get; set; } = string.Empty;

        // hashed password so we never store plain text
        [Required]
        public string PasswordHash { get; set; } = string.Empty;

        // contact details
        [Required]
        public string TlfNumber { get; set; } = string.Empty;

        [Required]
        public string Address { get; set; } = string.Empty;

        // user role (mainly "user", but can be expanded)
        [Required]
        public string Role { get; set; } = "user";

        // visits assigned to this user
        public ICollection<Visit> Visits { get; set; } = new List<Visit>();

    }
}

*/