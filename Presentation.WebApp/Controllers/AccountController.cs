using Microsoft.AspNetCore.Mvc;

namespace Presentation.WebApp.Controllers;

public class AccountController : Controller
{
    public IActionResult My()
    {
        return View();
    }

    public IActionResult MyBookings()
    {
        return View();
    }

}
