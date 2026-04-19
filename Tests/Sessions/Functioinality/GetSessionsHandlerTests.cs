using Application.Sessions.Queries;
using Domain.Sessions;
using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using Tests.Common;

namespace Tests.Sessions.Functioinality;

public class GetSessionsHandlerTests : BaseIntegrationTest
{
    private readonly GetSessionsHandler _handler;

    public GetSessionsHandlerTests()
    {
        _handler = new GetSessionsHandler(UnitOfWork);
    }

    [Fact]
    public async Task Handle_Should_ReturnOnlyFutureAndNonDeletedSessions()
    {
        // --- 1. Arrange ---
        var utcNow = DateTime.UtcNow;
        var futureSession = CreateSession("Yoga Framtid", utcNow.AddDays(1));
        var pastSession = CreateSession("Yoga Dåtid", utcNow.AddDays(2));
        var deletedSession = CreateSession("Yoga Raderad", utcNow.AddDays(3));

        deletedSession.Delete(utcNow);

        Context.Set<SessionEntity>().AddRange(futureSession, pastSession, deletedSession);
        await Context.SaveChangesAsync();

        var schedule = pastSession.Schedule;
        var type = typeof(TimeSlot);
        var startProp = type.GetProperty(nameof(TimeSlot.StartTime));
        var endProp = type.GetProperty(nameof(TimeSlot.EndTime));

        startProp?.SetValue(schedule, utcNow.AddDays(-1));
        endProp?.SetValue(schedule, utcNow.AddDays(-1).AddHours(1));

        Context.Entry(pastSession).State = EntityState.Modified;
        await Context.SaveChangesAsync();

        Context.ChangeTracker.Clear();

        var query = new GetSessionsQuery(null);

        // --- 2. Act ---
        var result = await _handler.Handle(query, CancellationToken.None);

        // --- 3. Assert ---
        result.IsSuccess.Should().BeTrue();
        result.Value.Should().ContainSingle();
        result.Value.First().Title.Should().Be("Yoga Framtid");
    }

    private static SessionEntity CreateSession(string title, DateTime start)
    {
        var creationTime = DateTime.UtcNow.AddMonths(-1);

        return SessionEntity.Create(
            Title.Create(title).Value,
            Description.Create("Besk").Value,
            Instructor.Create("Anna").Value,
            SessionCategory.Yoga,
            TimeSlot.Create(start, start.AddHours(1)).Value,
            Capacity.Create(15).Value,
            creationTime).Value;
    }
}