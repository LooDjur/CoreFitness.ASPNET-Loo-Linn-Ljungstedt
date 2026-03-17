using Application.CustomerSupport;
using Application.CustomerSupport.Input;
using Microsoft.AspNetCore.Mvc;
using Presentation.WebApp.Models.ContactForm;

namespace Presentation.WebApp.Controllers;

public class CustomerServiceController(IContactFormService contactFormService): Controller
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

        var input = new RegisterContactRequestInput(
            model.FirstName,
            model.LastName,
            model.Email,
            model.Phone,
            model.Message
        );

        await contactFormService.RegisterContactRequestAsync(input);
        TempData["Message"] = "Your message has been sent successfully. We will get back to you as soon as possible.";
        return RedirectToAction("Index");
    }
}
