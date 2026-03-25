using Domain.Common;
using Microsoft.AspNetCore.Mvc;

namespace Presentation.WebApp.Controllers.Common;

public abstract class BaseController : Controller
{
    protected IActionResult HandleFailure<TModel>(Result result, TModel model)
    {
        if (result.Error.Type == ErrorType.Validation)
        {
            ModelState.AddModelError(string.Empty, result.Error.Description);

            return View(model);
        }

        int statusCode = result.Error.Type switch
        {
            ErrorType.NotFound => 404,
            ErrorType.Unauthorized => 401,
            ErrorType.Forbidden => 403,
            ErrorType.Conflict => 409,
            _ => 500
        };

        return RedirectToAction("HandleErrorCode", "Errors", new
        {
            statusCode,
            message = result.Error.Description
        });
    }
}