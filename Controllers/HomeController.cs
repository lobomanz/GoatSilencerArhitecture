using System.Diagnostics;
using GoatSilencerArchitecture.Data;
using GoatSilencerArchitecture.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace GoatSilencerArchitecture.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly ApplicationDbContext _context;

        public HomeController(ILogger<HomeController> logger, ApplicationDbContext context)
        {
            _logger = logger;
            _context = context;
        }

        public async Task<IActionResult> Index()
        {
            var galleries = await _context.Galleries
                .Include(g => g.Images)
                .Where(g => g.IsPublished)
                .OrderByDescending(g => g.UpdatedUtc) // latest first
                .ToListAsync();

            // Featured = last 3 updated galleries
            ViewData["FeaturedGalleries"] = galleries.Take(3).ToList();

            return View(galleries);
        }


        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
