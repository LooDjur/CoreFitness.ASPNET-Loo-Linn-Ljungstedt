using Application.CustomerSupport.Commands;
using Application.CustomerSupport.Handler;
using Domain.Common;
using Domain.ContactReq.Entities;
using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using Tests.Common;

namespace Tests.CustomerSupport;

public class RegisterContactHandlerTests : BaseIntegrationTest
{
    private readonly RegisterContactCommandHandler _handler;

    public RegisterContactHandlerTests()
    {
        _handler = new RegisterContactCommandHandler(UnitOfWork.ContactRequests, UnitOfWork);
    }

    [Fact]
    public async Task Handle_Should_RegisterContactRequest_WhenDataIsValid()
    {
        // --- 1. Arrange ---
        var emailStr = "kalle@ankeborg.se";
        var command = new RegisterContactCommand(
            "Kalle",
            "Anka",
            emailStr,
            "0701234567",
            "Jag vill ha hjälp med min träning.");

        // --- 2. Act ---
        var result = await _handler.Handle(command, CancellationToken.None);

        // --- 3. Assert ---
        result.IsSuccess.Should().BeTrue();

        Context.ChangeTracker.Clear();

        var allRequests = await Context.Set<ContactRequestEntity>().ToListAsync();
        var registeredRequest = allRequests.FirstOrDefault(x => x.Email.Value == emailStr);

        registeredRequest.Should().NotBeNull();
        registeredRequest!.FirstName.Value.Should().Be("Kalle");
    }

    [Fact]
    public async Task Handle_Should_ReturnFailure_WhenEmailIsInvalid()
    {
        // --- 1. Arrange ---
        var command = new RegisterContactCommand(
            "Kalle",
            "Anka",
            "inte-en-epost",
            "0701234567",
            "Hej!");

        // --- 2. Act ---
        var result = await _handler.Handle(command, CancellationToken.None);

        // --- 3. Assert ---
        result.IsFailure.Should().BeTrue();
        var count = await Context.Set<ContactRequestEntity>().CountAsync();
        count.Should().Be(0);
    }
}
