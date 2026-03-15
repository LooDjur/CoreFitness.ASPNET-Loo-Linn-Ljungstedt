using System;
using System.Collections.Generic;
using System.Text;

using Domain.Common;
using Domain.Common.ValueObjects.Shared;
using Domain.Memberships.Entities;
using Domain.Memberships.Enums;
using Domain.Memberships.ValueObjects;

namespace Tests.Unit.Memberships;
public class MembershipEntityTests
{
    private MemberId MemberId => new MemberId(Guid.NewGuid());
    private FirstName FirstName => new FirstName("John");
    private LastName LastName => new LastName("Doe");
    private Email Email => new Email("john@test.com");

    [Fact]
    public void Create_ShouldReturnSuccess()
    {
        var result = MembershipEntity.Create(MemberId, FirstName, LastName, Email);

        Assert.True(result.IsSuccess);
        Assert.NotNull(result.Value);
        Assert.Equal(MembershipStatus.Active, result.Value.Status);
    }

    [Fact]
    public void UpdateProfile_ShouldReturnSuccess_WhenActive()
    {
        var membership = MembershipEntity.Create(MemberId, FirstName, LastName, Email).Value;

        var result = membership.UpdateProfile(
            new FirstName("Jane"),
            new LastName("Doe"),
            new Email("jane@test.com"),
            "image.png");

        Assert.True(result.IsSuccess);
        Assert.Equal("image.png", membership.ProfileImageUrl);
    }

    [Fact]
    public void UpdateProfile_ShouldFail_WhenSuspended()
    {
        var membership = MembershipEntity.Create(MemberId, FirstName, LastName, Email).Value;

        membership.AdminUpdateStatus(MembershipStatus.Suspended);

        var result = membership.UpdateProfile(
            new FirstName("Jane"),
            new LastName("Doe"),
            new Email("jane@test.com"),
            null);

        Assert.True(result.IsFailure);
        Assert.Equal(DomainErrors.Membership.Ineligible, result.Error);
    }

    [Fact]
    public void AdminExtendMembership_ShouldIncreaseExpiryDate()
    {
        var membership = MembershipEntity.Create(MemberId, FirstName, LastName, Email).Value;
        var originalExpiry = membership.ExpiryDate;

        var result = membership.AdminExtendMembership(1);

        Assert.True(result.IsSuccess);
        Assert.True(membership.ExpiryDate > originalExpiry);
    }

    [Fact]
    public void AdminExtendMembership_ShouldFail_WhenMonthsInvalid()
    {
        var membership = MembershipEntity.Create(MemberId, FirstName, LastName, Email).Value;

        var result = membership.AdminExtendMembership(0);

        Assert.True(result.IsFailure);
        Assert.Equal(DomainErrors.Validation.InvalidFormat, result.Error);
    }

    [Fact]
    public void AdminUpdateStatus_ShouldFail_WhenSameStatus()
    {
        var membership = MembershipEntity.Create(MemberId, FirstName, LastName, Email).Value;

        var result = membership.AdminUpdateStatus(MembershipStatus.Active);

        Assert.True(result.IsFailure);
    }

    [Fact]
    public void AdminChangeType_ShouldFail_WhenSameType()
    {
        var membership = MembershipEntity.Create(MemberId, FirstName, LastName, Email).Value;

        var result = membership.AdminChangeType(MembershipType.Standard);

        Assert.True(result.IsFailure);
    }
}
