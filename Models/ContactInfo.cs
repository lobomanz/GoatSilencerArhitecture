
using System.ComponentModel.DataAnnotations;

namespace GoatSilencerArchitecture.Models
{
    public class ContactInfo
    {
        public int Id { get; set; }

        [Required]
        [EmailAddress]
        public string Email { get; set; } = string.Empty;

        [Required]
        [Phone]
        public string PhoneNumber { get; set; } = string.Empty;

        [Required]
        public string CompanyName { get; set; } = string.Empty;

        [Required]
        public string Address { get; set; } = string.Empty;
    }
}
