using Application.Abstractions.Authentication;
using Application.Users.Commands.Create.User;
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
        return View(new SignInForm());
    }

    [HttpPost]
    [Route("/sign-in")]
    public async Task<IActionResult> SignIn(SignInForm form, string? returnUrl = null)
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

        ModelState.AddModelError("ErrorMessage", "Invalid email or password.");
        return View(form);
    }

    [HttpGet]
    [Route("/sign-up")]
    public IActionResult SignUp() => View(new SignUpForm());

    [HttpPost]
    [Route("/sign-up")]
    public IActionResult SignUp(SignUpForm form)
    {
        if (!ModelState.IsValid)
            return View(form);

        HttpContext.Session.SetString("reg_email", form.Email);

        return RedirectToAction("SetPassword");
    }

    [HttpGet]
    [Route("/set-password")]
    public IActionResult SetPassword()
    {
        var email = HttpContext.Session.GetString("reg_email");
        if (string.IsNullOrEmpty(email)) return RedirectToAction("SignUp");

        return View(new SetPasswordForm());
    }

    [HttpPost]
    [Route("/set-password")]
    public async Task<IActionResult> SetPassword(SetPasswordForm form)
    {
        var email = HttpContext.Session.GetString("reg_email");

        if (string.IsNullOrEmpty(email)) return RedirectToAction("SignUp");

        if (!ModelState.IsValid) return View(form);

        var command = new RegisterQuickCommand(email, form.Password, "Member");
        var result = await sender.Send(command);

        if (result.IsFailure)
        {
            ModelState.AddModelError("ErrorMessage", result.Error.Description);
            return View(form);
        }

        var signInResult = await authService.SignInUserAsync(email, form.Password, false);

        if (signInResult.Succeeded)
        {
            HttpContext.Session.Remove("reg_email");
            return RedirectToAction("Index", "Home");
        }

        return RedirectToAction("SignIn");
    }
}