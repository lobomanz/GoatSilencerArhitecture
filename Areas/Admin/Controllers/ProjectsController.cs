using GoatSilencerArchitecture.Data;
using GoatSilencerArchitecture.Models;
using GoatSilencerArchitecture.Services;
using GoatSilencerArchitecture.Utilities;
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
        public async Task<IActionResult> Index(string sortOrder, string currentFilter, string searchString, int? pageNumber)
        {
            ViewData["CurrentSort"] = sortOrder;
            ViewData["SortOrderSortParm"] = sortOrder == "SortOrder" ? "sortOrder_desc" : "SortOrder";
            ViewData["CreatedUtcSortParm"] = sortOrder == "CreatedUtc" ? "createdUtc_desc" : "CreatedUtc";
            ViewData["UpdatedUtcSortParm"] = sortOrder == "UpdatedUtc" ? "updatedUtc_desc" : "UpdatedUtc";

            if (searchString != null)
            {
                pageNumber = 1;
            }
            else
            {
                searchString = currentFilter;
            }

            ViewData["CurrentFilter"] = searchString;

            var projects = from p in _context.Projects
                           select p;

            if (!String.IsNullOrEmpty(searchString))
            {
                projects = projects.Where(p => p.Title.Contains(searchString)
                                       || p.Description.Contains(searchString));
            }

            switch (sortOrder)
            {
                case "sortOrder_desc":
                    projects = projects.OrderByDescending(p => p.SortOrder);
                    break;
                case "SortOrder":
                    projects = projects.OrderBy(p => p.SortOrder);
                    break;
                case "updatedUtc_desc":
                    projects = projects.OrderByDescending(p => p.UpdatedUtc);
                    break;
                case "UpdatedUtc":
                    projects = projects.OrderBy(p => p.UpdatedUtc);
                    break;
                case "createdUtc_desc":
                    projects = projects.OrderByDescending(p => p.CreatedUtc);
                    break;
                case "CreatedUtc":
                    projects = projects.OrderBy(p => p.CreatedUtc);
                    break;
                default:
                    projects = projects.OrderByDescending(p => p.UpdatedUtc); // Default sort by last updated
                    break;
            }

            int pageSize = 10; // You can adjust this as needed
            return View("~/Areas/Admin/Views/Projects/Index.cshtml", await PaginatedList<Project>.CreateAsync(projects.AsNoTracking(), pageNumber ?? 1, pageSize));
        }

        // GET: Admin/Projects/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null) return NotFound();

            var project = await _context.Projects
                .Include(p => p.Images)
                .FirstOrDefaultAsync(p => p.Id == id);

            if (project == null) return NotFound();

            return View("~/Areas/Admin/Views/Projects/Details.cshtml", project);
        }

        // GET: Admin/Projects/Create
        public async Task<IActionResult> Create()
        {
            var project = new Project();
            var maxSortOrder = await _context.Projects.MaxAsync(p => (int?)p.SortOrder) ?? 0;
            project.SortOrder = maxSortOrder + 1;
            return View("~/Areas/Admin/Views/Projects/Create.cshtml", project);
        }

        // POST: Admin/Projects/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(
            [Bind("Title,Description,IsPublished,SortOrder,RichTextContent,ImageLeftHeading,ImageRightTopHeading,ImageRightBottomHeading,ImageLeftParagraph,ImageRightTopParagraph,ImageRightBottomParagraph")] Project project,
            IFormFile? mainImageLeftFile,
            IFormFile? mainImageTopRightFile,
            IFormFile? mainImageBottomRightFile,
            List<IFormFile>? images
        )
        {
            if (ModelState.IsValid)
            {
                project.CreatedUtc = DateTime.UtcNow;
                project.UpdatedUtc = DateTime.UtcNow;

                if (mainImageLeftFile != null)
                    project.MainImageLeft = await _imageService.SaveAndCompressImage(mainImageLeftFile);

                if (mainImageTopRightFile != null)
                    project.MainImageTopRight = await _imageService.SaveAndCompressImage(mainImageTopRightFile);

                if (mainImageBottomRightFile != null)
                    project.MainImageBottomRight = await _imageService.SaveAndCompressImage(mainImageBottomRightFile);

                // Handle gallery images
                var gallery = new List<ProjectImage>();
                int sortOrder = 0;

                if (images != null && images.Any())
                {
                    foreach (var file in images)
                    {
                        var url = await _imageService.SaveAndCompressImage(file);
                        if (url == null) continue;

                        var imageName = Path.GetFileNameWithoutExtension(file.FileName);
                        var title = CultureInfo.CurrentCulture.TextInfo.ToTitleCase(imageName.Replace('_', ' '));

                        gallery.Add(new ProjectImage
                        {
                            ImageUrl = url,
                            Title = title,
                            AltText = title,
                            SortOrder = sortOrder++
                        });
                    }
                }

                project.Images = gallery;

                _context.Projects.Add(project);
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

        // POST: Admin/Projects/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(
            int id,
            [Bind("Id,Title,Description,IsPublished,SortOrder,RichTextContent,ImageLeftHeading,ImageRightTopHeading,ImageRightBottomHeading,ImageLeftParagraph,ImageRightTopParagraph,ImageRightBottomParagraph")] Project postedProject,
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
                _context.ProjectImages.RemoveRange(projectToUpdate.Images);
                projectToUpdate.Images = unifiedImages;

                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }

            return View("~/Areas/Admin/Views/Projects/Edit.cshtml", projectToUpdate);
        }

        // GET: Admin/Projects/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null) return NotFound();

            var project = await _context.Projects
                .Include(p => p.Images)
                .FirstOrDefaultAsync(m => m.Id == id);

            if (project == null) return NotFound();

            return View("~/Areas/Admin/Views/Projects/Delete.cshtml", project);
        }

        // POST: Admin/Projects/Delete/5
        [HttpPost, ActionName("DeleteConfirmed")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var project = await _context.Projects
                .Include(p => p.Images)
                .FirstOrDefaultAsync(p => p.Id == id);

            if (project == null) return NotFound();

            if (project.Images.Any())
            {
                _context.ProjectImages.RemoveRange(project.Images);
            }

            _context.Projects.Remove(project);
            await _context.SaveChangesAsync();

            return RedirectToAction(nameof(Index));
        }

        private bool ProjectExists(int id)
        {
            return _context.Projects.Any(e => e.Id == id);
        }
    }
}
