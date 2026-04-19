using Domain.Common.ValueObjects.Shared;
using Domain.Users.Entities;
using Domain.Users.Enums;
using FluentAssertions;

namespace Tests.Users.Membership;

public class MembershipEntityTests
{
    private readonly UserId _userId = UserId.Create(Guid.NewGuid()).Value;
    private readonly DateTime _utcNow = DateTime.UtcNow;

    [Fact]
    public void Cancel_Should_SetStatusToCancelled_And_IsDeletedToTrue()
    {
        var membership = MembershipEntity.CreateInternal(_userId, MembershipType.Standard, _utcNow).Value;

        // Act
        membership.Cancel(_utcNow);

        // Assert
        membership.Status.Should().Be(MembershipStatus.Cancelled);
        membership.IsDeleted.Should().BeTrue();
        membership.Modified.Should().BeCloseTo(_utcNow, TimeSpan.FromMilliseconds(100));
    }

    [Fact]
    public void Reactivate_Should_ResetStatusToActive_And_SetNewExpiryDate()
    {
        // Arrange 
        var membership = MembershipEntity.CreateInternal(_userId, MembershipType.Standard, _utcNow).Value;
        membership.Cancel(_utcNow);

        membership.Reactivate(MembershipType.Standard, _utcNow);

        // Assert
        membership.IsDeleted.Should().BeFalse();
        membership.Status.Should().Be(MembershipStatus.Active);
        membership.Type.Should().Be(MembershipType.Standard);
        membership.ExpiryDate.Should().BeCloseTo(_utcNow.AddYears(1), TimeSpan.FromSeconds(1));
    }

    [Fact]
    public void IsEligibleToBook_Should_ReturnFalse_When_Expired()
    {
        // Arrange
        var membership = MembershipEntity.CreateInternal(_userId, MembershipType.Standard, _utcNow).Value;

        // Act
        var result = membership.IsEligibleToBook(_utcNow.AddYears(2));

        // Assert
        result.Should().BeFalse();
    }

    [Fact]
    public void AdminExtend_Should_ReturnFailure_When_ExtendingTooManyMonths()
    {
        // Arrange
        var membership = MembershipEntity.CreateInternal(_userId, MembershipType.Standard, _utcNow).Value;

        // Act
        var result = membership.AdminExtend(3, _utcNow); // Max är 2 i din kod

        // Assert
        result.IsFailure.Should().BeTrue();
    }

    [Fact]
    public void ChangeType_Should_ReturnFailure_When_SameType()
    {
        // Arrange
        var membership = MembershipEntity.CreateInternal(_userId, MembershipType.Standard, _utcNow).Value;

        // Act
        var result = membership.ChangeType(MembershipType.Standard, _utcNow);

        // Assert
        result.IsFailure.Should().BeTrue();
    }
}