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

namespace GoatSilencerArchitecture.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class GalleriesController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly IWebHostEnvironment _env;

        public GalleriesController(ApplicationDbContext context, IWebHostEnvironment env)
        {
            _context = context;
            _env = env;
        }

        // GET: Admin/Galleries
        public async Task<IActionResult> Index()
        {
            var galleries = await _context.Galleries.ToListAsync();
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
        public IActionResult Create()
        {
            return View();
        }

        // POST: Admin/Galleries/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Gallery gallery, IFormFile? MainImageFile)
        {
            if (ModelState.IsValid)
            {
                if (MainImageFile != null && MainImageFile.Length > 0)
                {
                    gallery.MainImage = await SaveMainImage(MainImageFile);
                }

                _context.Add(gallery);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
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
        public async Task<IActionResult> Edit(int id, Gallery gallery, IFormFile? MainImageFile)
        {
            if (id != gallery.Id) return NotFound();

            if (ModelState.IsValid)
            {
                try
                {
                    if (MainImageFile != null && MainImageFile.Length > 0)
                    {
                        gallery.MainImage = await SaveMainImage(MainImageFile);
                    }

                    _context.Update(gallery);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!GalleryExists(gallery.Id)) return NotFound();
                    throw;
                }
                return RedirectToAction(nameof(Index));
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
                // Delete main image
                if (!string.IsNullOrEmpty(gallery.MainImage))
                {
                    var path = Path.Combine(_env.WebRootPath, gallery.MainImage.TrimStart('/'));
                    if (System.IO.File.Exists(path))
                        System.IO.File.Delete(path);
                }

                // Delete gallery images
                var images = _context.GalleryImages.Where(i => i.GalleryId == id).ToList();
                foreach (var img in images)
                {
                    var filePath = Path.Combine(_env.WebRootPath, img.FilePath.TrimStart('/'));
                    if (System.IO.File.Exists(filePath))
                        System.IO.File.Delete(filePath);

                    _context.GalleryImages.Remove(img);
                }

                _context.Galleries.Remove(gallery);
                await _context.SaveChangesAsync();
            }

            return RedirectToAction(nameof(Index));
        }

        // POST: Admin/Galleries/UploadImage
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> UploadImage(int galleryId, IFormFile file, string? caption)
        {
            var gallery = await _context.Galleries.Include(g => g.Images).FirstOrDefaultAsync(g => g.Id == galleryId);
            if (gallery == null) return NotFound();

            if (file != null && file.Length > 0)
            {
                var uploadPath = Path.Combine(_env.WebRootPath, "uploads");
                Directory.CreateDirectory(uploadPath);

                var fileName = Guid.NewGuid() + Path.GetExtension(file.FileName);
                var filePath = Path.Combine(uploadPath, fileName);

                using (var stream = new FileStream(filePath, FileMode.Create))
                {
                    await file.CopyToAsync(stream);
                }

                var newImage = new GalleryImage
                {
                    GalleryId = galleryId,
                    FilePath = "/uploads/" + fileName,
                    Caption = caption ?? ""
                };

                _context.GalleryImages.Add(newImage);

                if (string.IsNullOrEmpty(gallery.MainImage))
                {
                    gallery.MainImage = newImage.FilePath;
                    _context.Update(gallery);
                }

                await _context.SaveChangesAsync();
            }

            return RedirectToAction("Edit", new { id = galleryId });
        }

        // POST: Admin/Galleries/DeleteImage
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteImage(int id, int galleryId)
        {
            var gallery = await _context.Galleries
                .Include(g => g.Images)
                .FirstOrDefaultAsync(g => g.Id == galleryId);

            if (gallery == null) return NotFound();

            var image = await _context.GalleryImages.FindAsync(id);
            if (image != null)
            {
                var filePath = Path.Combine(_env.WebRootPath, image.FilePath.TrimStart('/'));
                if (System.IO.File.Exists(filePath))
                    System.IO.File.Delete(filePath);

                _context.GalleryImages.Remove(image);
                await _context.SaveChangesAsync();

                if (gallery.MainImage == image.FilePath)
                {
                    var newMain = gallery.Images
                        .Where(i => i.Id != id)
                        .OrderBy(i => i.Id)
                        .FirstOrDefault();

                    gallery.MainImage = newMain?.FilePath;
                    _context.Update(gallery);
                    await _context.SaveChangesAsync();
                }
            }

            return RedirectToAction("Edit", new { id = galleryId });
        }

        // POST: Admin/Galleries/SetMainImage
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> SetMainImage(int id, int galleryId)
        {
            var gallery = await _context.Galleries
                .Include(g => g.Images)
                .FirstOrDefaultAsync(g => g.Id == galleryId);

            if (gallery == null) return NotFound();

            var image = gallery.Images.FirstOrDefault(i => i.Id == id);
            if (image != null)
            {
                gallery.MainImage = image.FilePath;
                _context.Update(gallery);
                await _context.SaveChangesAsync();
            }

            return RedirectToAction("Edit", new { id = galleryId });
        }

        private bool GalleryExists(int id)
        {
            return _context.Galleries.Any(e => e.Id == id);
        }

        private async Task<string> SaveMainImage(IFormFile file)
        {
            var uploadPath = Path.Combine(_env.WebRootPath, "uploads");
            Directory.CreateDirectory(uploadPath);

            var fileName = Guid.NewGuid() + Path.GetExtension(file.FileName);
            var filePath = Path.Combine(uploadPath, fileName);

            using (var stream = new FileStream(filePath, FileMode.Create))
            {
                await file.CopyToAsync(stream);
            }

            return "/uploads/" + fileName;
        }
    }
}
