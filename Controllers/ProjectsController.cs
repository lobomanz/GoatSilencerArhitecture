using GoatSilencerArchitecture.Data;
using GoatSilencerArchitecture.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Text.Json;

namespace GoatSilencerArchitecture.Controllers
{
    public class ProjectsController : Controller
    {
        private readonly ApplicationDbContext _context;

        public ProjectsController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: Projects
        public async Task<IActionResult> Index()
        {
            var projects = await _context.Projects
                .Where(p => p.IsPublished)
                .OrderBy(p => p.SortOrder)
                .ToListAsync();

            return View(projects);
        }

        // GET: Projects/Details/5
        public async Task<IActionResult> Details(int id)
        {
            var project = await _context.Projects
                .FirstOrDefaultAsync(p => p.Id == id && p.IsPublished);

            if (project == null)
                return NotFound();

            // Parse BlogsIdList into ordered list of IDs
            var blogIds = new List<int>();
            if (!string.IsNullOrEmpty(project.BlogsIdList))
            {
                try
                {
                    blogIds = JsonSerializer.Deserialize<List<int>>(project.BlogsIdList) ?? new List<int>();
                }
                catch
                {
                    blogIds = new List<int>();
                }
            }

            // Fetch blogs and preserve order
            var blogs = new List<BlogComponent>();
            if (blogIds.Any())
            {
                var allBlogs = await _context.Blogs
                    .Include(b => b.Images)
                    .Where(b => blogIds.Contains(b.Id))
                    .ToListAsync();

                blogs = blogIds
                    .Select(id2 => allBlogs.FirstOrDefault(b => b.Id == id2))
                    .Where(b => b != null)
                    .ToList()!;
            }

            ViewData["Blogs"] = blogs;
            return View(project);
        }
    }
}
