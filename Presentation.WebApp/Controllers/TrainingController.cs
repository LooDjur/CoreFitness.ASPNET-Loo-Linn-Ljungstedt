using Microsoft.AspNetCore.Mvc;
using Presentation.WebApp.Controllers.Common;
using Presentation.WebApp.Models.Sessions;

namespace Presentation.WebApp.Controllers;

public class TrainingController : BaseController
{
    public IActionResult Index()
    {
        var viewModel = new SessionFormViewModel();

        // Här kommer du senare hämta listan på alla klasser också
        // var sessions = await _sessionService.GetAllAsync();

        return View(viewModel);
    }
}
