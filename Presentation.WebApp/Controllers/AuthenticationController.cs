using Microsoft.AspNetCore.Mvc;
using Presentation.WebApp.Controllers.Common;

namespace Presentation.WebApp.Controllers
{
    public class AuthenticationController : BaseController
    {
        [HttpGet]
        public IActionResult SignIn()
        {
            return View();
        }
    }
}
