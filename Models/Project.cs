using System.Collections.Generic;
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
        public List<ImageWithHeadingAndParagraph> ImageSections { get; set; } = new List<ImageWithHeadingAndParagraph>();

        public string? BlogsIdList { get; set; }
    }
}
