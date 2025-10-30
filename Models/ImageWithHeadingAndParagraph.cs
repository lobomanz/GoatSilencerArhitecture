namespace GoatSilencerArchitecture.Models
{
    public class ImageWithHeadingAndParagraph
    {
        public int Id { get; set; }
        public string? ImageUrl { get; set; }
        public string? Heading { get; set; }
        public string? Paragraph { get; set; }
        public string? Position { get; set; } // e.g., "Left", "RightTop", "RightBottom"
        public int ProjectId { get; set; }
        public Project? Project { get; set; }
    }
}
