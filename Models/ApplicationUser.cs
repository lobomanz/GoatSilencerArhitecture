using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations;

namespace GoatSilencerArchitecture.Models
{
    // Extend IdentityUser with CMS-specific fields
    public class ApplicationUser : IdentityUser
    {
        [MaxLength(128)]
        public string? DisplayName { get; set; }

        [MaxLength(256)]
        public string? RoleDescription { get; set; } // optional, purely informational

        // Add more fields later as needed (e.g., AvatarUrl, Bio, etc.)
    }
}
