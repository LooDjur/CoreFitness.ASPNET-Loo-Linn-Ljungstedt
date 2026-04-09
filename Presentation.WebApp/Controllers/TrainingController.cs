using Application.Sessions.Commands.Create;
using Application.Sessions.Commands.Update;
using Application.Sessions.Commands.Delete;
using Application.Sessions.Queries;
using Application.Sessions.Output;
using Domain.Sessions;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Presentation.WebApp.Models.Sessions;

namespace Presentation.WebApp.Controllers;

[Authorize]
public class TrainingController(ISender sender) : Controller
{
    private async Task<List<SessionOutput>> GetSessionsListAsync(CancellationToken ct)
    {
        var result = await sender.Send(new GetSessionsQuery(), ct);
        return result.IsSuccess ? result.Value : [];
    }

    [HttpGet]
    [AllowAnonymous]
    public async Task<IActionResult> Index(CancellationToken ct)
    {
        var viewModel = new SessionsViewModel
        {
            Sessions = await GetSessionsListAsync(ct),
            Form = new SessionFormViewModel()
        };

        return View(viewModel);
    }

    [HttpGet]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> Edit(Guid id, CancellationToken ct)
    {
        var allSessions = await GetSessionsListAsync(ct);
        var result = await sender.Send(new GetSessionByIdQuery(id), ct);

        if (result.IsFailure)
            return NotFound();

        var s = result.Value;

        var viewModel = new SessionsViewModel
        {
            Sessions = allSessions,
            Form = new SessionFormViewModel
            {
                Id = s.Id,
                Title = s.Title,
                Instructor = s.Instructor,
                Category = Enum.Parse<SessionCategory>(s.Category),
                MaxCapacity = s.MaxCapacity,
                StartTime = s.StartTime,
                Description = s.Description
            }
        };

        return View("Index", viewModel);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> Save(SessionFormViewModel form, CancellationToken ct)
    {
        if (!ModelState.IsValid)
        {
            return View("Index", new SessionsViewModel
            {
                Sessions = await GetSessionsListAsync(ct),
                Form = form
            });
        }

        if (form.Category is not SessionCategory category)
        {
            ModelState.AddModelError("Category", "A valid category must be selected.");
            return View("Index", new SessionsViewModel
            {
                Sessions = await GetSessionsListAsync(ct),
                Form = form
            });
        }

        var endTime = form.StartTime.AddHours(1);

        if (form.Id is Guid sessionId && sessionId != Guid.Empty)
        {
            var command = new UpdateSessionCommand(
                sessionId,
                form.Title,
                form.Description,
                form.Instructor,
                category,
                form.StartTime,
                endTime,
                form.MaxCapacity);

            var result = await sender.Send(command, ct);
            if (result.IsFailure)
            {
                ModelState.AddModelError("", result.Error.Description);
                return View("Index", new SessionsViewModel { Sessions = await GetSessionsListAsync(ct), Form = form });
            }
        }
        else
        {
            var command = new CreateSessionCommand(
                form.Title,
                form.Description,
                form.Instructor,
                category,
                form.StartTime,
                endTime,
                form.MaxCapacity);

            var result = await sender.Send(command, ct);
            if (result.IsFailure)
            {
                ModelState.AddModelError("", result.Error.Description);
                return View("Index", new SessionsViewModel { Sessions = await GetSessionsListAsync(ct), Form = form });
            }
        }

        return RedirectToAction(nameof(Index));
    }
}