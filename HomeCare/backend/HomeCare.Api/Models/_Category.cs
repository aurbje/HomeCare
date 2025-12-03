/* ============================================================
 * ORIGINAL GROUP CODE - Commented out
 * Replaced with merged version in Category.cs
 * This file kept for reference purposes
 * ============================================================

using System.ComponentModel.DataAnnotations;

namespace HomeCare.Api.Models
{
    // category used for appointments and booking
    public class Category
    {
        public int Id { get; set; }

        // name of the category (like "Medication", "Cooking", "Other")
        [Required]
        public string Name { get; set; } = string.Empty;
    }
}

*/