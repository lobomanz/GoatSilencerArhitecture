using System.ComponentModel.DataAnnotations;

namespace GoatSilencerArchitecture.Models
{
    public class ImageModel
    {
        public int Id { get; set; }

        [Required]
        public string ImageUrl { get; set; } = string.Empty;

        public string? Title { get; set; }

        public string? AltText { get; set; }

        public int SortOrder { get; set; }

    }
}
