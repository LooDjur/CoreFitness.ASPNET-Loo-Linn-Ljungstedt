using Application.Abstractions.Authentication;
using Application.Users.Commands.Create.User;
using Domain.Common;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Presentation.WebApp.Controllers.Common;
using Presentation.WebApp.Models.Authentication;

namespace Presentation.WebApp.Controllers;

public class AuthenticationController(
    IAuthService authService,
    ISender sender) : BaseController
{
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
            return View(form);

        var result = await authService.SignInUserAsync(form.Email, form.Password, form.RememberMe);

        if (result.Succeeded)
        {
            if (!string.IsNullOrEmpty(returnUrl) && Url.IsLocalUrl(returnUrl))
                return Redirect(returnUrl);

            return RedirectToAction("Index", "Home");
        }

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
    public IActionResult SignUp(SignUpFormViewModel form, string? returnUrl = null)
    {
        if (!ModelState.IsValid)
            return View(form);

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

        var command = new RegisterQuickCommand(email, form.Password, "Member");
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