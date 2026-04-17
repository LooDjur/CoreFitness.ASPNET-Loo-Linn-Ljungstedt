using Application.Sessions.Commands.Update;
using Domain.Common;
using Domain.Sessions;
using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using Tests.Common;

namespace Tests.Sessions.Functioinality;

public class UpdateSessionHandlerTests : BaseIntegrationTest
{
    private readonly UpdateSessionCommandHandler _handler;

    public UpdateSessionHandlerTests()
    {
        _handler = new UpdateSessionCommandHandler(UnitOfWork);
    }

    [Fact]
    public async Task Handle_Should_UpdateSessionDetails_WhenDataIsValid()
    {
        // --- 1. Arrange ---
        var baseDate = DateTime.UtcNow.AddDays(5).Date;
        var originalStart = baseDate.AddHours(10);
        var originalEnd = baseDate.AddHours(11);
        var creationTime = DateTime.UtcNow;

        var session = SessionEntity.Create(
            Title.Create("Gammal Titel").Value,
            Description.Create("Besk").Value,
            Instructor.Create("Anna").Value,
            SessionCategory.Yoga,
            TimeSlot.Create(originalStart, originalEnd).Value,
            Capacity.Create(10).Value,
            creationTime).Value;

        Context.Set<SessionEntity>().Add(session);
        await Context.SaveChangesAsync();

        var newStart = baseDate.AddHours(14);
        var newEnd = baseDate.AddHours(15);

        var command = new UpdateSessionCommand(
            session.Id.Value,
            "Uppdaterad Yoga",
            "Ny beskrivning",
            "Ny Instruktör",
            SessionCategory.Yoga,
            newStart,
            newEnd,
            20,
            creationTime);

        // --- 2. Act ---
        var result = await _handler.Handle(command, CancellationToken.None);

        // --- 3. Assert ---
        result.IsSuccess.Should().BeTrue(result.Error.Description ?? result.Error.Code);

        Context.ChangeTracker.Clear();
        var sessionInDb = await Context.Set<SessionEntity>().FindAsync(session.Id);
        sessionInDb!.Title.Value.Should().Be("Uppdaterad Yoga");
    }

    [Fact]
    public async Task Handle_Should_ReturnFailure_WhenTryingToUpdatePastSession()
    {
        // --- 1. Arrange ---
        var futureStart = DateTime.UtcNow.AddDays(2);

        var session = SessionEntity.Create(
            Title.Create("Gammalt Pass").Value,
            Description.Create("Besk").Value,
            Instructor.Create("Anna").Value,
            SessionCategory.Yoga,
            TimeSlot.Create(futureStart, futureStart.AddHours(1)).Value,
            Capacity.Create(10).Value,
            DateTime.UtcNow).Value;

        Context.Set<SessionEntity>().Add(session);
        await Context.SaveChangesAsync();

        // --- 2. Act ---
        var fakeNowInFuture = futureStart.AddDays(1);

        var command = new UpdateSessionCommand(
            session.Id.Value,
            "Försök uppdatera",
            "Besk",
            "Anna",
            SessionCategory.Yoga,
            futureStart.AddDays(2),
            futureStart.AddDays(2).AddHours(1),
            10,
            fakeNowInFuture);

        var result = await _handler.Handle(command, CancellationToken.None);

        // --- 3. Assert ---
        result.IsFailure.Should().BeTrue();
        result.Error.Should().Be(DomainErrors.Session.ActionNotAllowed);
    }
    [Fact]
    public async Task Handle_Should_ReturnFailure_WhenSessionNotFound()
    {
        // --- 1. Arrange ---
        var command = new UpdateSessionCommand(
            Guid.NewGuid(),
            "Titel",
            "Beskrivning",
            "Instruktör",
            SessionCategory.Yoga,
            DateTime.UtcNow.AddDays(1),
            DateTime.UtcNow.AddDays(1).AddHours(1),
            10,
            DateTime.UtcNow);

        // --- 2. Act ---
        var result = await _handler.Handle(command, CancellationToken.None);

        // --- 3. Assert ---
        result.IsFailure.Should().BeTrue();
        result.Error.Should().Be(DomainErrors.Session.NotFound);
    }

    [Fact]
    public async Task Handle_Should_ReturnFailure_WhenValidationFails()
    {
        // --- 1. Arrange ---
        var utcNow = DateTime.UtcNow;
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

        var command = new UpdateSessionCommand(
            session.Id.Value,
            "",
            "Beskrivning",
            "Instruktör",
            SessionCategory.Yoga,
            utcNow.AddDays(1),
            utcNow.AddDays(1).AddHours(1),
            10,
            utcNow);

        // --- 2. Act ---
        var result = await _handler.Handle(command, CancellationToken.None);

        // --- 3. Assert ---
        result.IsFailure.Should().BeTrue();
    }
}