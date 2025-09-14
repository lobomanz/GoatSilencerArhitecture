using GoatSilencerArchitecture.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace GoatSilencerArchitecture.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class HomeController : Controller
    {
        private readonly ApplicationDbContext _context;

        public HomeController(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<IActionResult> Index()
        {
            var galleries = await _context.Galleries
                .Include(g => g.Images)
                .OrderByDescending(g => g.UpdatedUtc)
                .ToListAsync();

            // Show last 3 updated galleries as "featured"
            ViewData["FeaturedGalleries"] = galleries.Take(3).ToList();

            return View(galleries);
        }
    }
}
