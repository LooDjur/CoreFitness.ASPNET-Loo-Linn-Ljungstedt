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

        //[HttpPost]
        //public IActionResult SignIn(SignInFormCommand command)
        //{
        //    if (!ModelState.IsValid)
        //    {
        //        return View("Index", command);
        //    }

        //    return View();
        //}
    }
}
