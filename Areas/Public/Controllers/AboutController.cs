using Microsoft.AspNetCore.Mvc;

namespace GoatSilencerArchitecture.Areas.Public.Controllers
{
    [Area("Public")]
    public class AboutController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
