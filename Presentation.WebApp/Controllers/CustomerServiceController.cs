using Application.CustomerSupport.Commands;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Presentation.WebApp.Controllers.Common;
using Presentation.WebApp.Models.CustomerService;

namespace Presentation.WebApp.Controllers;

public class CustomerServiceController(ISender sender) : BaseController
{
    [HttpGet]
    [AllowAnonymous]
    [Route("/customer-service")]
    public IActionResult Index()
    {
        //ViewData["Title"] = "Customer Service";
        return View();
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    [Route("/customer-service")]
    public async Task<IActionResult> HandleSubmit(RegisterContactCommand command)
    {
        var viewModel = new ContactViewModel
        {
            FirstName = command.FirstName,
            LastName = command.LastName,
            Email = command.Email,
            Phone = command.Phone,
            Message = command.Message,
            TermsAndConditions = true
        };

        if (!ModelState.IsValid)
        {
            return View("Index", viewModel);
        }

        var result = await sender.Send(command);

        if (result.IsFailure)
        {
            ModelState.AddModelError(string.Empty, result.Error.Description);
            return View("Index", viewModel);
        }

        TempData["Message"] = "Thank you, we have received your email!";
        return RedirectToAction("Index");
    }
}