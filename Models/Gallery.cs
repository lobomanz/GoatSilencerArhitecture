using System.ComponentModel.DataAnnotations;

namespace GoatSilencerArchitecture.Models
{
    public class Gallery
    {
        public int Id { get; set; }
        public string Type { get; set; } = "Project"; // "Project" or "Service"

        [Required]
        public string Title { get; set; } = string.Empty;

        public string? Description { get; set; }
        public bool IsPublished { get; set; }
        public int SortOrder { get; set; }

        public DateTime CreatedUtc { get; set; } = DateTime.UtcNow;
        public DateTime UpdatedUtc { get; set; } = DateTime.UtcNow;

        // New property for the project/service thumbnail (used in grids)
        public string? MainImage { get; set; }
        public string? MainImageAltText { get; set; } = string.Empty;

        public ICollection<GalleryImage> Images { get; set; } = new List<GalleryImage>();
        public string? RichTextContent { get; set; }

    }
}
