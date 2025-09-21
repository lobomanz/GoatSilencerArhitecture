using Microsoft.AspNetCore.Mvc;

namespace GoatSilencerArchitecture.Controllers
{
    public class AboutController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
