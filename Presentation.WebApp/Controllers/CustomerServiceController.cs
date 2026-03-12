using Microsoft.AspNetCore.Mvc;

namespace Presentation.WebApp.Controllers;

public class CustomerServiceController: Controller
{
    public IActionResult Index()
    {
        return View();
    }
}
