using System.Globalization;
using System.IO;

namespace GoatSilencerArchitecture.Utilities
{
    public static class AltTextHelper
    {
        public static string GenerateAltText(string imagePath)
        {
            if (string.IsNullOrEmpty(imagePath))
            {
                return "";
            }

            var imageName = Path.GetFileNameWithoutExtension(imagePath);
            var title = CultureInfo.CurrentCulture.TextInfo.ToTitleCase(imageName.Replace('_', ' '));
            return title;
        }
    }
}
