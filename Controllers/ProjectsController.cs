using GoatSilencerArchitecture.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

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
                .Where(p => p.IsPublished) // only published projects on public side
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

            return View(project);
        }
    }
}
