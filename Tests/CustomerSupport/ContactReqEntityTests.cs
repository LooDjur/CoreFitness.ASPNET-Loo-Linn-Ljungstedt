using Domain.Common;
using Domain.Common.ValueObjects.Shared;
using Domain.ContactReq.Entities;
using Domain.ContactReq.ValueObjects;
using FluentAssertions;

namespace Tests.CustomerSupport;

public class ContactRequestEntityTests
{
    [Fact]
    public void Create_Should_ReturnSuccess_WhenDataIsValid()
    {
        // --- Arrange ---
        var firstName = FirstName.Create("Kalle").Value;
        var lastName = LastName.Create("Anka").Value;
        var email = Email.Create("kalle@ankeborg.se").Value;
        var phone = PhoneNumber.Create("0701234567").Value;
        var message = MessageBody.Create("Jag vill boka ett PT-pass.").Value;

        // --- Act ---
        var result = ContactRequestEntity.Create(firstName, lastName, email, phone, message);

        // --- Assert ---
        result.IsSuccess.Should().BeTrue();
        result.Value.FirstName.Should().Be(firstName);
        result.Value.Email.Should().Be(email);
        result.Value.Id.Should().NotBeNull();
    }

    [Fact]
    public void Create_Should_AssignNewId_WhenEntityIsCreated()
    {
        // --- Arrange & Act ---
        var result = CreateValidContactRequest();

        // --- Assert ---
        result.Value.Id.Value.Should().NotBeEmpty();
    }

    [Theory]
    [InlineData("")]
    [InlineData("a")]
    [InlineData("Detta är ett alldeles för långt förnamn som borde trigga ett valideringsfel i domänen...")]
    public void FirstName_Create_Should_ReturnFailure_WhenValueIsInvalid(string invalidName)
    {
        // --- Act ---
        var result = FirstName.Create(invalidName);

        // --- Assert ---
        result.IsFailure.Should().BeTrue();
        result.Error.Should().Be(DomainErrors.Validation.InvalidFormat);
    }

    [Fact]
    public void Email_Create_Should_ReturnFailure_WhenEmailMissingAtSymbol()
    {
        // --- Arrange ---
        var invalidEmail = "inte-en-epost";

        // --- Act ---
        var result = Email.Create(invalidEmail);

        // --- Assert ---
        result.IsFailure.Should().BeTrue();
        result.Error.Should().Be(DomainErrors.Validation.InvalidFormat);
    }

    private static Result<ContactRequestEntity> CreateValidContactRequest()
    {
        return ContactRequestEntity.Create(
            FirstName.Create("Kalle").Value,
            LastName.Create("Anka").Value,
            Email.Create("kalle@ankeborg.se").Value,
            PhoneNumber.Create("0701234567").Value,
            MessageBody.Create("Hej hej").Value);
    }
}
