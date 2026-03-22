using Domain.Common;
using Microsoft.AspNetCore.Mvc;

namespace Presentation.WebApp.Controllers.Common;

public abstract class BaseController : Controller
{
    protected IActionResult HandleFailure(Result result)
    {
        if (result.IsSuccess)
        {
            return RedirectToAction("Index", "Home");
        }

        int statusCode = result.Error.Type switch
        {
            ErrorType.NotFound => 404,
            ErrorType.Unauthorized => 401,
            ErrorType.Forbidden => 403,
            ErrorType.Validation => 400,
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