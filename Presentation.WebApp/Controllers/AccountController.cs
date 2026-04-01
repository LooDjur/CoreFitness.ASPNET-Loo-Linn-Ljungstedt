using Application.Abstractions.Authentication;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Presentation.WebApp.Controllers.Common;

namespace Presentation.WebApp.Controllers;

[Authorize]
public class AccountController (
    IAuthService authService) : BaseController
{
    [HttpGet]
    [Route("account/{viewName?}")]
    public IActionResult Index(string viewName = "about-me")
    {
        ViewData["ActiveView"] = viewName.ToLower();

        return View();
    }

    [HttpGet]
    [Route("/sign-out")]
    public async Task<IActionResult> Logout()
    {
        await authService.SignOutUserAsync();
        return RedirectToAction("Index", "Home");
    }
}
