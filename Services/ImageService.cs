using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using System;
using System.IO;
using System.Threading.Tasks;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.Processing;
using SixLabors.ImageSharp.Formats.Webp;
using SixLabors.ImageSharp.Formats.Jpeg;

namespace GoatSilencerArchitecture.Services
{
    public class ImageService : IImageService
    {
        private readonly IWebHostEnvironment _env;

        public ImageService(IWebHostEnvironment env)
        {
            _env = env;
        }

        public async Task<string> SaveAndCompressImage(IFormFile imageFile)
        {
            if (imageFile == null || imageFile.Length == 0)
            {
                return null;
            }

            var uploadsFolder = Path.Combine(_env.WebRootPath, "uploads");
            if (!Directory.Exists(uploadsFolder))
            {
                Directory.CreateDirectory(uploadsFolder);
            }

            var uniqueFileName = Guid.NewGuid().ToString();
            var fileExtension = Path.GetExtension(imageFile.FileName).ToLowerInvariant();
            var tempFilePath = Path.Combine(uploadsFolder, uniqueFileName + fileExtension);

            using (var stream = new FileStream(tempFilePath, FileMode.Create))
            {
                await imageFile.CopyToAsync(stream);
            }

            var webpFilePath = Path.Combine(uploadsFolder, uniqueFileName + ".webp");
            var mobileWebpFilePath = Path.Combine(uploadsFolder, uniqueFileName + "_mobile.webp");
            var jpgFilePath = Path.Combine(uploadsFolder, uniqueFileName + ".jpg");
            var mobileJpgFilePath = Path.Combine(uploadsFolder, uniqueFileName + "_mobile.jpg");

            using (var image = await Image.LoadAsync(tempFilePath))
            {
                // Desktop versions
                using (var desktopImage = image.Clone(x => x.Resize(new ResizeOptions { Size = new Size(1920, 0), Mode = ResizeMode.Max })))
                {
                    await desktopImage.SaveAsync(webpFilePath, new WebpEncoder { Quality = 75 });
                    await desktopImage.SaveAsync(jpgFilePath, new JpegEncoder { Quality = 75 });
                }

                // Mobile versions
                using (var mobileImage = image.Clone(x => x.Resize(new ResizeOptions { Size = new Size(1080, 0), Mode = ResizeMode.Max })))
                {
                    await mobileImage.SaveAsync(mobileWebpFilePath, new WebpEncoder { Quality = 75 });
                    await mobileImage.SaveAsync(mobileJpgFilePath, new JpegEncoder { Quality = 75 });
                }
            }

            // Delete the original temporary file
            if (File.Exists(tempFilePath))
            {
                File.Delete(tempFilePath);
            }

            return "/uploads/" + uniqueFileName + ".webp"; // Return WebP path for desktop
        }

        public void DeleteImageFiles(string filePath)
        {
            if (string.IsNullOrEmpty(filePath)) return;

            var fileNameWithoutExtension = Path.GetFileNameWithoutExtension(filePath);
            var uploadsFolder = Path.Combine(_env.WebRootPath, "uploads");

            var webpFilePath = Path.Combine(uploadsFolder, fileNameWithoutExtension + ".webp");
            var mobileWebpFilePath = Path.Combine(uploadsFolder, fileNameWithoutExtension + "_mobile.webp");
            var jpgFilePath = Path.Combine(uploadsFolder, fileNameWithoutExtension + ".jpg");
            var mobileJpgFilePath = Path.Combine(uploadsFolder, fileNameWithoutExtension + "_mobile.jpg");

            if (File.Exists(webpFilePath))
            {
                File.Delete(webpFilePath);
            }
            if (File.Exists(mobileWebpFilePath))
            {
                File.Delete(mobileWebpFilePath);
            }
            if (File.Exists(jpgFilePath))
            {
                File.Delete(jpgFilePath);
            }
            if (File.Exists(mobileJpgFilePath))
            {
                File.Delete(mobileJpgFilePath);
            }
        }
    }
}