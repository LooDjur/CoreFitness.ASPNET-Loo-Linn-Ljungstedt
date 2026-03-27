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
        //[ValidateAntiForgeryToken]
        //public IActionResult SignIn(SignInFormCommand command)
        //{
        //    if (!ModelState.IsValid)
        //    {
        //        return View("Index", command);
        //    }

        //    var claims = new List<Claim>
        //    {
        //        new Claim(ClaimTypes.Name, user.Email),
        //        new Claim(ClaimTypes.Role, user.Role.ToString())
        //    };

        //    var claimsIdentity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
        //    await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, new ClaimsPrincipal(claimsIdentity));

        //    return View();
        //}
    }
}
