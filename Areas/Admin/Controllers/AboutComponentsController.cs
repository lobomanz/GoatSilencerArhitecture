using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using GoatSilencerArchitecture.Data;
using GoatSilencerArchitecture.Models;
using HtmlAgilityPack;
using Microsoft.AspNetCore.Mvc.ModelBinding;

namespace GoatSilencerArchitecture.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class AboutComponentsController : Controller
    {
        private readonly ApplicationDbContext _context;

        public AboutComponentsController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: Admin/AboutComponents
        public async Task<IActionResult> Index()
        {
            var components = await _context.AboutComponents.OrderBy(c => c.SortOrder).ToListAsync();
            return View(components);
        }

        // TODO: Implement Create, Edit, Delete, Reorder actions

        // GET: Admin/AboutComponents/Create
        [HttpGet]
        public IActionResult Create()
        {
            return View();
        }

        public async Task<IActionResult> Create(
            [Bind("LayoutType,TextContent,Image1AltText,Image2AltText")] AboutComponent aboutComponent,
            IFormFile? image1File,
            IFormFile? image2File)
        {
            ValidateRichTextContent(aboutComponent.TextContent, ModelState); // Call validation

            if (ModelState.IsValid)
            {
                // Handle image uploads
                if (image1File != null)
                {
                    aboutComponent.Image1Path = await UploadImage(image1File);
                }
                if (image2File != null)
                {
                    aboutComponent.Image2Path = await UploadImage(image2File);
                }

                // Set SortOrder
                var maxSortOrder = await _context.AboutComponents.MaxAsync(c => (int?)c.SortOrder) ?? 0;
                aboutComponent.SortOrder = maxSortOrder + 1;

                aboutComponent.CreatedUtc = DateTime.UtcNow;
                aboutComponent.UpdatedUtc = DateTime.UtcNow;

                _context.Add(aboutComponent);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(aboutComponent);
        }

        private async Task<string> UploadImage(IFormFile file)
        {
            if (file == null || file.Length == 0)
                return null;

            var uploadsFolder = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "uploads");
            if (!Directory.Exists(uploadsFolder))
            {
                Directory.CreateDirectory(uploadsFolder);
            }

            var uniqueFileName = Guid.NewGuid().ToString() + "_" + file.FileName;
            var filePath = Path.Combine(uploadsFolder, uniqueFileName);

            using (var fileStream = new FileStream(filePath, FileMode.Create))
            {
                await file.CopyToAsync(fileStream);
            }

            return "/uploads/" + uniqueFileName;
        }


        public async Task<IActionResult> Edit(int id,
            [Bind("Id,LayoutType,TextContent,SortOrder,CreatedUtc,Image1AltText,Image2AltText")] AboutComponent aboutComponent,
            IFormFile? image1File,
            IFormFile? image2File)
        {
            if (id != aboutComponent.Id)
            {
                return NotFound();
            }

            ValidateRichTextContent(aboutComponent.TextContent, ModelState); // Call validation

            // Custom validation based on LayoutType
            switch (aboutComponent.LayoutType)
            {
                case "layout-type-1": // Text Left, Image Right
                case "layout-type-2": // Image Left, Text Right
                case "layout-type-4": // Image Top Right, Text Spill Left
                case "layout-type-5": // Image Top Left, Text Spill Right
                    if (string.IsNullOrEmpty(aboutComponent.TextContent))
                        ModelState.AddModelError("TextContent", "Text Content is required for this layout type.");
                    if (image1File == null && string.IsNullOrEmpty(aboutComponent.Image1Path))
                        ModelState.AddModelError("Image1Path", "Image 1 is required for this layout type.");
                    if (string.IsNullOrEmpty(aboutComponent.Image1AltText))
                        ModelState.AddModelError("Image1AltText", "Image 1 Alt Text is required for this layout type.");
                    break;
                case "layout-type-3": // Image Left, Text Middle, Image Right
                    if (string.IsNullOrEmpty(aboutComponent.TextContent))
                        ModelState.AddModelError("TextContent", "Text Content is required for this layout type.");
                    if (image1File == null && string.IsNullOrEmpty(aboutComponent.Image1Path))
                        ModelState.AddModelError("Image1Path", "Image 1 is required for this layout type.");
                    if (string.IsNullOrEmpty(aboutComponent.Image1AltText))
                        ModelState.AddModelError("Image1AltText", "Image 1 Alt Text is required for this layout type.");
                    if (image2File == null && string.IsNullOrEmpty(aboutComponent.Image2Path))
                        ModelState.AddModelError("Image2Path", "Image 2 is required for this layout type.");
                    if (string.IsNullOrEmpty(aboutComponent.Image2AltText))
                        ModelState.AddModelError("Image2AltText", "Image 2 Alt Text is required for this layout type.");
                    break;
                case "layout-type-6": // 100% Text
                    if (string.IsNullOrEmpty(aboutComponent.TextContent))
                        ModelState.AddModelError("TextContent", "Text Content is required for this layout type.");
                    break;
                case "layout-type-7": // 100% Picture
                    if (image1File == null && string.IsNullOrEmpty(aboutComponent.Image1Path))
                        ModelState.AddModelError("Image1Path", "Image 1 is required for this layout type.");
                    if (string.IsNullOrEmpty(aboutComponent.Image1AltText))
                        ModelState.AddModelError("Image1AltText", "Image 1 Alt Text is required for this layout type.");
                    break;
            }

            // AltText uniqueness validation (already implemented)
            if (!string.IsNullOrEmpty(aboutComponent.Image1AltText) &&
                !string.IsNullOrEmpty(aboutComponent.Image2AltText) &&
                aboutComponent.Image1AltText == aboutComponent.Image2AltText) // Check if both are provided and same
            {
                ModelState.AddModelError("Image1AltText", "Image Alt Texts cannot be the same.");
                ModelState.AddModelError("Image2AltText", "Image Alt Texts cannot be the same.");
            }

            if (ModelState.IsValid)
            {
                try
                {
                    // Preserve existing image paths and alt texts if no new file is uploaded
                    var existingComponent = await _context.AboutComponents.AsNoTracking().FirstOrDefaultAsync(c => c.Id == id);
                    if (existingComponent == null)
                    {
                        return NotFound();
                    }

                    // Handle Image 1
                    if (image1File != null)
                    {
                        // Delete old image if it exists
                        if (!string.IsNullOrEmpty(existingComponent.Image1Path))
                        {
                            DeleteImageFile(existingComponent.Image1Path);
                        }
                        aboutComponent.Image1Path = await UploadImage(image1File);
                    }
                    else
                    {
                        aboutComponent.Image1Path = existingComponent.Image1Path; // Keep existing image
                        aboutComponent.Image1AltText = existingComponent.Image1AltText; // Keep existing alt text
                    }

                    // Handle Image 2
                    if (image2File != null)
                    {
                        // Delete old image if it exists
                        if (!string.IsNullOrEmpty(existingComponent.Image2Path))
                        {
                            DeleteImageFile(existingComponent.Image2Path);
                        }
                        aboutComponent.Image2Path = await UploadImage(image2File);
                    }
                    else
                    {
                        aboutComponent.Image2Path = existingComponent.Image2Path; // Keep existing image
                        aboutComponent.Image2AltText = existingComponent.Image2AltText; // Keep existing alt text
                    }

                    aboutComponent.UpdatedUtc = DateTime.UtcNow;
                    _context.Update(aboutComponent);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!AboutComponentExists(aboutComponent.Id))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(Index));
            }
            return View(aboutComponent);
        }

        private bool AboutComponentExists(int id)
        {
            return _context.AboutComponents.Any(e => e.Id == id);
        }

        private void ValidateRichTextContent(string htmlContent, ModelStateDictionary modelState)
        {
            if (string.IsNullOrEmpty(htmlContent)) return;

            var doc = new HtmlDocument();
            doc.LoadHtml(htmlContent);

            // --- Link Validation ---
            var links = doc.DocumentNode.SelectNodes("//a[@href]");
            if (links != null)
            {
                var linkGroups = new Dictionary<string, List<(string text, string title)>>();

                foreach (var link in links)
                {
                    var href = link.GetAttributeValue("href", "");
                    var text = link.InnerText.Trim();
                    var title = link.GetAttributeValue("title", "").Trim();

                    // Only validate internal links (same site)
                    // This is a simplification; a more robust check would involve base URLs
                    if (!string.IsNullOrEmpty(href) && !href.StartsWith("http://") && !href.StartsWith("https://") && !href.StartsWith("//"))
                    {
                        if (!linkGroups.ContainsKey(href))
                        {
                            linkGroups[href] = new List<(string text, string title)>();
                        }
                        linkGroups[href].Add((text, title));
                    }
                }

                foreach (var entry in linkGroups)
                {
                    var firstLink = entry.Value.First();
                    if (entry.Value.Any(l => l.text != firstLink.text || l.title != firstLink.title))
                    {
                        modelState.AddModelError("TextContent", $"Link consistency error: Links to '{entry.Key}' must have the same text and title. Found variations.");
                        break; // Only report one error per field
                    }
                }
            }

            // --- Heading Hierarchy Validation ---
            var headings = doc.DocumentNode.SelectNodes("//h1|//h2|//h3|//h4|//h5|//h6");
            if (headings != null)
            {
                int lastLevel = 0; // Represents the last heading level encountered (e.g., 1 for h1, 2 for h2)
                bool hierarchyBroken = false;

                foreach (var heading in headings)
                {
                    int currentLevel = int.Parse(heading.Name.Substring(1)); // h1 -> 1, h2 -> 2

                    if (currentLevel > lastLevel + 1)
                    {
                        modelState.AddModelError("TextContent", $"Heading hierarchy error: Cannot jump from H{lastLevel} to H{currentLevel}. Headings must follow a sequential order (e.g., H1 -> H2 -> H3).");
                        hierarchyBroken = true;
                        break;
                    }
                    lastLevel = currentLevel;
                }
            }
        }

        private void DeleteImageFile(string imagePath)
        {
            if (string.IsNullOrEmpty(imagePath)) return;

            var filePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", imagePath.TrimStart('/'));
            if (System.IO.File.Exists(filePath))
            {
                System.IO.File.Delete(filePath);
            }
        }

        // GET: Admin/AboutComponents/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var aboutComponent = await _context.AboutComponents
                .FirstOrDefaultAsync(m => m.Id == id);
            if (aboutComponent == null)
            {
                return NotFound();
            }

            return View(aboutComponent);
        }

        // POST: Admin/AboutComponents/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var aboutComponent = await _context.AboutComponents.FindAsync(id);
            if (aboutComponent != null)
            {
                // Delete associated images
                if (!string.IsNullOrEmpty(aboutComponent.Image1Path))
                {
                    DeleteImageFile(aboutComponent.Image1Path);
                }
                if (!string.IsNullOrEmpty(aboutComponent.Image2Path))
                {
                    DeleteImageFile(aboutComponent.Image2Path);
                }

                _context.AboutComponents.Remove(aboutComponent);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        // POST: Admin/AboutComponents/Reorder
        [HttpPost]
        public async Task<IActionResult> Reorder([FromBody] int[] ids)
        {
            if (ids == null || ids.Length == 0)
            {
                return BadRequest("No IDs provided.");
            }

            for (int i = 0; i < ids.Length; i++)
            {
                var component = await _context.AboutComponents.FindAsync(ids[i]);
                if (component != null)
                {
                    component.SortOrder = i + 1; // Set new sort order
                    component.UpdatedUtc = DateTime.UtcNow;
                    _context.Update(component);
                }
            }

            await _context.SaveChangesAsync();
            return Ok();
        }
    }
}
