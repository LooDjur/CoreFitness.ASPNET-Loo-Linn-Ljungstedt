using Application.Sessions.Commands.Delete;
using Domain.Common;
using Domain.Common.ValueObjects.Shared;
using Domain.Sessions;
using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using Tests.Common;

namespace Tests.Sessions.Functioinality;

public class DeleteSessionHandlerTests : BaseIntegrationTest
{
    private readonly DeleteSessionCommandHandler _handler;

    public DeleteSessionHandlerTests()
    {
        _handler = new DeleteSessionCommandHandler(UnitOfWork);
    }

    [Fact]
    public async Task Handle_Should_MarkSessionAsDeleted_WhenSessionIsInFuture()
    {
        // --- 1. Arrange ---
        var utcNow = DateTime.UtcNow;
        var sessionId = SessionId.New();

        var session = SessionEntity.Create(
            Title.Create("Yoga").Value,
            Description.Create("Desc").Value,
            Instructor.Create("Anna").Value,
            SessionCategory.Yoga,
            TimeSlot.Create(utcNow.AddDays(1), utcNow.AddDays(1).AddHours(1)).Value,
            Capacity.Create(10).Value,
            utcNow).Value;

        Context.Set<SessionEntity>().Add(session);
        await Context.SaveChangesAsync();

        var command = new DeleteSessionCommand(session.Id.Value, utcNow);

        // --- 2. Act ---
        var result = await _handler.Handle(command, CancellationToken.None);

        // --- 3. Assert ---
        result.IsSuccess.Should().BeTrue();

        Context.ChangeTracker.Clear();

        var sessionInDb = await Context.Set<SessionEntity>()
            .IgnoreQueryFilters()
            .FirstOrDefaultAsync(s => s.Id == session.Id);

        sessionInDb.Should().NotBeNull();
        sessionInDb!.IsDeleted.Should().BeTrue();
        sessionInDb.Modified.Should().Be(utcNow);
    }

    [Fact]
    public async Task Handle_Should_ReturnFailure_WhenSessionIsAlreadyInThePast()
    {
        // --- 1. Arrange ---
        var sessionStartTime = DateTime.UtcNow.AddSeconds(5);
        var utcNowAtCreation = DateTime.UtcNow;

        var sessionResult = SessionEntity.Create(
            Title.Create("Gammalt Pass").Value,
            Description.Create("Desc").Value,
            Instructor.Create("Anna").Value,
            SessionCategory.Yoga,
            TimeSlot.Create(sessionStartTime, sessionStartTime.AddHours(1)).Value,
            Capacity.Create(10).Value,
            utcNowAtCreation);

        var session = sessionResult.Value;
        Context.Set<SessionEntity>().Add(session);
        await Context.SaveChangesAsync();

        var nowWhenDeleting = sessionStartTime.AddMinutes(1);
        var command = new DeleteSessionCommand(session.Id.Value, nowWhenDeleting);

        // --- 2. Act ---
        var result = await _handler.Handle(command, CancellationToken.None);

        // --- 3. Assert ---
        result.IsFailure.Should().BeTrue();
        result.Error.Should().Be(DomainErrors.Session.ActionNotAllowed);
    }

    [Fact]
    public async Task Handle_Should_ReturnFailure_WhenSessionNotFound()
    {
        // --- Arrange ---
        var command = new DeleteSessionCommand(Guid.NewGuid(), DateTime.UtcNow);

        // --- Act ---
        var result = await _handler.Handle(command, CancellationToken.None);

        // --- Assert ---
        result.IsFailure.Should().BeTrue();
        result.Error.Should().Be(DomainErrors.Session.NotFound);
    }
}