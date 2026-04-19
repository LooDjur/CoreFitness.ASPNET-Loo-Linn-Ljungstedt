using Domain.Common;
using Domain.Sessions;
using FluentAssertions;

namespace Tests.Sessions;

public class SessionEntityTests
{

    private static SessionEntity CreateValidSession(DateTime? startTime = null)
    {
        var start = startTime ?? DateTime.UtcNow.AddDays(1);

        var result = SessionEntity.Create(
            Title.Create("Yoga Flow").Value,
            Description.Create("Ett lugnt pass").Value,
            Instructor.Create("Anna").Value,
            SessionCategory.Yoga,
            TimeSlot.Create(start, start.AddHours(1)).Value,
            Capacity.Create(15).Value,
            DateTime.UtcNow.AddDays(-1));

        return result.Value;
    }

    [Fact]
    public void Create_Should_ReturnSuccess_WhenDataIsValid()
    {
        // Arrange & Act
        var result = SessionEntity.Create(
            Title.Create("Pilates").Value,
            Description.Create("Stärkande").Value,
            Instructor.Create("Erik").Value,
            SessionCategory.Yoga,
            TimeSlot.Create(DateTime.UtcNow.AddDays(1), DateTime.UtcNow.AddDays(1).AddHours(1)).Value,
            Capacity.Create(20).Value,
            DateTime.UtcNow);

        // Assert
        result.IsSuccess.Should().BeTrue();
        result.Value.Title.Value.Should().Be("Pilates");
        result.Value.MaxCapacity.Value.Should().Be(20);
    }

    [Fact]
    public void Book_Should_ReturnFailure_WhenSessionIsFull()
    {
        // Arrange
        var session = CreateValidSession();
        var currentBookingsCount = 15;
        var utcNow = DateTime.UtcNow;

        // Act
        var result = session.Book(currentBookingsCount, utcNow);

        // Assert
        result.IsFailure.Should().BeTrue();
        result.Error.Should().Be(DomainErrors.Session.InvalidCapacity);
    }

    [Fact]
    public void Book_Should_ReturnFailure_WhenSessionHasAlreadyStarted()
    {
        // Arrange
        var futureDate = DateTime.UtcNow.AddDays(1);
        var session = CreateValidSession(futureDate);

        // Act
        var result = session.Book(0, futureDate.AddHours(1));

        // Assert
        result.IsFailure.Should().BeTrue();
        result.Error.Should().Be(DomainErrors.Session.ActionNotAllowed);
    }

    [Fact]
    public void Book_Should_ReturnSuccess_WhenCapacityExistsAndInFuture()
    {
        // Arrange
        var session = CreateValidSession();
        var currentBookingsCount = 5;
        var utcNow = DateTime.UtcNow;

        // Act
        var result = session.Book(currentBookingsCount, utcNow);

        // Assert
        result.IsSuccess.Should().BeTrue();
        result.Value.Should().NotBeNull();
    }

    [Fact]
    public void Delete_Should_ReturnFailure_WhenSessionIsAlreadyInThePast()
    {
        // Arrange
        var futureDate = DateTime.UtcNow.AddDays(1);
        var session = CreateValidSession(futureDate);

        // Act
        var result = session.Delete(futureDate.AddHours(1));

        // Assert
        result.IsFailure.Should().BeTrue();
        result.Error.Should().Be(DomainErrors.Session.ActionNotAllowed);
    }

    [Fact]
    public void Delete_Should_SetIsDeletedToTrue_WhenSessionIsInFuture()
    {
        // Arrange
        var session = CreateValidSession();
        var utcNow = DateTime.UtcNow;

        // Act
        var result = session.Delete(utcNow);

        // Assert
        result.IsSuccess.Should().BeTrue();
        session.IsDeleted.Should().BeTrue();
    }

    [Fact]
    public void UpdateDetails_Should_UpdateAllProperties()
    {
        // Arrange
        var session = CreateValidSession(DateTime.UtcNow.AddDays(2));
        var newTitle = Title.Create("Ny Titel").Value;
        var newCapacity = Capacity.Create(20).Value;
        var utcNow = DateTime.UtcNow;

        // Act
        var result = session.UpdateDetails(
            newTitle,
            session.Description,
            session.Instructor,
            session.Category,
            session.Schedule,
            newCapacity,
            utcNow);

        // Assert
        result.IsSuccess.Should().BeTrue();
        session.Title.Should().Be(newTitle);
        session.MaxCapacity.Should().Be(newCapacity);
        session.Modified.Should().Be(utcNow);
    }
}
