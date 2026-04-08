using Application.Users.Commands.Create.Membership;
using Application.Users.Queries;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Presentation.WebApp.Controllers.Common;
using Presentation.WebApp.Models.Account;
using System.Security.Claims;

namespace Presentation.WebApp.Controllers;

public class MembershipsController(ISender sender) : BaseController
{
    [HttpGet]
    [AllowAnonymous]
    [Route("/memberships")]
    public async Task<IActionResult> Index()
    {
        ViewData["Title"] = "Our Memberships";

        var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        if (Guid.TryParse(userIdClaim, out var userId))
        {
            var result = await sender.Send(new GetUserProfileQuery(userId));
            if (result.IsSuccess)
            {
                ViewData["CurrentPlan"] = result.Value.MembershipPlan;
            }
        }

        return View();
    }

    [HttpGet]
    [Route("/membership/select")]
    public IActionResult Select(string plan)
    {
        var onboardingUrl = $"/membership/onboarding?plan={plan}";

        if (User.Identity?.IsAuthenticated != true)
        {
            return RedirectToAction("SignUp", "Authentication", new { returnUrl = onboardingUrl });
        }

        return RedirectToAction("Onboarding", new { plan });
    }

    [HttpGet]
    [Route("/membership/onboarding")]
    public async Task<IActionResult> Onboarding(string plan)
    {
        if (User.Identity?.IsAuthenticated != true)
            return RedirectToAction("SignIn", "Authentication");

        var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        if (!Guid.TryParse(userIdClaim, out var userId))
            return RedirectToAction("SignIn", "Authentication");

        var model = new AboutMeViewModel();

        var result = await sender.Send(new GetUserProfileQuery(userId));

        if (result.IsSuccess)
        {
            var user = result.Value;
            model.FirstName = user.FirstName;
            model.LastName = user.LastName;
            model.Phone = user.Phone;
            model.ProfileImageUrl = user.ProfileImageUrl;

            if (!string.IsNullOrEmpty(user.Email))
                model.Email = user.Email;

            ViewData["CurrentPlan"] = user.MembershipPlan;
        }

        ViewData["SelectedPlan"] = plan;

        return View(model);
    }

    [HttpPost]
    [Route("/membership/onboarding")]
    public async Task<IActionResult> CompleteOnboarding(AboutMeViewModel model, string plan)
    {
        if (!ModelState.IsValid)
        {
            ViewData["SelectedPlan"] = plan;
            return View("Onboarding", model);
        }

        var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        if (!Guid.TryParse(userIdClaim, out var userId))
            return RedirectToAction("SignIn", "Authentication");

        var command = new CompleteOnboardingCommand(
            userId,
            model.FirstName!,
            model.LastName!,
            model.Phone,
            plan
        );

        var result = await sender.Send(command);

        if (result.IsFailure)
        {
            ViewData["SelectedPlan"] = plan;
            return HandleFailure(result, model);
        }

        TempData["SuccessMessage"] = "Your profile and memberhsip has been succesfully updated!";
        return RedirectToAction("Index", "Account", new { viewName = "my-membership" });
    }
}