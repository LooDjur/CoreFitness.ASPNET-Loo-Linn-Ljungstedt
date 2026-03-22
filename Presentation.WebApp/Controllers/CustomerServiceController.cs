using Application.CustomerSupport.Commands;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Presentation.WebApp.Controllers.Common;
using Presentation.WebApp.Models.ContactForm;

namespace Presentation.WebApp.Controllers;

public class CustomerServiceController(ISender sender) : BaseController
{
    [HttpGet]
    public IActionResult Index()
    {
        return View();
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> HandleSubmit(ContactFormSection model)
    {
        if (!ModelState.IsValid)
            return View("Index", model);

        var command = new RegisterContactCommand(
            model.FirstName,
            model.LastName,
            model.Email,
            model.Phone,
            model.Message
        );

        var result = await sender.Send(command);

        if (result.IsFailure)
        {
            return HandleFailure(result);
        }

        TempData["Message"] = "Your message has been sent successfully. We will get back to you as soon as possible.";
        return RedirectToAction("Index");
    }
}
