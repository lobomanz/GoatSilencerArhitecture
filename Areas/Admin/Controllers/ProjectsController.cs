using GoatSilencerArchitecture.Data;
using GoatSilencerArchitecture.Models;
using GoatSilencerArchitecture.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Globalization;

namespace GoatSilencerArchitecture.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class ProjectsController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly IImageService _imageService;

        public ProjectsController(ApplicationDbContext context, IImageService imageService)
        {
            _context = context;
            _imageService = imageService;
        }

        // GET: Admin/Projects
        public async Task<IActionResult> Index()
        {
            return View("~/Areas/Admin/Views/Projects/Index.cshtml", await _context.Projects.ToListAsync());
        }

        // GET: Admin/Projects/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var project = await _context.Projects
                .Include(p => p.Images)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (project == null)
            {
                return NotFound();
            }

            return View("~/Areas/Admin/Views/Projects/Details.cshtml", project);
        }

        // GET: Admin/Projects/Create
        public IActionResult Create()
        {
            return View("~/Areas/Admin/Views/Projects/Create.cshtml");
        }

        // POST: Admin/Projects/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,Title,Description,IsPublished,SortOrder,RichTextContent")] Project project, List<IFormFile> images, IFormFile mainImageLeftFile, IFormFile mainImageTopRightFile, IFormFile mainImageBottomRightFile)
        {
            if (ModelState.IsValid)
            {
                project.CreatedUtc = DateTime.UtcNow;
                project.UpdatedUtc = DateTime.UtcNow;

                if (mainImageLeftFile != null)
                {
                    project.MainImageLeft = await _imageService.SaveAndCompressImage(mainImageLeftFile);
                }

                if (mainImageTopRightFile != null)
                {
                    project.MainImageTopRight = await _imageService.SaveAndCompressImage(mainImageTopRightFile);
                }

                if (mainImageBottomRightFile != null)
                {
                    project.MainImageBottomRight = await _imageService.SaveAndCompressImage(mainImageBottomRightFile);
                }

                if (images != null && images.Count > 0)
                {
                    foreach (var image in images)
                    {
                        var imageUrl = await _imageService.SaveAndCompressImage(image);
                        if (imageUrl != null)
                        {
                            var imageName = Path.GetFileNameWithoutExtension(image.FileName);
                            var title = CultureInfo.CurrentCulture.TextInfo.ToTitleCase(imageName.Replace('_', ' '));

                            project.Images.Add(new ProjectImage
                            {
                                ImageUrl = imageUrl,
                                Title = title,
                                AltText = title
                            });
                        }
                    }
                }

                _context.Add(project);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View("~/Areas/Admin/Views/Projects/Create.cshtml", project);
        }
        // GET: Admin/Projects/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null) return NotFound();

            var project = await _context.Projects
                .Include(p => p.Images)
                .FirstOrDefaultAsync(p => p.Id == id);

            if (project == null) return NotFound();

            return View("~/Areas/Admin/Views/Projects/Edit.cshtml", project);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(
    int id,
    [Bind("Id,Title,Description,IsPublished,SortOrder,RichTextContent")] Project postedProject,
    IFormFile? mainImageLeftFile,
    IFormFile? mainImageTopRightFile,
    IFormFile? mainImageBottomRightFile,
    List<IFormFile>? images,
    string[]? existingImages
)
        {
            var projectToUpdate = await _context.Projects
                .Include(p => p.Images)
                .FirstOrDefaultAsync(p => p.Id == id);

            if (projectToUpdate == null) return NotFound();

            if (ModelState.IsValid)
            {
                projectToUpdate.Title = postedProject.Title;
                projectToUpdate.Description = postedProject.Description;
                projectToUpdate.IsPublished = postedProject.IsPublished;
                projectToUpdate.SortOrder = postedProject.SortOrder;
                projectToUpdate.RichTextContent = postedProject.RichTextContent;
                projectToUpdate.UpdatedUtc = DateTime.UtcNow;

                if (mainImageLeftFile != null)
                    projectToUpdate.MainImageLeft = await _imageService.SaveAndCompressImage(mainImageLeftFile);

                if (mainImageTopRightFile != null)
                    projectToUpdate.MainImageTopRight = await _imageService.SaveAndCompressImage(mainImageTopRightFile);

                if (mainImageBottomRightFile != null)
                    projectToUpdate.MainImageBottomRight = await _imageService.SaveAndCompressImage(mainImageBottomRightFile);

                // Build unified images list
                var unifiedImages = new List<ProjectImage>();
                int sortOrder = 0;

                // Existing images that were not removed
                if (existingImages != null)
                {
                    foreach (var url in existingImages)
                    {
                        unifiedImages.Add(new ProjectImage
                        {
                            ImageUrl = url,
                            Title = Path.GetFileNameWithoutExtension(url),
                            AltText = Path.GetFileNameWithoutExtension(url),
                            SortOrder = sortOrder++
                        });
                    }
                }

                // New uploads
                if (images != null && images.Any())
                {
                    foreach (var file in images)
                    {
                        var url = await _imageService.SaveAndCompressImage(file);
                        if (url == null) continue;

                        var imageName = Path.GetFileNameWithoutExtension(file.FileName);
                        var title = CultureInfo.CurrentCulture.TextInfo.ToTitleCase(imageName.Replace('_', ' '));

                        unifiedImages.Add(new ProjectImage
                        {
                            ImageUrl = url,
                            Title = title,
                            AltText = title,
                            SortOrder = sortOrder++
                        });
                    }
                }

                // Replace list
                _context.ProjectImages.RemoveRange(projectToUpdate.Images); // wipe old
                projectToUpdate.Images = unifiedImages;

                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }

            return View("~/Areas/Admin/Views/Projects/Edit.cshtml", projectToUpdate);
        }





        // GET: Admin/Projects/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var project = await _context.Projects
                .FirstOrDefaultAsync(m => m.Id == id);
            if (project == null)
            {
                return NotFound();
            }

            return View("~/Areas/Admin/Views/Projects/Delete.cshtml", project);
        }

        // POST: Admin/Projects/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var project = await _context.Projects.Include(p => p.Images).FirstOrDefaultAsync(p => p.Id == id);
            if (project != null)
            {
                foreach (var image in project.Images)
                {
                    _imageService.DeleteImageFiles(image.ImageUrl);
                }

                _context.Projects.Remove(project);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

       

        private bool ProjectExists(int id)
        {
            return _context.Projects.Any(e => e.Id == id);
        }
    }
}
