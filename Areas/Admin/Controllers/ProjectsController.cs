using GoatSilencerArchitecture.Data;
using GoatSilencerArchitecture.Models;
using GoatSilencerArchitecture.Services;
using GoatSilencerArchitecture.Utilities;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

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

            var projects = from p in _context.Projects.Include(p => p.MainImage)
                           select p;

            if (!string.IsNullOrEmpty(searchString))
            {
                var searchStringLower = searchString.ToLower();
                if (int.TryParse(searchString, out int searchId))
                {
                    projects = projects.Where(p => p.Id == searchId ||
                                           p.Title.ToLower().Contains(searchStringLower) ||
                                           (p.Description != null && p.Description.ToLower().Contains(searchStringLower)));
                }
                else
                {
                    projects = projects.Where(p => p.Title.ToLower().Contains(searchStringLower) ||
                                           (p.Description != null && p.Description.ToLower().Contains(searchStringLower)));
                }
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
                    projects = projects.OrderByDescending(p => p.UpdatedUtc);
                    break;
            }

            int pageSize = 10;
            return View("~/Areas/Admin/Views/Projects/Index.cshtml",
                await PaginatedList<Project>.CreateAsync(projects.AsNoTracking(), pageNumber ?? 1, pageSize));
        }

        // GET: Admin/Projects/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null) return NotFound();

            var project = await _context.Projects
                .Include(p => p.ImageSections)
                .Include(p => p.MainImage)
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
            [Bind("Title,Description,IsPublished,SortOrder,BlogsIdList,YearBuilt,Location")] Project project,
            IFormFile mainImageFile,
            List<IFormFile> files,
            List<string> headings,
            List<string> paragraphs,
            List<string> positions
        )
        {
            if (ModelState.IsValid)
            {
                project.CreatedUtc = DateTime.UtcNow;
                project.UpdatedUtc = DateTime.UtcNow;

                if (mainImageFile != null)
                {
                    var imageUrl = await _imageService.SaveAndCompressImage(mainImageFile);
                    var image = new ImageModel { ImageUrl = imageUrl, AltText = project.Title };
                    _context.BlogImages.Add(image);
                    await _context.SaveChangesAsync();
                    project.MainImageId = image.Id;
                }

                for (int i = 0; i < files.Count; i++)
                {
                    var imageUrl = files[i] != null ? await _imageService.SaveAndCompressImage(files[i]) : null;
                    project.ImageSections.Add(new ImageWithHeadingAndParagraph
                    {
                        ImageUrl = imageUrl,
                        Heading = headings[i],
                        Paragraph = paragraphs[i],
                        Position = positions[i]
                    });
                }

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

            var project = await _context.Projects.Include(p => p.ImageSections).Include(p => p.MainImage).FirstOrDefaultAsync(p => p.Id == id);
            if (project == null) return NotFound();

            return View("~/Areas/Admin/Views/Projects/Edit.cshtml", project);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(
     int id,
     [Bind("Id,Title,Description,IsPublished,SortOrder,BlogsIdList,MainImageId,YearBuilt,Location")] Project postedProject,
     IFormFile? mainImageFile,
     List<IFormFile>? files,
     List<string>? headings,
     List<string>? paragraphs,
     List<string>? positions,
     List<int>? imageSectionIds
 )
        {
            var projectToUpdate = await _context.Projects
                .Include(p => p.ImageSections)
                .Include(p => p.MainImage)
                .FirstOrDefaultAsync(p => p.Id == id);

            if (projectToUpdate == null)
                return NotFound();

            if (!ModelState.IsValid)
                return View("~/Areas/Admin/Views/Projects/Edit.cshtml", projectToUpdate);

            // === basic info ===
            projectToUpdate.Title = postedProject.Title;
            projectToUpdate.Description = postedProject.Description;
            projectToUpdate.IsPublished = postedProject.IsPublished;
            projectToUpdate.SortOrder = postedProject.SortOrder;
            projectToUpdate.BlogsIdList = postedProject.BlogsIdList;
            projectToUpdate.YearBuilt = postedProject.YearBuilt;
            projectToUpdate.Location = postedProject.Location;
            projectToUpdate.UpdatedUtc = DateTime.UtcNow;

            // === main image ===
            if (mainImageFile != null && mainImageFile.Length > 0)
            {
                var imageUrl = await _imageService.SaveAndCompressImage(mainImageFile);
                var image = new ImageModel { ImageUrl = imageUrl, AltText = projectToUpdate.Title };
                _context.BlogImages.Add(image);
                await _context.SaveChangesAsync();
                projectToUpdate.MainImageId = image.Id;
            }
            else
            {
                // Keep existing image
                projectToUpdate.MainImageId = postedProject.MainImageId;
            }

            // === image sections ===
            files ??= new List<IFormFile>();
            headings ??= new List<string>();
            paragraphs ??= new List<string>();
            positions ??= new List<string>();
            imageSectionIds ??= new List<int>();

            for (int i = 0; i < positions.Count; i++)
            {
                var sectionId = imageSectionIds.ElementAtOrDefault(i);
                var heading = headings.ElementAtOrDefault(i) ?? string.Empty;
                var paragraph = paragraphs.ElementAtOrDefault(i) ?? string.Empty;
                var position = positions.ElementAtOrDefault(i) ?? "Left";
                var file = files.ElementAtOrDefault(i);

                if (sectionId > 0)
                {
                    // Update existing section
                    var sectionToUpdate = projectToUpdate.ImageSections.FirstOrDefault(s => s.Id == sectionId);
                    if (sectionToUpdate != null)
                    {
                        if (file != null && file.Length > 0)
                        {
                            sectionToUpdate.ImageUrl = await _imageService.SaveAndCompressImage(file);
                        }
                        sectionToUpdate.Heading = heading;
                        sectionToUpdate.Paragraph = paragraph;
                        sectionToUpdate.Position = position;
                    }
                }
                else
                {
                    // Add new section
                    string? imageUrl = null;
                    if (file != null && file.Length > 0)
                        imageUrl = await _imageService.SaveAndCompressImage(file);

                    projectToUpdate.ImageSections.Add(new ImageWithHeadingAndParagraph
                    {
                        ImageUrl = imageUrl,
                        Heading = heading,
                        Paragraph = paragraph,
                        Position = position
                    });
                }
            }

            // === remove deleted sections ===
            var submittedIds = imageSectionIds.Where(x => x > 0).ToList();
            var sectionsToRemove = projectToUpdate.ImageSections
                .Where(s => s.Id > 0 && !submittedIds.Contains(s.Id))
                .ToList();

            foreach (var section in sectionsToRemove)
            {
                _context.Remove(section);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }


        // GET: Admin/Projects/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null) return NotFound();

            var project = await _context.Projects.Include(p => p.MainImage).FirstOrDefaultAsync(m => m.Id == id);
            if (project == null) return NotFound();

            return View("~/Areas/Admin/Views/Projects/Delete.cshtml", project);
        }

        // POST: Admin/Projects/Delete/5
        [HttpPost, ActionName("DeleteConfirmed")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var project = await _context.Projects.FirstOrDefaultAsync(p => p.Id == id);
            if (project == null) return NotFound();

            _context.Projects.Remove(project);
            await _context.SaveChangesAsync();

            return RedirectToAction(nameof(Index));
        }

        private bool ProjectExists(int id)
        {
            return _context.Projects.Any(e => e.Id == id);
        }

        // GET: Admin/Projects/GetAllBlogs
        [HttpGet]
        public async Task<IActionResult> GetAllBlogs()
        {
            var blogs = await _context.Blogs
                .Select(b => new
                {
                    id = b.Id,
                    title = b.Title,
                    layoutType = b.LayoutType,
                    createdUtc = b.CreatedUtc,
                    updatedUtc = b.UpdatedUtc
                })
                .OrderByDescending(b => b.updatedUtc)
                .ToListAsync();

            return Json(blogs);
        }
    }
}
