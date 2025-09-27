using System.ComponentModel.DataAnnotations;

namespace GoatSilencerArchitecture.Models
{
    public class Project
    {
        public int Id { get; set; }

        [Required]
        public string Title { get; set; } = string.Empty;

        public string? Description { get; set; }
        public bool IsPublished { get; set; }
        public int SortOrder { get; set; }

        public DateTime CreatedUtc { get; set; } = DateTime.UtcNow;
        public DateTime UpdatedUtc { get; set; } = DateTime.UtcNow;

        public string? MainImageLeft { get; set; }
        public string? MainImageTopRight { get; set; }
        public string? MainImageBottomRight { get; set; }

        public List<ProjectImage> Images { get; set; } = new List<ProjectImage>();
        public string? RichTextContent { get; set; }
    }
}
