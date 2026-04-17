using FluentAssertions;
using Domain.Common.ValueObjects.Shared;
using Domain.Users.Entities;
using Domain.Users.Enums;

namespace Tests.Users.User;

public class UserEntityTests
{
    private readonly UserId _userId = UserId.Create(Guid.NewGuid()).Value;
    private readonly Email _email = Email.Create("test@fitness.se").Value;
    private readonly DateTime _utcNow = DateTime.UtcNow;

    // --- REGISTRERING ---

    [Fact]
    public void Register_Should_AssignCorrectRole()
    {
        // Act
        var user = UserEntity.Register(_userId, _email, _utcNow, UserRole.Admin);

        // Assert
        user.Role.Should().Be(UserRole.Admin);
    }

    // --- PROFIL ---

    [Fact]
    public void CompleteProfile_Should_UpdateProperties_And_ReturnSuccess()
    {
        // Arrange
        var user = UserEntity.Register(_userId, _email, _utcNow);
        var firstName = FirstName.Create("Kalle").Value;
        var lastName = LastName.Create("Anka").Value;

        // Act
        var result = user.CompleteProfile(firstName, lastName, null, _utcNow);

        // Assert
        result.IsSuccess.Should().BeTrue();
        user.FirstName.Should().Be(firstName);
        user.LastName.Should().Be(lastName);
    }

    // --- MEDLEMSKAP ---

    [Fact]
    public void StartMembership_Should_CreateNewMembership_When_NoneExists()
    {
        // Arrange
        var user = UserEntity.Register(_userId, _email, _utcNow);

        // Act
        var result = user.StartMembership(MembershipType.Standard, _utcNow);

        // Assert
        result.IsSuccess.Should().BeTrue();
        user.Membership.Should().NotBeNull();
        user.Membership!.Type.Should().Be(MembershipType.Standard);
    }

    [Fact]
    public void StartMembership_Should_Reactivate_When_MembershipIsDeleted()
    {
        // Arrange
        var user = UserEntity.Register(_userId, _email, _utcNow);
        user.StartMembership(MembershipType.Standard, _utcNow);
        user.CancelMembership(_utcNow);

        // Act
        var result = user.StartMembership(MembershipType.Standard, _utcNow.AddDays(1));

        // Assert
        result.IsSuccess.Should().BeTrue();
        user.Membership!.IsDeleted.Should().Be(false);
        user.Membership.Status.Should().Be(MembershipStatus.Active);
    }

    [Fact]
    public void CancelMembership_Should_ReturnFailure_When_NoMembershipToCancel()
    {
        // Arrange
        var user = UserEntity.Register(_userId, _email, _utcNow);

        // Act
        var result = user.CancelMembership(_utcNow);

        // Assert
        result.IsFailure.Should().BeTrue();
    }
}