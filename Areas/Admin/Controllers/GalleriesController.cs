using System;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using GoatSilencerArchitecture.Data;
using GoatSilencerArchitecture.Models;
using GoatSilencerArchitecture.Services;

namespace GoatSilencerArchitecture.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class GalleriesController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly IWebHostEnvironment _env;
        private readonly IImageService _imageService;

        public GalleriesController(ApplicationDbContext context, IWebHostEnvironment env, IImageService imageService)
        {
            _context = context;
            _env = env;
            _imageService = imageService;
        }

        // GET: Admin/Galleries
        public async Task<IActionResult> Index(string type = "Project")
        {
            var galleries = await _context.Galleries.Where(g => g.Type == type).ToListAsync();
            ViewData["GalleryType"] = type;
            return View(galleries);
        }

        // GET: Admin/Galleries/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null) return NotFound();

            var gallery = await _context.Galleries
                .Include(g => g.Images)
                .FirstOrDefaultAsync(m => m.Id == id);

            if (gallery == null) return NotFound();

            return View(gallery);
        }

        // GET: Admin/Galleries/Create
        public IActionResult Create(string type = "Project")
        {
            ViewData["GalleryType"] = type;
            return View();
        }

        // POST: Admin/Galleries/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        [RequestSizeLimit(104857600)] // 100 MB
        public async Task<IActionResult> Create([Bind("Type,Title,Description,IsPublished,SortOrder,RichTextContent")] Gallery gallery, IFormFile? MainImageLeftFile, IFormFile? MainImageTopRightFile, IFormFile? MainImageBottomRightFile, IEnumerable<IFormFile> files)
        {
            if (ModelState.IsValid)
            {
                using (var transaction = _context.Database.BeginTransaction())
                {
                    try
                    {
                        if (MainImageLeftFile != null && MainImageLeftFile.Length > 0)
                        {
                            gallery.MainImageLeft = await _imageService.SaveAndCompressImage(MainImageLeftFile);
                        }
                        if (MainImageTopRightFile != null && MainImageTopRightFile.Length > 0)
                        {
                            gallery.MainImageTopRight = await _imageService.SaveAndCompressImage(MainImageTopRightFile);
                        }
                        if (MainImageBottomRightFile != null && MainImageBottomRightFile.Length > 0)
                        {
                            gallery.MainImageBottomRight = await _imageService.SaveAndCompressImage(MainImageBottomRightFile);
                        }

                        _context.Add(gallery);
                        await _context.SaveChangesAsync();

                        // Handle additional gallery images
                        foreach (var file in files)
                        {
                            if (file != null && file.Length > 0)
                            {
                                var savedPath = await _imageService.SaveAndCompressImage(file);
                                var newImage = new GalleryImage
                                {
                                    GalleryId = gallery.Id,
                                    FilePath = savedPath,
                                    Caption = "",
                                    AltText = "",
                                    IsMainImage = false // Default for additional images
                                };
                                _context.GalleryImages.Add(newImage);
                            }
                        }
                        await _context.SaveChangesAsync();

                        // Assign default main images if not explicitly set
                        await AssignDefaultMainImages(gallery);

                        transaction.Commit();
                        return RedirectToAction(nameof(Index), new { type = gallery.Type });
                    }
                    catch (Exception)
                    {
                        transaction.Rollback();
                        // Log the exception for debugging
                        // _logger.LogError(ex, "Error saving gallery with multiple images.");
                        ModelState.AddModelError("", "An error occurred while saving the gallery. Please try again.");
                    }
                }
            }
            return View(gallery);
        }

        // GET: Admin/Galleries/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null) return NotFound();

            var gallery = await _context.Galleries
                .Include(g => g.Images)
                .FirstOrDefaultAsync(g => g.Id == id);

            if (gallery == null) return NotFound();

            return View(gallery);
        }

        // POST: Admin/Galleries/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        [RequestSizeLimit(104857600)] // 100 MB
        public async Task<IActionResult> Edit(int id, [Bind("Id,Type,Title,Description,IsPublished,SortOrder,RichTextContent,MainImageLeft,MainImageTopRight,MainImageBottomRight,Images")] Gallery gallery, IFormFile? MainImageLeftFile, IFormFile? MainImageTopRightFile, IFormFile? MainImageBottomRightFile, IEnumerable<IFormFile> files)
        {
            if (id != gallery.Id) return NotFound();

            if (ModelState.IsValid)
            {
                using (var transaction = _context.Database.BeginTransaction())
                {
                    try
                    {
                        // Fetch the existing gallery from the database
                        var existingGallery = await _context.Galleries
                            .Include(g => g.Images)
                            .FirstOrDefaultAsync(g => g.Id == id);

                        if (existingGallery == null) return NotFound();

                        // Handle MainImageLeftFile
                        if (MainImageLeftFile != null && MainImageLeftFile.Length > 0)
                        {
                            if (!string.IsNullOrEmpty(existingGallery.MainImageLeft))
                            {
                                _imageService.DeleteImageFiles(existingGallery.MainImageLeft);
                            }
                            gallery.MainImageLeft = await _imageService.SaveAndCompressImage(MainImageLeftFile);
                        }
                        else
                        {
                            gallery.MainImageLeft = existingGallery.MainImageLeft;
                        }

                        // Handle MainImageTopRightFile
                        if (MainImageTopRightFile != null && MainImageTopRightFile.Length > 0)
                        {
                            if (!string.IsNullOrEmpty(existingGallery.MainImageTopRight))
                            {
                                _imageService.DeleteImageFiles(existingGallery.MainImageTopRight);
                            }
                            gallery.MainImageTopRight = await _imageService.SaveAndCompressImage(MainImageTopRightFile);
                        }
                        else
                        {
                            gallery.MainImageTopRight = existingGallery.MainImageTopRight;
                        }

                        // Handle MainImageBottomRightFile
                        if (MainImageBottomRightFile != null && MainImageBottomRightFile.Length > 0)
                        {
                            if (!string.IsNullOrEmpty(existingGallery.MainImageBottomRight))
                            {
                                _imageService.DeleteImageFiles(existingGallery.MainImageBottomRight);
                            }
                            gallery.MainImageBottomRight = await _imageService.SaveAndCompressImage(MainImageBottomRightFile);
                        }
                        else
                        {
                            gallery.MainImageBottomRight = existingGallery.MainImageBottomRight;
                        }

                        // Update scalar properties
                        _context.Entry(existingGallery).CurrentValues.SetValues(gallery);

                        // Handle additional gallery images
                        if (files != null)
                        {
                            foreach (var file in files)
                            {
                                if (file != null && file.Length > 0)
                                {
                                    var savedPath = await _imageService.SaveAndCompressImage(file);
                                    var newImage = new GalleryImage
                                    {
                                        GalleryId = existingGallery.Id,
                                        FilePath = savedPath,
                                        Caption = "",
                                        AltText = "",
                                        IsMainImage = false
                                    };
                                    _context.GalleryImages.Add(newImage);
                                }
                            }
                        }

                        // --- NEW LOGIC FOR EXISTING IMAGES ---

                        // Get a list of image IDs from the form submission
                        var imageIdsFromForm = gallery.Images?.Select(i => i.Id).ToList() ?? new List<int>();

                        // Images to delete: those in existingGallery but not in the form submission
                        var imagesToDelete = existingGallery.Images
                            .Where(img => !imageIdsFromForm.Contains(img.Id))
                            .ToList();

                        foreach (var imgToDelete in imagesToDelete)
                        {
                            _imageService.DeleteImageFiles(imgToDelete.FilePath);
                            _context.GalleryImages.Remove(imgToDelete);
                        }

                        // Update existing images: iterate through images from the form
                        if (gallery.Images != null)
                        {
                            foreach (var imageFromForm in gallery.Images)
                            {
                                var existingImage = existingGallery.Images.FirstOrDefault(img => img.Id == imageFromForm.Id);
                                if (existingImage != null)
                                {
                                    // Update properties
                                    existingImage.Caption = imageFromForm.Caption;
                                    existingImage.AltText = imageFromForm.AltText;
                                    // EF Core is tracking existingImage, so changes will be detected
                                }
                            }
                        }

                        // --- END NEW LOGIC ---
                        
                        await AssignDefaultMainImages(existingGallery);

                        transaction.Commit();
                    }
                    catch (DbUpdateConcurrencyException)
                    {
                        transaction.Rollback();
                        if (!GalleryExists(gallery.Id)) return NotFound();
                        throw;
                    }
                    catch (Exception)
                    {
                        transaction.Rollback();
                        // Log the exception for debugging
                        // _logger.LogError(ex, "Error saving gallery with multiple images.");
                        ModelState.AddModelError("", "An error occurred while saving the gallery. Please try again.");
                    }
                }
                return RedirectToAction(nameof(Index), new { type = gallery.Type });
            }
            return View(gallery);
        }

        // GET: Admin/Galleries/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null) return NotFound();

            var gallery = await _context.Galleries.FirstOrDefaultAsync(m => m.Id == id);
            if (gallery == null) return NotFound();

            return View(gallery);
        }

        // POST: Admin/Galleries/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var gallery = await _context.Galleries.FindAsync(id);
            if (gallery != null)
            {
                // Delete main images
                if (!string.IsNullOrEmpty(gallery.MainImageLeft))
                {
                    _imageService.DeleteImageFiles(gallery.MainImageLeft);
                }
                if (!string.IsNullOrEmpty(gallery.MainImageTopRight))
                {
                    _imageService.DeleteImageFiles(gallery.MainImageTopRight);
                }
                if (!string.IsNullOrEmpty(gallery.MainImageBottomRight))
                {
                    _imageService.DeleteImageFiles(gallery.MainImageBottomRight);
                }

                // Delete gallery images
                var images = _context.GalleryImages.Where(i => i.GalleryId == id).ToList();
                foreach (var img in images)
                {
                    _imageService.DeleteImageFiles(img.FilePath);
                    _context.GalleryImages.Remove(img);
                }

                _context.Galleries.Remove(gallery);
                await _context.SaveChangesAsync();
            }

            return RedirectToAction(nameof(Index));
        }
        private async Task AssignDefaultMainImages(Gallery gallery)
        {
            var images = await _context.GalleryImages
                                       .Where(gi => gi.GalleryId == gallery.Id)
                                       .OrderBy(gi => gi.Id) // Or some other relevant order
                                       .ToListAsync();

            if (string.IsNullOrEmpty(gallery.MainImageLeft) && images.Count > 0)
            {
                gallery.MainImageLeft = images[0].FilePath;
            }
            if (string.IsNullOrEmpty(gallery.MainImageTopRight) && images.Count > 1)
            {
                gallery.MainImageTopRight = images[1].FilePath;
            }
            if (string.IsNullOrEmpty(gallery.MainImageBottomRight) && images.Count > 2)
            {
                gallery.MainImageBottomRight = images[2].FilePath;
            }

            _context.Update(gallery);
            await _context.SaveChangesAsync();
        }

        private bool GalleryExists(int id)
        {
            return _context.Galleries.Any(e => e.Id == id);
        }


    }
}
