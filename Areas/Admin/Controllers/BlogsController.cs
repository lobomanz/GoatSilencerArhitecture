using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using GoatSilencerArchitecture.Data;
using GoatSilencerArchitecture.Models;
using HtmlAgilityPack;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using System.Globalization;

namespace GoatSilencerArchitecture.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class BlogComponentsController : Controller
    {
        private readonly ApplicationDbContext _context;

        public BlogComponentsController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: Admin/BlogComponents
        public async Task<IActionResult> Index()
        {
            var components = await _context.BlogComponents
                .Include(c => c.Images)
                .OrderBy(c => c.SortOrder)
                .ToListAsync();

            return View(components);
        }

        // GET: Admin/BlogComponents/Details/5
        [HttpGet]
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null) return NotFound();

            var blogComponent = await _context.BlogComponents
                .Include(c => c.Images)
                .FirstOrDefaultAsync(c => c.Id == id);

            if (blogComponent == null) return NotFound();

            return View(blogComponent);
        }

        // GET: Admin/BlogComponents/Create
        [HttpGet]
        public async Task<IActionResult> Create()
        {
            var blogComponent = new BlogComponent();
            var maxSortOrder = await _context.BlogComponents.MaxAsync(c => (int?)c.SortOrder) ?? 0;
            blogComponent.SortOrder = maxSortOrder + 1;
            return View(blogComponent);
        }

        // POST: Admin/BlogComponents/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(
            [Bind("LayoutType,TextContent,Image1AltText,Image2AltText,SortOrder")] BlogComponent blogComponent,
            IFormFile? image1File,
            IFormFile? image2File,
            List<IFormFile>? images
        )
        {
            ValidateRichTextContent(blogComponent.TextContent, ModelState);

            if (ModelState.IsValid)
            {
                // Handle single images
                if (image1File != null)
                    blogComponent.Image1Path = await UploadImage(image1File);

                if (image2File != null)
                    blogComponent.Image2Path = await UploadImage(image2File);

                // Handle gallery
                var gallery = new List<ImageModel>();
                int sortOrder = 0;

                if (images != null && images.Any())
                {
                    foreach (var file in images)
                    {
                        var url = await UploadImage(file);
                        if (url == null) continue;

                        var imageName = Path.GetFileNameWithoutExtension(file.FileName);
                        var title = CultureInfo.CurrentCulture.TextInfo.ToTitleCase(imageName.Replace('_', ' '));

                        gallery.Add(new ImageModel
                        {
                            ImageUrl = url,
                            Title = title,
                            AltText = title,
                            SortOrder = sortOrder++
                        });
                    }
                }

                blogComponent.Images = gallery;

                var maxSortOrder = await _context.BlogComponents.MaxAsync(c => (int?)c.SortOrder) ?? 0;
                blogComponent.SortOrder = maxSortOrder + 1;
                blogComponent.CreatedUtc = DateTime.UtcNow;
                blogComponent.UpdatedUtc = DateTime.UtcNow;

                _context.Add(blogComponent);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }

            return View(blogComponent);
        }

        // GET: Admin/BlogComponents/Edit/5
        [HttpGet]
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null) return NotFound();

            var blogComponent = await _context.BlogComponents
                .Include(c => c.Images)
                .FirstOrDefaultAsync(c => c.Id == id);

            if (blogComponent == null) return NotFound();

            return View(blogComponent);
        }

        // POST: Admin/BlogComponents/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(
            int id,
            [Bind("Id,LayoutType,TextContent,SortOrder,CreatedUtc,Image1AltText,Image2AltText")] BlogComponent blogComponent,
            IFormFile? image1File,
            IFormFile? image2File,
            List<IFormFile>? images,
            string[]? existingImages
        )
        {
            if (id != blogComponent.Id) return NotFound();

            ValidateRichTextContent(blogComponent.TextContent, ModelState);

            if (ModelState.IsValid)
            {
                var existingComponent = await _context.BlogComponents
                    .Include(c => c.Images)
                    .AsNoTracking()
                    .FirstOrDefaultAsync(c => c.Id == id);

                if (existingComponent == null) return NotFound();

                // Handle Image1
                if (image1File != null)
                {
                    if (!string.IsNullOrEmpty(existingComponent.Image1Path))
                        DeleteImageFile(existingComponent.Image1Path);

                    blogComponent.Image1Path = await UploadImage(image1File);
                }
                else
                {
                    blogComponent.Image1Path = existingComponent.Image1Path;
                    blogComponent.Image1AltText = existingComponent.Image1AltText;
                }

                // Handle Image2
                if (image2File != null)
                {
                    if (!string.IsNullOrEmpty(existingComponent.Image2Path))
                        DeleteImageFile(existingComponent.Image2Path);

                    blogComponent.Image2Path = await UploadImage(image2File);
                }
                else
                {
                    blogComponent.Image2Path = existingComponent.Image2Path;
                    blogComponent.Image2AltText = existingComponent.Image2AltText;
                }

                // Rebuild gallery
                var unifiedImages = new List<ImageModel>();
                int sortOrder = 0;

                if (existingImages != null)
                {
                    foreach (var url in existingImages)
                    {
                        unifiedImages.Add(new ImageModel
                        {
                            ImageUrl = url,
                            Title = Path.GetFileNameWithoutExtension(url),
                            AltText = Path.GetFileNameWithoutExtension(url),
                            SortOrder = sortOrder++
                        });
                    }
                }

                if (images != null && images.Any())
                {
                    foreach (var file in images)
                    {
                        var url = await UploadImage(file);
                        if (url == null) continue;

                        var imageName = Path.GetFileNameWithoutExtension(file.FileName);
                        var title = CultureInfo.CurrentCulture.TextInfo.ToTitleCase(imageName.Replace('_', ' '));

                        unifiedImages.Add(new ImageModel
                        {
                            ImageUrl = url,
                            Title = title,
                            AltText = title,
                            SortOrder = sortOrder++
                        });
                    }
                }

                _context.BlogImages.RemoveRange(existingComponent.Images);
                blogComponent.Images = unifiedImages;

                blogComponent.UpdatedUtc = DateTime.UtcNow;
                _context.Update(blogComponent);
                await _context.SaveChangesAsync();

                return RedirectToAction(nameof(Index));
            }

            return View(blogComponent);
        }

        // GET: Admin/BlogComponents/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null) return NotFound();

            var blogComponent = await _context.BlogComponents
                .Include(c => c.Images)
                .FirstOrDefaultAsync(c => c.Id == id);

            if (blogComponent == null) return NotFound();

            return View(blogComponent);
        }

        // POST: Admin/BlogComponents/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var blogComponent = await _context.BlogComponents
                .Include(c => c.Images)
                .FirstOrDefaultAsync(c => c.Id == id);

            if (blogComponent != null)
            {
                if (!string.IsNullOrEmpty(blogComponent.Image1Path))
                    DeleteImageFile(blogComponent.Image1Path);

                if (!string.IsNullOrEmpty(blogComponent.Image2Path))
                    DeleteImageFile(blogComponent.Image2Path);

                if (blogComponent.Images.Any())
                    _context.BlogImages.RemoveRange(blogComponent.Images);

                _context.BlogComponents.Remove(blogComponent);
                await _context.SaveChangesAsync();
            }

            return RedirectToAction(nameof(Index));
        }

        // POST: Admin/BlogComponents/Reorder
        [HttpPost]
        public async Task<IActionResult> Reorder([FromBody] int[] ids)
        {
            if (ids == null || ids.Length == 0) return BadRequest("No IDs provided.");

            for (int i = 0; i < ids.Length; i++)
            {
                var component = await _context.BlogComponents.FindAsync(ids[i]);
                if (component != null)
                {
                    component.SortOrder = i + 1;
                    component.UpdatedUtc = DateTime.UtcNow;
                    _context.Update(component);
                }
            }

            await _context.SaveChangesAsync();
            return Ok();
        }

        // Helpers
        private async Task<string> UploadImage(IFormFile file)
        {
            if (file == null || file.Length == 0)
                return null;

            var uploadsFolder = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "uploads");
            if (!Directory.Exists(uploadsFolder))
                Directory.CreateDirectory(uploadsFolder);

            var uniqueFileName = Guid.NewGuid().ToString() + "_" + file.FileName;
            var filePath = Path.Combine(uploadsFolder, uniqueFileName);

            using (var fileStream = new FileStream(filePath, FileMode.Create))
                await file.CopyToAsync(fileStream);

            return "/uploads/" + uniqueFileName;
        }

        private void DeleteImageFile(string imagePath)
        {
            if (string.IsNullOrEmpty(imagePath)) return;

            var filePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", imagePath.TrimStart('/'));
            if (System.IO.File.Exists(filePath))
                System.IO.File.Delete(filePath);
        }

        private void ValidateRichTextContent(string htmlContent, ModelStateDictionary modelState)
        {
            if (string.IsNullOrEmpty(htmlContent)) return;

            var doc = new HtmlDocument();
            doc.LoadHtml(htmlContent);

            // Links consistency
            var links = doc.DocumentNode.SelectNodes("//a[@href]");
            if (links != null)
            {
                var linkGroups = new Dictionary<string, List<(string text, string title)>>();

                foreach (var link in links)
                {
                    var href = link.GetAttributeValue("href", "");
                    var text = link.InnerText.Trim();
                    var title = link.GetAttributeValue("title", "").Trim();

                    if (!string.IsNullOrEmpty(href) && !href.StartsWith("http") && !href.StartsWith("//"))
                    {
                        if (!linkGroups.ContainsKey(href))
                            linkGroups[href] = new List<(string text, string title)>();

                        linkGroups[href].Add((text, title));
                    }
                }

                foreach (var entry in linkGroups)
                {
                    var firstLink = entry.Value.First();
                    if (entry.Value.Any(l => l.text != firstLink.text || l.title != firstLink.title))
                    {
                        modelState.AddModelError("TextContent", $"Link consistency error: Links to '{entry.Key}' must have the same text and title.");
                        break;
                    }
                }
            }

            // Heading hierarchy
            var headings = doc.DocumentNode.SelectNodes("//h1|//h2|//h3|//h4|//h5|//h6");
            if (headings != null)
            {
                int lastLevel = 0;
                foreach (var heading in headings)
                {
                    int currentLevel = int.Parse(heading.Name.Substring(1));
                    if (currentLevel > lastLevel + 1)
                    {
                        modelState.AddModelError("TextContent", $"Heading hierarchy error: Cannot jump from H{lastLevel} to H{currentLevel}.");
                        break;
                    }
                    lastLevel = currentLevel;
                }
            }
        }
    }
}
