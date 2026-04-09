using Application.Abstractions.Authentication;
using Application.Users.Commands.Delete.Membership;
using Application.Users.Commands.Delete.User;
using Application.Users.Commands.Update;
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
    private async Task PopulateAccountViewModel(AccountViewModel viewModel, Guid userId)
    {
        var result = await sender.Send(new GetUserProfileQuery(userId));

        if (result.IsSuccess)
        {
            var user = result.Value;

            viewModel.Profile.FirstName ??= user.FirstName;
            viewModel.Profile.LastName ??= user.LastName;
            viewModel.Profile.Email ??= user.Email;
            viewModel.Profile.Phone ??= user.Phone;
            viewModel.Profile.ProfileImageUrl = user.ProfileImageUrl;

            viewModel.Membership.HasMembership = user.MemberId.HasValue;
            viewModel.Membership.MembershipPlan = user.MembershipPlan;
            viewModel.Membership.IsActive = user.IsActive;
            viewModel.Membership.ExpiryDate = user.ExpiryDate ?? DateTime.MinValue;
            viewModel.Membership.MemberId = user.MemberId?.ToString() ?? "N/A";
        }
    }

    [HttpGet]
    [Route("account/{viewName?}")]
    public async Task<IActionResult> Index(string viewName = "about-me")
    {
        ViewData["ActiveView"] = viewName.ToLower();

        var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        if (!Guid.TryParse(userIdClaim, out var userId))
        {
            await authService.SignOutUserAsync();
            return RedirectToAction("SignIn", "Authentication");
        }

        var viewModel = new AccountViewModel();
        await PopulateAccountViewModel(viewModel, userId);

        return View("Index", viewModel);
    }

    [HttpPost]
    [Route("account/update-details")]
    public async Task<IActionResult> UpdateDetails(AccountViewModel model)
    {
        var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        if (!Guid.TryParse(userIdClaim, out var userId))
            return RedirectToAction("Logout");

        if (!ModelState.IsValid)
        {
            await PopulateAccountViewModel(model, userId);
            return View("Index", model);
        }

        var result = await sender.Send(new UpdateProfileCommand(
            userId,
            model.Profile.FirstName!,
            model.Profile.LastName!,
            model.Profile.Email!,
            model.Profile.Phone,
            model.Profile.ProfileImageUrl));

        if (result.IsSuccess)
        {
            TempData["SuccessMessage"] = "Profilen har uppdaterats!";
            return RedirectToAction("Index", new { viewName = "about-me" });
        }

        await PopulateAccountViewModel(model, userId);
        return View("Index", model);
    }

    [HttpGet]
    [Route("my-membership")]
    public IActionResult MyMembershipRedirect()
    {
        return RedirectToAction("Index", "Account", new { viewName = "my-membership" });
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    [Route("account/cancel-membership")]
    public async Task<IActionResult> CancelMembership()
    {
        var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        if (!Guid.TryParse(userIdClaim, out var userId))
            return RedirectToAction("Logout");

        await sender.Send(new CancelMembershipCommand(userId));

        return RedirectToAction("Index", new { viewName = "my-membership" });
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    [Route("account/delete-account")]
    public async Task<IActionResult> DeleteAccount()
    {
        var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        if (!Guid.TryParse(userIdClaim, out var userId))
            return RedirectToAction("Logout");

        var domainResult = await sender.Send(new DeleteUserCommand(userId));

        if (domainResult.IsFailure)
        {
            return RedirectToAction("Index", new { viewName = "about-me", error = "CouldNotDelete" });
        }

        await authService.DeleteIdentityUserAsync(userId);
        await authService.SignOutUserAsync();

        return RedirectToAction("Index", "Home", new { status = "account-deleted" });
    }

    [HttpGet]
    [Route("/sign-out")]
    public async Task<IActionResult> Logout()
    {
        await authService.SignOutUserAsync();
        return RedirectToAction("Index", "Home");
    }
}
