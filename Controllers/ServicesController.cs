using GoatSilencerArchitecture.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace GoatSilencerArchitecture.Controllers
{
    public class ServicesController : Controller
    {
        private readonly ApplicationDbContext _context;

        public ServicesController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: Services
        public async Task<IActionResult> Index()
        {
            var services = await _context.Galleries
                .Where(g => g.IsPublished && g.Type == "Service")
                .OrderBy(g => g.SortOrder)
                .ToListAsync();

            return View(services);
        }

        // GET: Services/Details/5
        public async Task<IActionResult> Details(int id)
        {
            var service = await _context.Galleries
                .Include(g => g.Images)
                .FirstOrDefaultAsync(g => g.Id == id && g.IsPublished && g.Type == "Service");

            if (service == null)
                return NotFound();

            return View(service);
        }
    }
}
