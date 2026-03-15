using Microsoft.AspNetCore.Mvc;

namespace Presentation.WebApp.Controllers
{
    public class FitnessCentersController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
