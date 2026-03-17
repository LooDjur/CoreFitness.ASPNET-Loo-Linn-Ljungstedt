using Microsoft.AspNetCore.Mvc;

namespace Presentation.WebApp.Controllers;

public class AccountController : Controller
{
    public IActionResult Index()
    {
        return View();
    }

    public IActionResult MyBookings()
    {
        return View();
    }

}
