using Microsoft.AspNetCore.Mvc;

namespace Presentation.WebApp.Controllers;

public class AuthenticationController : Controller
{
    public IActionResult SignUp()
    {
        return View();
    }

    public IActionResult SetPassword()
    {
        return View();
    }


    public IActionResult SignIn()
    {
        return View();
    }


    public new IActionResult SignOut()
    {
        return LocalRedirect("/");
    }
}
