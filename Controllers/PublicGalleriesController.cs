using GoatSilencerArchitecture.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace GoatSilencerArchitecture.Controllers
{
    public class PublicGalleriesController : Controller
    {
        private readonly ApplicationDbContext _context;

        public PublicGalleriesController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: /PublicGalleries
        public async Task<IActionResult> Index()
        {
            var galleries = await _context.Galleries
                .Include(g => g.Images)
                .Where(g => g.IsPublished)
                .OrderBy(g => g.SortOrder)
                .ToListAsync();

            return View(galleries);
        }

        // GET: /PublicGalleries/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var gallery = await _context.Galleries
                .Include(g => g.Images)
                .FirstOrDefaultAsync(m => m.Id == id && m.IsPublished);

            if (gallery == null)
            {
                return NotFound();
            }

            return View(gallery);
        }
    }
}
