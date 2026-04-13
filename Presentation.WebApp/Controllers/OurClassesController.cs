using Application.Booking;
using Application.Sessions.Commands.Create;
using Application.Sessions.Commands.Delete;
using Application.Sessions.Commands.Update;
using Application.Sessions.Output;
using Application.Sessions.Queries;
using Domain.Common.Abstractions;
using Domain.Common.ValueObjects.Shared;
using Domain.Sessions;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Presentation.WebApp.Models.Sessions;
using System.Security.Claims;

namespace Presentation.WebApp.Controllers;

[Authorize]
public class OurClassesController(IUnitOfWork unitOfWork, ISender sender) : Controller
{
    private async Task<List<SessionResponse>> GetSessionsListAsync(CancellationToken ct)
    {
        var userIdClaim = User.FindFirstValue(ClaimTypes.NameIdentifier);
        Guid? userId = userIdClaim != null ? Guid.Parse(userIdClaim) : null;

        var result = await sender.Send(new GetSessionsQuery(userId), ct);

        return result.IsSuccess ? result.Value : [];
    }

    [HttpGet]
    public async Task<IActionResult> Index(CancellationToken ct)
    {
        var userIdClaim = User.FindFirstValue(ClaimTypes.NameIdentifier);
        Guid? userId = userIdClaim != null ? Guid.Parse(userIdClaim) : null;

        var result = await sender.Send(new GetSessionsQuery(userId), ct);

        var user = userId.HasValue
            ? await unitOfWork.Users.GetByIdAsync(UserId.Create(userId.Value).Value, ct)
            : null;

        var viewModel = new SessionsViewModel
        {
            Sessions = result.IsSuccess ? result.Value : [],
            Form = new SessionFormViewModel(),

            IsMember = user?.Membership != null
        };

        return View(viewModel);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Book(Guid id, CancellationToken ct)
    {
        var userIdClaim = User.FindFirstValue(ClaimTypes.NameIdentifier);
        if (string.IsNullOrEmpty(userIdClaim)) return RedirectToAction("Login", "Account");

        if (!Guid.TryParse(userIdClaim, out var userId)) return RedirectToAction("Login", "Account");

        var command = new CreateBookingCommand(id, userId, DateTime.UtcNow);
        var result = await sender.Send(command, ct);

        if (result.IsFailure)
            TempData["Error"] = result.Error.Description;

        return RedirectToAction(nameof(Index));
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Cancel(Guid id, bool fromAccount = false, CancellationToken ct = default)
    {
        var userIdClaim = User.FindFirstValue(ClaimTypes.NameIdentifier);
        if (string.IsNullOrEmpty(userIdClaim)) return RedirectToAction("Login", "Account");

        var command = new CancelBookingCommand(id, Guid.Parse(userIdClaim));
        var result = await sender.Send(command, ct);

        if (result.IsFailure)
            TempData["Error"] = "Kunde inte avboka: " + result.Error.Description;

        if (fromAccount)
        {
            return RedirectToAction("Index", "Account", new { viewName = "my-bookings" });
        }

        return RedirectToAction(nameof(Index));
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
                EndTime = s.EndTime,
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

        var startTime = form.StartTime!.Value;
        var endTime = form.EndTime!.Value;
        var utcNow = DateTime.UtcNow;

        if (form.Id is Guid sessionId && sessionId != Guid.Empty)
        {
            var command = new UpdateSessionCommand(
                sessionId,
                form.Title,
                form.Description,
                form.Instructor,
                category,
                startTime,
                endTime,
                form.MaxCapacity,
                utcNow);

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
                startTime,
                endTime,
                form.MaxCapacity,
                utcNow);

            var result = await sender.Send(command, ct);
            if (result.IsFailure)
            {
                ModelState.AddModelError("", result.Error.Description);
                return View("Index", new SessionsViewModel { Sessions = await GetSessionsListAsync(ct), Form = form });
            }
        }

        return RedirectToAction(nameof(Index));
    }

    [HttpPost]
    [Authorize(Roles = "Admin")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Delete(Guid id, CancellationToken ct)
    {
        var command = new DeleteSessionCommand(id, DateTime.UtcNow);

        var result = await sender.Send(command, ct);

        if (result.IsFailure)
        {
            ModelState.AddModelError("DeleteError", result.Error.Description);

            var viewModel = new SessionsViewModel
            {
                Sessions = await GetSessionsListAsync(ct),
                Form = new SessionFormViewModel()
            };

            return View("Index", viewModel);
        }

        return RedirectToAction(nameof(Index));
    }
}