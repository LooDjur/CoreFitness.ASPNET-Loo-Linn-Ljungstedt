using Domain.Common;
using Domain.Common.Exceptions;
using Domain.Sessions.Entities;
using Domain.Sessions.Enums;
using Domain.Sessions.ValueObjects;

namespace Tests.Sessions;

public class SessionEntityTests
{
    private static Title GetValidTitle() => Title.Create("Yoga Flow").Value;
    private static Instructor GetValidInstructor() => Instructor.Create("Jane Doe").Value;
    private static Description GetValidDescription() => Description.Create("Ett avslappnande pass.").Value;
    private static Capacity GetValidCapacity() => Capacity.Create(20).Value;

    private static TimeSlot GetFutureSchedule()
    {
        var now = DateTime.UtcNow;
        // Här använder vi .Value eftersom vi VET att datan är giltig i vår helper
        return TimeSlot.Create(now.AddDays(1), now.AddDays(1).AddHours(1)).Value;
    }

    [Fact]
    public void Create_Should_ReturnFailure_When_CapacityIsInvalid()
    {
        // Act
        // Vi testar Value Objectets validering direkt - ingen exception kastas!
        var result = Capacity.Create(-5);

        // Assert
        Assert.True(result.IsFailure);
        Assert.Equal(DomainErrors.Session.InvalidCapacity, result.Error);
    }

    [Fact]
    public void Create_Should_ReturnSuccess_When_AllDataIsValid()
    {
        // Act
        var result = SessionEntity.Create(
            GetValidTitle(),
            GetValidDescription(),
            GetValidInstructor(),
            SessionCategory.Yoga,
            GetFutureSchedule(),
            GetValidCapacity());
            

        // Assert
        Assert.True(result.IsSuccess);
        Assert.NotNull(result.Value);
        Assert.Equal("Yoga Flow", result.Value.Title.Value);
    }

    [Fact]
    public void Should_ThrowException_When_TryingToCreateTimeSlotInPast()
    {
        var pastStart = DateTime.UtcNow.AddHours(-2);
        var pastEnd = DateTime.UtcNow.AddHours(-1);

        var result = TimeSlot.Create(pastStart, pastEnd);

        Assert.True(result.IsFailure);
        Assert.Equal(DomainErrors.Session.InvalidDate, result.Error);
    }

    [Fact]
    public void Delete_Should_ReturnSuccess_When_SessionIsInFuture()
    {
        // 1. Skapa en tid LÅNGT fram
        var futureDate = DateTime.UtcNow.AddDays(10);
        var futureSchedule = TimeSlot.Create(futureDate, futureDate.AddHours(1)).Value;

        var sessionResult = SessionEntity.Create(
            GetValidTitle(),
            GetValidDescription(),
            GetValidInstructor(),
            SessionCategory.Yoga,
            futureSchedule,
            GetValidCapacity());

        var session = sessionResult.Value;

        // 2. Kontrollera tillståndet INNAN Delete
        Assert.False(session.IsDeleted);

        // 3. Act
        var result = session.Delete();

        // Assert
        Assert.True(result.IsSuccess);
        Assert.True(session.IsDeleted);

        // Vi dubbelkollar även att felet INTE är ActionNotAllowed om något mot förmodan skulle ändras
        Assert.NotEqual(DomainErrors.Session.ActionNotAllowed, result.Error);
    }
}