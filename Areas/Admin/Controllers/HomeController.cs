using GoatSilencerArchitecture.Data;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace GoatSilencerArchitecture.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(Roles = "Admin,Editor")]
    public class HomeController : Controller
    {
        private readonly ApplicationDbContext _context;

        public HomeController(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<IActionResult> Index()
        {
            var projects = await _context.Projects
                .Include(p => p.MainImage) // Add this
                .OrderByDescending(p => p.UpdatedUtc)
                .ToListAsync();

            // Show last 3 updated projects as "featured"
            ViewData["FeaturedProjects"] = projects.Take(3).ToList();

            return View(projects);
        }
    }
}
