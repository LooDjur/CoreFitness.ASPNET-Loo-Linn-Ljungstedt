using Application.Abstractions.Authentication;
using Application.Users.Queries;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Presentation.WebApp.Controllers.Common;
using Presentation.WebApp.Models.Account;
using System.Security.Claims;

namespace Presentation.WebApp.Controllers;

[Authorize]
public class AccountController(
    IAuthService authService,
    ISender sender) : BaseController
{
    [HttpGet]
    [Route("account/{viewName?}")]
    public async Task<IActionResult> Index(string viewName = "about-me")
    {
        ViewData["ActiveView"] = viewName.ToLower();

        var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

        var viewModel = new AccountViewModel();

        if (Guid.TryParse(userIdClaim, out var userId))
        {
            var result = await sender.Send(new GetUserProfileQuery(userId));

            if (result.IsSuccess)
            {
                var user = result.Value;

                viewModel.Profile.FirstName = user.FirstName;
                viewModel.Profile.LastName = user.LastName;
                viewModel.Profile.Email = user.Email;
                viewModel.Profile.Phone = user.Phone;
                viewModel.Profile.ProfileImageUrl = user.ProfileImageUrl;

                viewModel.Membership.HasMembership = user.MemberId.HasValue;
                viewModel.Membership.MembershipPlan = user.MembershipPlan;
                viewModel.Membership.IsActive = user.IsActive;
                viewModel.Membership.ExpiryDate = user.ExpiryDate ?? DateTime.MinValue;
                viewModel.Membership.MemberId = user.MemberId?.ToString() ?? "N/A";
            }
        }
        else
        {
            await authService.SignOutUserAsync();
            return RedirectToAction("SignIn", "Authentication");
        }

        return View("Index", viewModel);
    }

    [HttpGet]
    [Route("my-membership")]
    public IActionResult MyMembershipRedirect()
    {
        return RedirectToAction("Index", "Account", new { viewName = "my-membership" });
    }

    [HttpGet]
    [Route("/sign-out")]
    public async Task<IActionResult> Logout()
    {
        await authService.SignOutUserAsync();
        return RedirectToAction("Index", "Home");
    }
}
