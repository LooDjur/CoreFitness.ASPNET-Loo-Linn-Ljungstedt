using Microsoft.AspNetCore.Mvc;
using Presentation.WebApp.Controllers.Common;

namespace Presentation.WebApp.Controllers;

public class AccountController : BaseController
{
    [HttpGet]
    [Route("account/about-me")]
    public IActionResult Index()
    {
        return View();
    }
}
