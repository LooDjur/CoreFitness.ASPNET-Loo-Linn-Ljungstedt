using Domain.Bookings.Entity;
using Domain.Bookings.Enum;
using Domain.Bookings.ValueObjects;
using Domain.Common;
using Domain.Common.ValueObjects.Shared;
using System;
using System.Collections.Generic;
using System.Text;

namespace Tests.Unit.Bookings;

public class BookingTests
{
    [Fact]
    public void Create_ShouldSucceed_WhenValidData()
    {
        var sessionId = new SessionId(Guid.NewGuid());
        var memberId = new MemberId(Guid.NewGuid());

        var result = BookingEntity.Create(sessionId, memberId);

        Assert.True(result.IsSuccess);
        Assert.NotNull(result.Value);
        Assert.Equal(BookingStatus.Confirmed, result.Value.Status);
    }

    [Fact]
    public void Create_ShouldFail_WhenSessionIdIsNull()
    {
        var memberId = new MemberId(Guid.NewGuid());

        var result = BookingEntity.Create(null!, memberId);

        Assert.True(result.IsFailure);
        Assert.Equal(DomainErrors.Validation.Required, result.Error);
    }

    [Fact]
    public void Cancel_ShouldSetStatusToCancelled()
    {
        var sessionId = new SessionId(Guid.NewGuid());
        var memberId = new MemberId(Guid.NewGuid());

        var booking = BookingEntity.Create(sessionId, memberId).Value;

        var result = booking.Cancel();

        Assert.True(result.IsSuccess);
        Assert.Equal(BookingStatus.Cancelled, booking.Status);
    }

    [Fact]
    public void Cancel_ShouldFail_WhenAlreadyCancelled()
    {
        var sessionId = new SessionId(Guid.NewGuid());
        var memberId = new MemberId(Guid.NewGuid());

        var booking = BookingEntity.Create(sessionId, memberId).Value;

        booking.Cancel();

        var result = booking.Cancel();

        Assert.True(result.IsFailure);
        Assert.Equal(DomainErrors.Session.ActionNotAllowed, result.Error);
    }
}
