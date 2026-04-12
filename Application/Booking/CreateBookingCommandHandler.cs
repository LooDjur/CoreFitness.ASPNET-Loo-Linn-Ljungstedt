using Domain.Bookings.Entity;
using Domain.Bookings.Repositories;
using Domain.Common;
using Domain.Common.Abstractions;
using Domain.Common.ValueObjects.Shared;
using Domain.Sessions;
using Domain.Users.Repositories;
using MediatR;

namespace Application.Booking;

public sealed class CreateBookingCommandHandler(IUnitOfWork unitOfWork)
    : IRequestHandler<CreateBookingCommand, Result<Guid>>
{
    public async Task<Result<Guid>> Handle(CreateBookingCommand request, CancellationToken ct)
    {
        var sessionIdResult = SessionId.Create(request.SessionId);
        var userIdResult = UserId.Create(request.UserId);

        var validationResult = Result.FirstFailureOrSuccess(sessionIdResult, userIdResult);
        if (validationResult.IsFailure)
            return Result.Failure<Guid>(validationResult.Error);

        var sessionId = sessionIdResult.Value;
        var userId = userIdResult.Value;

        var session = await unitOfWork.Sessions.GetByIdAsync(sessionId, ct);
        if (session is null)
            return Result.Failure<Guid>(DomainErrors.Session.NotFound);

        var user = await unitOfWork.Users.GetByIdAsync(userId, ct);
        if (user is null)
            return Result.Failure<Guid>(DomainErrors.User.NotFound);

        if (user.Membership is null)
            return Result.Failure<Guid>(DomainErrors.User.Ineligible);

        var memberId = user.Membership.Id;

        var alreadyBooked = await unitOfWork.Bookings.IsAlreadyBookedAsync(sessionId, memberId, ct);
        if (alreadyBooked)
            return Result.Failure<Guid>(DomainErrors.Session.ActionNotAllowed);

        var currentBookingsCount = await unitOfWork.Bookings.CountForSessionAsync(sessionId.Value, ct);

        var sessionResult = session.Book(currentBookingsCount, request.UtcNow);
        if (sessionResult.IsFailure)
            return Result.Failure<Guid>(sessionResult.Error);

        var bookingResult = BookingEntity.Create(sessionId, memberId, request.UtcNow);
        if (bookingResult.IsFailure)
            return Result.Failure<Guid>(bookingResult.Error);

        await unitOfWork.Bookings.AddAsync(bookingResult.Value, ct);

        await unitOfWork.SaveChangesAsync(ct);

        return Result.Success(bookingResult.Value.Id.Value);
    }
}