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
                .Include(g => g.Images)
                .OrderBy(g => g.SortOrder)
                .ToListAsync();

            return View(projects);
        }


        // GET: Projects/Details/5
        public async Task<IActionResult> Details(int id)
        {
            var project = await _context.Projects
                .Include(g => g.Images) // assuming Gallery has Images navigation
                .FirstOrDefaultAsync(g => g.Id == id && g.IsPublished);

            if (project == null)
                return NotFound();

            return View(project);
        }
    }
}
