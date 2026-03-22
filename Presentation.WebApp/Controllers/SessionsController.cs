using Application.Sessions.Commands;
using Application.Sessions.Queries;
using Domain.Common;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace Presentation.WebApp.Controllers;

[ApiController]
[Route("api/[controller]")]
public sealed class SessionsController(ISender sender) : ControllerBase
{
    [HttpGet]
    public async Task<IActionResult> GetAll(CancellationToken ct)
    {
        var result = await sender.Send(new GetSessionsQuery(), ct);

        // Returnerar 200 OK med List<SessionOutput> eller 400 om något mot förmodan brister
        return result.IsSuccess ? Ok(result.Value) : BadRequest(result.Error);
    }

    [HttpGet("{id:guid}")]
    public async Task<IActionResult> GetById(Guid id, CancellationToken ct)
    {
        var result = await sender.Send(new GetSessionByIdQuery(id), ct);

        if (result.IsFailure)
        {
            return result.Error == DomainErrors.Session.NotFound
                ? NotFound(result.Error)
                : BadRequest(result.Error);
        }

        return Ok(result.Value);
    }

    [HttpPost]
    public async Task<IActionResult> Create([FromBody] CreateSessionCommand command, CancellationToken ct)
    {
        var result = await sender.Send(command, ct);

        if (result.IsFailure)
            return BadRequest(result.Error);

        // Skickar tillbaka 201 Created med ID:t
        return CreatedAtAction(nameof(GetById), new { id = result.Value }, result.Value);
    }

    [HttpPut("{id:guid}")]
    public async Task<IActionResult> Update(Guid id, [FromBody] UpdateSessionCommand command, CancellationToken ct)
    {
        if (id != command.Id)
        {
            return BadRequest("ID in URL must match ID in body.");
        }

        var result = await sender.Send(command, ct);

        if (result.IsFailure)
        {
            return result.Error == DomainErrors.Session.NotFound
                ? NotFound(result.Error)
                : BadRequest(result.Error);
        }

        return NoContent(); // 204 Standard för lyckad PUT
    }


    [HttpDelete("{id:guid}")]
    public async Task<IActionResult> Delete(Guid id, CancellationToken ct)
    {
        var result = await sender.Send(new DeleteSessionCommand(id), ct);

        if (result.IsFailure)
        {
            return result.Error == DomainErrors.Session.NotFound
                ? NotFound(result.Error)
                : BadRequest(result.Error);
        }

        return NoContent(); // 204 Standard för lyckad DELETE
    }
}