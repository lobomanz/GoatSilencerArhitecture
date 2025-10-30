namespace GoatSilencerArchitecture.Utilities
{
    public static class ImageHelper
    {
        public static string GetMobileImageUrl(string desktopImageUrl)
        {
            if (string.IsNullOrEmpty(desktopImageUrl))
            {
                return string.Empty;
            }

            var fileName = System.IO.Path.GetFileNameWithoutExtension(desktopImageUrl);
            var extension = System.IO.Path.GetExtension(desktopImageUrl);
            var directory = System.IO.Path.GetDirectoryName(desktopImageUrl);

            var mobileFileName = $"{fileName}_mobile{extension}";

            // The directory might be null if the path is just a filename.
            // In this application, paths are like "/uploads/image.webp", so Path.GetDirectoryName will return "/uploads".
            // We need to combine them correctly.
            if (!string.IsNullOrEmpty(directory))
            {
                // Path.Combine for URLs might use backslashes on Windows, so we'll use string concatenation with forward slashes.
                return $"{directory.Replace('\\', '/')}/{mobileFileName}";
            }

            return mobileFileName;
        }
    }
}
