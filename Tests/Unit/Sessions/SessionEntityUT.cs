using Domain.Common;
using Domain.Common.Exceptions;
using Domain.Sessions.Entities;
using Domain.Sessions.Enums;
using Domain.Sessions.ValueObjects;

namespace Tests.Unit.Sessions;

public class SessionEntityTests
{
    private Title ValidTitle => new Title("Yoga Flow");
    private Instructor ValidInstructor => new Instructor("Jane Doe");
    private Description ValidDescription => new Description("Ett avslappnande pass.");
    private Capacity ValidCapacity => new Capacity(20);
    private TimeSlot GetFutureSchedule()
    {
        var now = DateTime.UtcNow;
        return new TimeSlot(now.AddDays(1), now.AddDays(1).AddHours(1));
    }

    [Fact]
    public void Create_ValidData_ShouldReturnSuccess()
    {
        var result = SessionEntity.Create(
            ValidTitle,
            ValidInstructor,
            SessionCategory.Yoga,
            GetFutureSchedule(),
            ValidCapacity,
            ValidDescription);

        Assert.True(result.IsSuccess);
        Assert.NotNull(result.Value);
        Assert.Equal(ValidTitle, result.Value.Title);
    }

    [Fact]
    public void Create_ShouldReturnFailure_WhenDateIsInThePast()
    {
        // Arrange
        var now = DateTime.UtcNow;

        var pastSchedule = new TimeSlot(
            now.AddDays(-1),
            now.AddDays(-1).AddHours(1));

        // Act
        var result = SessionEntity.Create(
            ValidTitle,
            ValidInstructor,
            SessionCategory.Yoga,
            pastSchedule,
            ValidCapacity,
            ValidDescription);

        // Assert
        Assert.True(result.IsFailure);
        Assert.Equal(DomainErrors.Session.InvalidDate, result.Error);
    }

    [Fact]
    public void Delete_ShouldReturnSuccess_WhenSessionIsInFuture()
    {
        // Arrange
        var session = SessionEntity.Create(
            ValidTitle,
            ValidInstructor,
            SessionCategory.Yoga,
            GetFutureSchedule(),
            ValidCapacity,
            ValidDescription).Value;

        // Act
        var result = session.Delete();

        // Assert
        Assert.True(result.IsSuccess);
        Assert.True(session.IsDeleted);
    }

    [Fact]
    public void Delete_ShouldReturnFailure_WhenSessionAlreadyDeleted()
    {
        // Arrange
        var session = SessionEntity.Create(
            ValidTitle,
            ValidInstructor,
            SessionCategory.Yoga,
            GetFutureSchedule(),
            ValidCapacity,
            ValidDescription).Value;

        session.Delete();

        // Act
        var result = session.Delete();

        // Assert
        Assert.True(result.IsFailure);
        Assert.Equal(DomainErrors.Session.ActionNotAllowed, result.Error);
    }

    [Fact]
    public void UpdateDetails_ShouldReturnFailure_WhenNewDateIsInPast()
    {
        // Arrange
        var session = SessionEntity.Create(
            ValidTitle,
            ValidInstructor,
            SessionCategory.Yoga,
            GetFutureSchedule(),
            ValidCapacity,
            ValidDescription).Value;

        var now = DateTime.UtcNow;

        var invalidSchedule = new TimeSlot(
            now.AddDays(-2),
            now.AddDays(-2).AddHours(1));

        // Act
        var result = session.UpdateDetails(
            ValidTitle,
            ValidInstructor,
            ValidDescription,
            SessionCategory.HIIT,
            invalidSchedule,
            ValidCapacity);

        // Assert
        Assert.True(result.IsFailure);
        Assert.Equal(DomainErrors.Session.InvalidDate, result.Error);
    }

    [Fact]
    public void TimeSlot_ShouldThrow_WhenStartIsInPast()
    {
        var now = DateTime.UtcNow;

        var ex = Assert.Throws<DomainException>(() =>
            new TimeSlot(now.AddDays(-1), now.AddDays(-1).AddHours(1)));

        Assert.Equal(DomainErrors.Session.InvalidDate, ex.Error);
    }
    [Fact]
    public void Title_ShouldThrow_WhenTooShort()
    {
        var ex = Assert.Throws<DomainException>(() => new Title("ab"));
        Assert.Equal(DomainErrors.Validation.InvalidFormat, ex.Error);
    }

    [Fact]
    public void Title_ShouldThrow_WhenEmpty()
    {
        var ex = Assert.Throws<DomainException>(() => new Title(""));
        Assert.Equal(DomainErrors.Validation.Required, ex.Error);
    }

    [Fact]
    public void Capacity_ShouldThrow_WhenBelowMinimum()
    {
        var ex = Assert.Throws<DomainException>(() => new Capacity(5));
        Assert.Equal(DomainErrors.Session.InvalidCapacity, ex.Error);
    }
}