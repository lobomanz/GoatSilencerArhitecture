using GoatSilencerArchitecture.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace GoatSilencerArchitecture.Areas.Public.Controllers
{
    [Area("Public")]
    public class HomeController : Controller
    {
        private readonly ApplicationDbContext _context;

        public HomeController(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<IActionResult> Index()
        {
            // Only published projects
            var projects = await _context.Projects
                .Where(g => g.IsPublished)
                .Include(p => p.MainImage) // Add this
                .OrderByDescending(g => g.UpdatedUtc)
                .ToListAsync();

            return View(projects);
        }
    }
}
