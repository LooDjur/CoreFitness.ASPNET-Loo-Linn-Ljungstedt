using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Presentation.WebApp.Controllers.Common;

namespace Presentation.WebApp.Controllers;

public class MembershipsController : BaseController
{
    [HttpGet]
    [AllowAnonymous]
    public IActionResult Index()
    {
        ViewData["Title"] = "Our Memberships";
        return View();
    }
}
