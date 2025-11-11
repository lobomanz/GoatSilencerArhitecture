using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using GoatSilencerArchitecture.Data;
using GoatSilencerArchitecture.Models;
using Microsoft.AspNetCore.Authorization;

namespace GoatSilencerArchitecture.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(Roles = "Admin,Editor")]
    public class AboutUsPageController : Controller
    {
        private readonly ApplicationDbContext _context;

        public AboutUsPageController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: Admin/AboutUsPage/Edit
        [HttpGet]
        public async Task<IActionResult> Edit()
        {
            // Load all blogs for modal selection
            var allBlogs = await _context.Blogs
                .OrderByDescending(b => b.UpdatedUtc)
                .ToListAsync();

            // Load About Us configuration (with related BlogComponent)
            var aboutUsSections = await _context.AboutUsSections
                .Include(s => s.BlogComponent)
                .OrderBy(s => s.DisplayOrder)
                .ToListAsync();

            ViewBag.AllBlogs = allBlogs;

            return View("~/Areas/Admin/Views/AboutUsPage/Edit.cshtml", aboutUsSections);
        }

        // POST: Admin/AboutUsPage/Save
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Save([FromForm] string BlogsIdList)
        {
            if (string.IsNullOrWhiteSpace(BlogsIdList))
                BlogsIdList = "[]";

            var selectedIds = System.Text.Json.JsonSerializer.Deserialize<List<int>>(BlogsIdList) ?? new List<int>();

            // Load all existing sections
            var existingSections = await _context.AboutUsSections.ToListAsync();

            // Remove unselected ones
            var toRemove = existingSections.Where(s => !selectedIds.Contains(s.BlogComponentId)).ToList();
            if (toRemove.Any())
            {
                _context.AboutUsSections.RemoveRange(toRemove);
            }

            // Update or add new ones while preserving IDs
            int order = 1;
            foreach (var blogId in selectedIds)
            {
                var existing = existingSections.FirstOrDefault(s => s.BlogComponentId == blogId);
                if (existing != null)
                {
                    existing.DisplayOrder = order++;
                    _context.Update(existing);
                }
                else
                {
                    _context.AboutUsSections.Add(new AboutUsSection
                    {
                        BlogComponentId = blogId,
                        DisplayOrder = order++
                    });
                }
            }

            await _context.SaveChangesAsync();

            TempData["Success"] = "About Us page updated successfully!";
            return RedirectToAction(nameof(Edit));
        }

        // GET: Admin/AboutUsPage/GetAllBlogs (for modal)
        [HttpGet]
        public async Task<IActionResult> GetAllBlogs()
        {
            var blogs = await _context.Blogs
                .OrderByDescending(b => b.CreatedUtc)
                .Select(b => new
                {
                    id = b.Id,
                    title = b.Title,
                    layoutType = b.LayoutType,
                    createdUtc = b.CreatedUtc.ToString("yyyy-MM-dd"),
                    updatedUtc = b.UpdatedUtc.ToString("yyyy-MM-dd")
                })
                .ToListAsync();

            return Json(blogs);
        }
    }
}
