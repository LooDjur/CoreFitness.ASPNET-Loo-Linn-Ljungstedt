//using Domain.Bookings.Entity;
//using Domain.Bookings.Enum;
//using Domain.Bookings.ValueObjects;
//using Domain.Common;
//using Domain.Common.ValueObjects.Shared;
//using FluentAssertions;
//using System;
//using System.Collections.Generic;
//using System.Text;

//namespace Tests.Bookings;

//public class BookingEntityUT
//{
//    [Fact]
//    public void Create_Should_ReturnSuccess_When_DataIsValid()
//    {
//        // Arrange
//        var sessionId = SessionId.New();
//        var memberId = MemberId.New();

//        // Act
//        var result = BookingEntity.Create(sessionId, memberId);

//        // Assert
//        result.IsSuccess.Should().BeTrue();
//        result.Value.SessionId.Should().Be(sessionId);
//        result.Value.MemberId.Should().Be(memberId);
//        result.Value.Status.Should().Be(BookingStatus.Confirmed);
//        result.Value.Id.Value.Should().NotBeEmpty(); // Kontrollera att BaseEntity satte ett ID
//    }

//    [Fact]
//    public void Create_Should_ReturnFailure_When_SessionIdIsInvalid()
//    {
//        // Arrange
//        var memberId = MemberId.New();
//        // Vi simulerar ett ogiltigt ID genom att försöka skapa ett från en tom Guid
//        // Men eftersom BookingEntity.Create kollar null, testar vi det först:

//        // Act
//        var result = BookingEntity.Create(null!, memberId);

//        // Assert
//        result.IsFailure.Should().BeTrue();
//        result.Error.Should().Be(DomainErrors.Session.NotFound);
//    }

//    [Fact]
//    public void Cancel_Should_UpdateStatusAndModifiedDate()
//    {
//        // Arrange
//        // Vi använder .Value här för att vi VET att Create fungerar i detta testfall
//        var booking = BookingEntity.Create(SessionId.New(), MemberId.New()).Value;

//        // Act
//        var result = booking.Cancel();

//        // Assert
//        result.IsSuccess.Should().BeTrue();
//        booking.Status.Should().Be(BookingStatus.Cancelled);
//        booking.Modified.Should().NotBeNull();
//        // Eftersom DateTime.UtcNow går fort kan vi kolla att det är "nu"
//        booking.Modified.Should().BeCloseTo(DateTime.UtcNow, precision: TimeSpan.FromMilliseconds(500));
//    }

//    [Fact]
//    public void Cancel_Should_ReturnFailure_When_AlreadyCancelled()
//    {
//        // Arrange
//        var booking = BookingEntity.Create(SessionId.New(), MemberId.New()).Value;
//        booking.Cancel(); // Första avbokningen

//        // Act
//        var result = booking.Cancel(); // Andra avbokningen

//        // Assert
//        result.IsFailure.Should().BeTrue();
//        result.Error.Should().Be(DomainErrors.Session.ActionNotAllowed);
//    }
//}
