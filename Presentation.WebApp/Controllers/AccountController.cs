using Application.Abstractions.Authentication;
using Application.Booking;
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
        var utcNow = DateTime.UtcNow;
        var result = await sender.Send(new GetUserProfileQuery(userId, utcNow));

        if (result.IsSuccess)
        {
            var user = result.Value;

            if (string.IsNullOrEmpty(viewModel.Profile.FirstName))
                viewModel.Profile.FirstName = user.FirstName;

            if (string.IsNullOrEmpty(viewModel.Profile.LastName))
                viewModel.Profile.LastName = user.LastName;

            if (string.IsNullOrEmpty(viewModel.Profile.Email))
                viewModel.Profile.Email = user.Email;

            if (string.IsNullOrEmpty(viewModel.Profile.Phone))
                viewModel.Profile.Phone = user.Phone;

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

        var utcNow = DateTime.UtcNow;

        var result = await sender.Send(new UpdateProfileCommand(
            userId,
            model.Profile.FirstName!,
            model.Profile.LastName!,
            model.Profile.Email!,
            model.Profile.Phone,
            model.Profile.ProfileImageUrl,
            utcNow));

        if (result.IsSuccess)
        {
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

        await sender.Send(new CancelMembershipCommand(userId, DateTime.UtcNow));

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
    [Route("account/my-bookings")]
    public async Task<IActionResult> Index(string viewName = "about-me", CancellationToken ct = default)
    {
        if (string.Equals(viewName, "bookings", StringComparison.OrdinalIgnoreCase))
            viewName = "my-bookings";

        ViewData["ActiveView"] = viewName.ToLower();

        var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        if (!Guid.TryParse(userIdClaim, out var userId))
        {
            await authService.SignOutUserAsync();
            return RedirectToAction("SignIn", "Authentication");
        }

        var viewModel = new AccountViewModel();
        await PopulateAccountViewModel(viewModel, userId);

        if (ViewData["ActiveView"]?.ToString() == "my-bookings")
        {
            var result = await sender.Send(new GetMyBookingsQuery(userId), ct);
            if (result.IsSuccess)
            {
                viewModel.MyBookings = result.Value;
            }
        }

        return View("Index", viewModel);
    }


    [HttpGet]
    [Route("/sign-out")]
    public async Task<IActionResult> Logout()
    {
        await authService.SignOutUserAsync();
        return RedirectToAction("Index", "Home");
    }
}
