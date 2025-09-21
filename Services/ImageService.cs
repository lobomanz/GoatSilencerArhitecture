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
            var filePath = Path.Combine(uploadsFolder, uniqueFileName + fileExtension);
            var webpFilePath = Path.Combine(uploadsFolder, uniqueFileName + ".webp");

            using (var stream = new FileStream(filePath, FileMode.Create))
            {
                await imageFile.CopyToAsync(stream);
            }

            // Process and save as WebP
            using (var image = await Image.LoadAsync(filePath))
            {
                image.Mutate(x => x.Resize(new ResizeOptions
                {
                    Size = new Size(1200, 0), // Max width 1200px, height auto
                    Mode = ResizeMode.Max
                }));
                await image.SaveAsync(webpFilePath, new WebpEncoder { Quality = 75 });
            }

            // Optionally, save a JPEG fallback for older browsers
            if (fileExtension != ".jpeg" && fileExtension != ".jpg")
            {
                using (var image = await Image.LoadAsync(filePath))
                {
                    image.Mutate(x => x.Resize(new ResizeOptions
                    {
                        Size = new Size(1200, 0), // Max width 1200px, height auto
                        Mode = ResizeMode.Max
                    }));
                    await image.SaveAsync(Path.Combine(uploadsFolder, uniqueFileName + ".jpg"), new JpegEncoder { Quality = 75 });
                }
            }
            
            // Delete original file if it's not a jpg/jpeg and we saved a jpg fallback
            if (fileExtension != ".jpeg" && fileExtension != ".jpg" && File.Exists(filePath))
            {
                File.Delete(filePath);
            }


            return "/uploads/" + uniqueFileName + ".webp"; // Return WebP path
        }

        public void DeleteImageFiles(string filePath)
        {
            if (string.IsNullOrEmpty(filePath)) return;

            var fileNameWithoutExtension = Path.GetFileNameWithoutExtension(filePath);
            var uploadsFolder = Path.Combine(_env.WebRootPath, "uploads");

            var originalFilePath = Path.Combine(uploadsFolder, fileNameWithoutExtension + Path.GetExtension(filePath));
            var webpFilePath = Path.Combine(uploadsFolder, fileNameWithoutExtension + ".webp");
            var jpgFilePath = Path.Combine(uploadsFolder, fileNameWithoutExtension + ".jpg"); // For the JPEG fallback

            if (File.Exists(originalFilePath))
            {
                File.Delete(originalFilePath);
            }
            if (File.Exists(webpFilePath))
            {
                File.Delete(webpFilePath);
            }
            if (File.Exists(jpgFilePath))
            {
                File.Delete(jpgFilePath);
            }
        }
    }
}
