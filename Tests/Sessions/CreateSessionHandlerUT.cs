using Application.Sessions.Commands;
using Application.Sessions.Handlers;
using Domain.Common;
using Domain.Common.Abstractions;
using Domain.Common.Exceptions;
using Domain.Sessions.Entities;
using Domain.Sessions.Enums;
using Domain.Sessions.Repositories;
using NSubstitute;

namespace Tests.Sessions;

public class CreateSessionHandlerUT
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly ISessionRepository _sessionRepository;
    private readonly CreateSessionHandler _handler;

    public CreateSessionHandlerUT()
    {
        _unitOfWork = Substitute.For<IUnitOfWork>();
        _sessionRepository = Substitute.For<ISessionRepository>();

        // Setup: Koppla ihop repo med unit of work
        _unitOfWork.Sessions.Returns(_sessionRepository);

        _handler = new CreateSessionHandler(_unitOfWork);
    }

    [Fact]
    public async Task Handle_Should_ReturnFailure_When_CapacityIsInvalid()
    {
        // Arrange
        var tomorrow = DateTime.UtcNow.AddDays(1);
        var command = new CreateSessionCommand(
            "Yoga",
            "Ett lugnt Yoga pass",
            "Stina",
            SessionCategory.Yoga,
            tomorrow,
            tomorrow.AddHours(1),
            -5); // Felaktig kapacitet (< 0)

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        Assert.True(result.IsFailure);
        Assert.Equal(DomainErrors.Session.InvalidCapacity, result.Error);

        // Verifiera att vi aldrig anropade AddAsync eller SaveChanges
        await _sessionRepository.DidNotReceive().AddAsync(Arg.Any<SessionEntity>(), Arg.Any<CancellationToken>());
        await _unitOfWork.DidNotReceive().SaveChangesAsync(Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task Handle_Should_ReturnFailure_When_TimeSlotIsInPast()
    {
        // Arrange
        var past = DateTime.UtcNow.AddDays(-1);
        var command = new CreateSessionCommand(
            "Yoga",
            "Beskrivning",
            "Stina",
            SessionCategory.Yoga,
            past,
            past.AddHours(1),
            20);

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        Assert.True(result.IsFailure);
        Assert.Equal(DomainErrors.Session.InvalidDate, result.Error);
    }

    [Fact]
    public async Task Handle_Should_ReturnSuccess_When_CommandIsValid()
    {
        // Arrange
        // Vi använder en fast tid för att undvika "flickering" i tester
        var futureStart = DateTime.UtcNow.AddDays(1);
        var futureEnd = futureStart.AddHours(1);

        var command = new CreateSessionCommand(
            "Spinning",
            "Stärkande pass",
            "Olof",
            SessionCategory.Spinning,
            futureStart,
            futureEnd,
            20);

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        Assert.True(result.IsSuccess);
        Assert.NotEqual(Guid.Empty, result.Value); // Vi förväntar oss ett giltigt ID tillbaka

        // Verifiera flödet mot databaslagret
        await _sessionRepository.Received(1).AddAsync(Arg.Any<SessionEntity>(), Arg.Any<CancellationToken>());
        await _unitOfWork.Received(1).SaveChangesAsync(Arg.Any<CancellationToken>());
    }
}