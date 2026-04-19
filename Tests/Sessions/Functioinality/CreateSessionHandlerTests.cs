using Application.Sessions.Commands.Create;
using Domain.Sessions;
using Microsoft.EntityFrameworkCore;
using FluentAssertions;
using Tests.Common;
using Domain.Common.ValueObjects.Shared;

namespace Tests.Sessions.Functioinality;

public class CreateSessionHandlerTests : BaseIntegrationTest
{
    private readonly CreateSessionCommandHandler _handler;

    public CreateSessionHandlerTests()
    {
        _handler = new CreateSessionCommandHandler(UnitOfWork);
    }

    [Fact]
    public async Task Handle_Should_CreateSession_WhenDataIsValid()
    {
        // --- 1. Arrange ---
        var utcNow = DateTime.UtcNow;
        var command = new CreateSessionCommand(
            "Morgonyoga",
            "Ett härligt pass för att starta dagen",
            "Anna Andersson",
            SessionCategory.Yoga,
            utcNow.AddDays(1),
            utcNow.AddDays(1).AddHours(1),
            20,
            utcNow);

        // --- 2. Act ---
        var result = await _handler.Handle(command, CancellationToken.None);

        // --- 3. Assert ---
        result.IsSuccess.Should().BeTrue();
        result.Value.Should().NotBeEmpty();

        var sessionId = SessionId.Create(result.Value).Value;

        var sessionInDb = await Context.Set<SessionEntity>()
            .FirstOrDefaultAsync(s => s.Id == sessionId);

        sessionInDb.Should().NotBeNull();
        sessionInDb!.Title.Value.Should().Be("Morgonyoga");
        sessionInDb.MaxCapacity.Value.Should().Be(20);
        sessionInDb.Category.Should().Be(SessionCategory.Yoga);
    }

    [Fact]
    public async Task Handle_Should_ReturnFailure_WhenTitleIsInvalid()
    {
        // --- 1. Arrange ---
        var command = new CreateSessionCommand(
            "",
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
    }

    [Fact]
    public async Task Handle_Should_ReturnFailure_WhenTimeSlotIsInvalid()
    {
        // --- 1. Arrange ---
        var utcNow = DateTime.UtcNow;
        var command = new CreateSessionCommand(
            "Yoga",
            "Beskrivning",
            "Instruktör",
            SessionCategory.Yoga,
            utcNow.AddDays(1),
            utcNow,
            10,
            utcNow);

        // --- 2. Act ---
        var result = await _handler.Handle(command, CancellationToken.None);

        // --- 3. Assert ---
        result.IsFailure.Should().BeTrue();
    }
}
