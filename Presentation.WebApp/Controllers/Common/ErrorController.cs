using Microsoft.AspNetCore.Mvc;
using Presentation.WebApp.Models.Error;
using System.Diagnostics;

namespace Presentation.WebApp.Controllers.Common;

public class ErrorController : Controller
{
    [Route("error/{statusCode}")]
    public IActionResult HandleErrorCode(int statusCode)
    {
        var model = new ErrorViewModel
        {
            RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier,
            StatusCode = statusCode
        };
        return View("Error", model);
    }
}
