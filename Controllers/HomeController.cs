using Microsoft.AspNetCore.Mvc;

namespace GoatSilencerArchitecture.Controllers
{
    public class HomeController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
