using Microsoft.AspNetCore.Mvc;

namespace Presentation.WebApp.Controllers;

public class ErrorsController : Controller
{
    [Route("error/{statusCode}")]
    public IActionResult HandleErrorCode(int statusCode)
    {
        Response.StatusCode = statusCode;

        return statusCode switch
        {
            401 => View("Unauthorized"),
            403 => View("Forbidden"),
            404 => View("NotFound"),
            503 => View("ServiceUnavailable"),
            _ => View("Error")
        };
    }
}
