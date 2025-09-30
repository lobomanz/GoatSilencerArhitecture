using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace GoatSilencerArchitecture.Models
{
    public class BlogComponent
    {
        public int Id { get; set; }
        public string Title { get; set; } = string.Empty;

        [Required]
        public string LayoutType { get; set; } = ""; // e.g., "layout-type-1", "layout-type-10"

        [Column(TypeName = "TEXT")] // For rich text content
        public string? TextContent { get; set; } = "";

        public string? Image1Path { get; set; }
        public string? Image1AltText { get; set; }
        public string? Image2Path { get; set; } // For layout-type-3
        public string? Image2AltText { get; set; }

        public int SortOrder { get; set; }

        public DateTime CreatedUtc { get; set; } = DateTime.UtcNow;
        public DateTime UpdatedUtc { get; set; } = DateTime.UtcNow;
        public List<ImageModel> Images { get; set; } = new List<ImageModel>();
    }
}
