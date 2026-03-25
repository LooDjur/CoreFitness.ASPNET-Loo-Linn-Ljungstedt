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
    public async Task<IActionResult> HandleSubmit(RegisterContactCommand command)
    {
        if (!ModelState.IsValid)
        {
            return View("Index", command);
        }

        var result = await sender.Send(command);

        if (result.IsFailure)
        {
            return HandleFailure(result, command);
        }

        TempData["Message"] = "Tack! Vi har tagit emot ditt meddelande.";
        return RedirectToAction("Index");
    }
}
