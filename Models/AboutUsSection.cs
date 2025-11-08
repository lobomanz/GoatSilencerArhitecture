using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace GoatSilencerArchitecture.Models
{
    public class AboutUsSection
    {
        public int Id { get; set; }

        [Required]
        public int BlogComponentId { get; set; }

        [ForeignKey(nameof(BlogComponentId))]
        public BlogComponent BlogComponent { get; set; }

        public int DisplayOrder { get; set; }
    }
}
