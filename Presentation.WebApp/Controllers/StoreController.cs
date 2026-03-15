using Microsoft.AspNetCore.Mvc;

namespace Presentation.WebApp.Controllers
{
    public class StoreController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
