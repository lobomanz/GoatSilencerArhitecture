using System.ComponentModel.DataAnnotations;

namespace GoatSilencerArchitecture.Models
{
    public enum LayoutTypeEnum
    {
        [Display(Name = "Layout Type 1 (Text Left, Image Right)")]
        LayoutType1,

        [Display(Name = "Layout Type 2 (Image Left, Text Right)")]
        LayoutType2,

        [Display(Name = "Layout Type 3 (Image Left, Text Middle, Image Right)")]
        LayoutType3,

        [Display(Name = "Layout Type 4 (Image Top Right, Text Spill Left)")]
        LayoutType4,

        [Display(Name = "Layout Type 5 (Image Top Left, Text Spill Right)")]
        LayoutType5,

        [Display(Name = "Layout Type 6 (100% Text)")]
        LayoutType6,

        [Display(Name = "Layout Type 7 (100% Picture)")]
        LayoutType7,

        [Display(Name = "Layout Type 8 (Gallery)")]
        LayoutType8
    }
}
