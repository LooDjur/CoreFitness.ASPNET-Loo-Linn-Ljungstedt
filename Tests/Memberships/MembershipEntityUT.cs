using System;
using System.Collections.Generic;
using System.Text;

using Domain.Common;
using Domain.Common.ValueObjects.Shared;
using Domain.Memberships.Entities;
using Domain.Memberships.Enums;
using Domain.Memberships.ValueObjects;

namespace Tests.Memberships;

public class MembershipEntityUT
{
    // Helpers för att skapa giltiga objekt utan att repetera logik
    private static MemberId ValidMemberId => MemberId.New();
    private static FirstName ValidFirstName => FirstName.Create("John").Value;
    private static LastName ValidLastName => LastName.Create("Doe").Value;
    private static Email ValidEmail => Email.Create("john@test.com").Value;

    [Fact]
    public void Create_Should_ReturnSuccess_And_MapIdCorrectly()
    {
        // Act
        var result = MembershipEntity.Create(ValidFirstName, ValidLastName, ValidEmail);

        // Assert
        Assert.True(result.IsSuccess);
        Assert.NotEqual(Guid.Empty, (Guid)result.Value.Id); 
        Assert.Equal(MembershipStatus.Active, result.Value.Status);
    }

    [Fact]
    public void UpdateProfile_Should_ReturnSuccess_WhenActive()
    {
        // Arrange
        var membership = MembershipEntity.Create(ValidFirstName, ValidLastName, ValidEmail).Value;
        var newFirstName = FirstName.Create("Jane").Value;
        var newLastName = LastName.Create("Doe").Value;
        var newEmail = Email.Create("jane@test.com").Value;

        // Act
        var result = membership.UpdateProfile(newFirstName, newLastName, newEmail, "image.png");

        // Assert
        Assert.True(result.IsSuccess);
        Assert.Equal("Jane", membership.FirstName.Value);
        Assert.Equal("image.png", membership.ProfileImageUrl);
    }

    [Fact]
    public void UpdateProfile_Should_Fail_WhenSuspended()
    {
        // Arrange
        var membership = MembershipEntity.Create(ValidFirstName, ValidLastName, ValidEmail).Value;
        membership.AdminUpdateStatus(MembershipStatus.Suspended);

        // Act
        var result = membership.UpdateProfile(ValidFirstName, ValidLastName, ValidEmail, null);

        // Assert
        Assert.True(result.IsFailure);
        Assert.Equal(DomainErrors.Membership.Ineligible, result.Error);
    }

    [Fact]
    public void AdminExtendMembership_Should_IncreaseExpiryDate()
    {
        // Arrange
        var membership = MembershipEntity.Create(ValidFirstName, ValidLastName, ValidEmail).Value;
        var originalExpiry = membership.ExpiryDate;

        // Act
        var result = membership.AdminExtendMembership(1);

        // Assert
        Assert.True(result.IsSuccess);
        Assert.True(membership.ExpiryDate > originalExpiry);
    }

    [Fact]
    public void AdminExtendMembership_Should_Fail_WhenMonthsAreInvalid()
    {
        // Arrange
        var membership = MembershipEntity.Create(ValidFirstName, ValidLastName, ValidEmail).Value;

        // Act & Assert (Vi testar både 0 och för många månader enligt din logik)
        var resultZero = membership.AdminExtendMembership(0);
        var resultTooMany = membership.AdminExtendMembership(5); // Gränsen var 2

        Assert.True(resultZero.IsFailure);
        Assert.Equal(DomainErrors.Validation.InvalidFormat, resultZero.Error);

        Assert.True(resultTooMany.IsFailure);
        Assert.Equal(DomainErrors.Membership.LimitReached, resultTooMany.Error);
    }

    [Fact]
    public void AdminUpdateStatus_Should_Fail_WhenSameStatus()
    {
        // Arrange
        var membership = MembershipEntity.Create(ValidFirstName, ValidLastName, ValidEmail).Value;

        // Act
        var result = membership.AdminUpdateStatus(MembershipStatus.Active);

        // Assert
        Assert.True(result.IsFailure);
        Assert.Equal(DomainErrors.Membership.Ineligible, result.Error);
    }

    [Fact]
    public void AdminChangeType_Should_Fail_WhenSameType()
    {
        // Arrange
        var membership = MembershipEntity.Create(ValidFirstName, ValidLastName, ValidEmail).Value;

        // Act
        var result = membership.AdminChangeType(MembershipType.Standard);

        // Assert
        Assert.True(result.IsFailure);
        Assert.Equal(DomainErrors.Membership.Ineligible, result.Error);
    }
}
