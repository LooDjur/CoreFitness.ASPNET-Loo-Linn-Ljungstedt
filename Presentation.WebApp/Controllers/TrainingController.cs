using Microsoft.AspNetCore.Mvc;
using Presentation.WebApp.Controllers.Common;

namespace Presentation.WebApp.Controllers;

public class TrainingController : BaseController
{
    public IActionResult Index()
    {
        return View();
    }
}
