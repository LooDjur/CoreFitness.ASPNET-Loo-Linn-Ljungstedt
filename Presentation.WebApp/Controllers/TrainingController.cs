using Microsoft.AspNetCore.Mvc;

namespace Presentation.WebApp.Controllers
{
    public class TrainingController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
