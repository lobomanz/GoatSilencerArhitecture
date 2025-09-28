using System.ComponentModel.DataAnnotations;
using GoatSilencerArchitecture.Services.Validation;

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

        [ValidateRichTextAccessibility]
        public string? RichTextContent { get; set; }

        public string? ImageLeftHeading { get; set; }
        public string? ImageRightTopHeading { get; set; }
        public string? ImageRightBottomHeading { get; set; }
        public string? ImageLeftParagraph { get; set; }
        public string? ImageRightTopParagraph { get; set; }
        public string? ImageRightBottomParagraph { get; set; }
    }
}
