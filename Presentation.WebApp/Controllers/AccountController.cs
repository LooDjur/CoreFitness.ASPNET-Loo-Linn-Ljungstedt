using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Presentation.WebApp.Controllers.Common;

namespace Presentation.WebApp.Controllers;

[Authorize]
public class AccountController : BaseController
{
    [HttpGet]
    [Route("account/{viewName?}")]
    public IActionResult Index(string viewName = "about-me")
    {
        ViewData["ActiveView"] = viewName.ToLower();

        return View();
    }

    //[HttpPost("update-details")]
    //[ValidateAntiForgeryToken]
    //public async Task<IActionResult> UpdateDetails(AboutMeForm form)
    //{
    //    if (!ModelState.IsValid)
    //    {
    //        ViewData["ActiveView"] = "about-me";
    //        ViewData["PageClass"] = "about-me-page";
    //        return View("Index", form);
    //    }

    //    var userId = "5677ba79-34b6-424e-af76-7a7c3b4cc4f3";

    //    var result = await _accountService.UploadImageAsync(userId, form.ProfileImage);

    //    if (!result.Succeeded)
    //    {
    //        ViewData["ErrorMessage"] = result.Message;
    //        ViewData["ActiveView"] = "about-me";
    //        ViewData["PageClass"] = "about-me-page";

    //        return View("Index", form);
    //    }

    //    TempData["SuccessMessage"] = "Profile updated successfully!";
    //    return RedirectToAction(nameof(Index), new { viewName = "about-me" });
    //}
}
