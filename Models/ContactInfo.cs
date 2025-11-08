using System.ComponentModel.DataAnnotations;

namespace GoatSilencerArchitecture.Models
{
    public class ContactInfo
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "Company name is required.")]
        public string CompanyName { get; set; } = string.Empty;

        [Required(ErrorMessage = "Email is required.")]
        [EmailAddress(ErrorMessage = "Invalid email format.")]
        public string Email { get; set; } = string.Empty;

        [Required(ErrorMessage = "Phone number is required.")]
        [RegularExpression(@"^[0-9+]+$", ErrorMessage = "Phone number can only contain numbers and '+'")]
        public string PhoneNumber { get; set; } = string.Empty;

        [Required(ErrorMessage = "Address is required.")]
        public string Address { get; set; } = string.Empty;
    }
}
