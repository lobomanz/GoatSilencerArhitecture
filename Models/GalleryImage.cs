using System.ComponentModel.DataAnnotations;

namespace GoatSilencerArchitecture.Models
{
    public class GalleryImage
    {
        public int Id { get; set; }
        public int GalleryId { get; set; }

        public string FilePath { get; set; } = string.Empty;
        public string Caption { get; set; } = string.Empty;
        [Required]
        public string AltText { get; set; } = string.Empty;

        public DateTime CreatedUtc { get; set; } = DateTime.UtcNow;

        public Gallery? Gallery { get; set; }
    }
}
