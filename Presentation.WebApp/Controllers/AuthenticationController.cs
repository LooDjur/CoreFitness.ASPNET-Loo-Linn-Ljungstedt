using Application.Abstractions.Authentication;
using Application.Users.Commands.Create.User;
using Domain.Common;
using Infrastructure.Identit;
using MediatR;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Presentation.WebApp.Controllers.Common;
using Presentation.WebApp.Models.Authentication;
using System.Security.Claims;

namespace Presentation.WebApp.Controllers;

public class AuthenticationController(
    IAuthService authService,
    ISender sender,
    SignInManager<AppUser> signInManager) : BaseController
{
    [HttpGet]
    [Route("/github-login")]
    public IActionResult GitHubLogin(string? returnUrl = null)
    {
        var redirectUrl = Url.Action(nameof(GitHubCallback), new { returnUrl });
        var properties = signInManager.ConfigureExternalAuthenticationProperties("GitHub", redirectUrl);

        return Challenge(properties, "GitHub");
    }

    [HttpGet]
    [Route("/github-callback")]
    public async Task<IActionResult> GitHubCallback(string? returnUrl = null, string? remoteError = null)
    {
        if (remoteError != null)
            return RedirectToAction("SignIn");

        var info = await signInManager.GetExternalLoginInfoAsync();
        if (info == null)
            return RedirectToAction("SignIn");

        var email = info.Principal.FindFirstValue(ClaimTypes.Email);
        if (string.IsNullOrEmpty(email))
            return RedirectToAction("SignIn");

        var command = new RegisterExternalUserCommand(email, info.LoginProvider, info.ProviderKey);
        var result = await sender.Send(command);

        if (result.IsFailure)
            return HandleFailure(result, new SignInFormViewModel());

        var signInResult = await signInManager.ExternalLoginSignInAsync(
            info.LoginProvider,
            info.ProviderKey,
            isPersistent: false);

        if (signInResult.Succeeded)
        {
            if (!string.IsNullOrEmpty(returnUrl) && Url.IsLocalUrl(returnUrl))
                return Redirect(returnUrl);

            return RedirectToAction("Index", "Home");
        }
        return RedirectToAction("SignIn");
    }

    [HttpGet]
    [Route("/sign-in")]
    public IActionResult SignIn(string? returnUrl = null)
    {
        ViewData["ReturnUrl"] = returnUrl;
        return View(new SignInFormViewModel());
    }

    [HttpPost]
    [Route("/sign-in")]
    public async Task<IActionResult> SignIn(SignInFormViewModel form, string? returnUrl = null)
    {
        if (!ModelState.IsValid)
        {
            ViewData["ReturnUrl"] = returnUrl;
            return View(form);
        }

        var result = await authService.SignInUserAsync(form.Email, form.Password, form.RememberMe);

        if (result.Succeeded)
        {
            if (!string.IsNullOrEmpty(returnUrl) && Url.IsLocalUrl(returnUrl))
                return Redirect(returnUrl);

            return RedirectToAction("Index", "Home");
        }

        ViewData["ReturnUrl"] = returnUrl;
        return HandleFailure(Result.Failure(DomainErrors.Authentication.InvalidCredentials), form);
    }

    [HttpGet]
    [Route("/sign-up")]
    public IActionResult SignUp(string? returnUrl = null)
    {
        ViewData["ReturnUrl"] = returnUrl;
        return View(new SignUpFormViewModel());
    }

    [HttpPost]
    [Route("/sign-up")]
    public async Task<IActionResult> SignUpAsync(SignUpFormViewModel form, string? returnUrl = null)
    {
        if (!ModelState.IsValid)
            return View(form);

        if (await authService.UserExistsAsync(form.Email))
        {
            ModelState.AddModelError("Email", "This email is already registered.");
            return View(form);
        }
        HttpContext.Session.SetString("reg_email", form.Email);
        return RedirectToAction("SetPassword", new { returnUrl });
    }

    [HttpGet]
    [Route("/set-password")]
    public IActionResult SetPassword(string? returnUrl = null)
    {
        ViewData["ReturnUrl"] = returnUrl;
        var email = HttpContext.Session.GetString("reg_email");

        if (string.IsNullOrEmpty(email))
            return RedirectToAction("SignUp");

        return View(new SetPasswordViewModel());
    }

    [HttpPost]
    [Route("/set-password")]
    public async Task<IActionResult> SetPassword(SetPasswordViewModel form, string? returnUrl = null)
    {
        var email = HttpContext.Session.GetString("reg_email");
        if (string.IsNullOrEmpty(email))
            return RedirectToAction("SignUp");

        if (!ModelState.IsValid)
            return View(form);

        var utcNow = DateTime.UtcNow;
        var command = new RegisterQuickCommand(email, form.Password, "Member", utcNow);
        var result = await sender.Send(command);

        if (result.IsFailure)
        {
            return HandleFailure(result, form);
        }

        var signInResult = await authService.SignInUserAsync(email, form.Password, false);

        if (signInResult.Succeeded)
        {
            HttpContext.Session.Remove("reg_email");

            if (!string.IsNullOrEmpty(returnUrl) && Url.IsLocalUrl(returnUrl))
                return Redirect(returnUrl);

            return RedirectToAction("Index", "Home");
        }

        return HandleFailure(Result.Failure(DomainErrors.Authentication.InvalidCredentials), form);
    }
}