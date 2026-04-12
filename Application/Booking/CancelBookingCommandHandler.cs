using Domain.Bookings.Repositories;
using Domain.Common;
using Domain.Common.Abstractions;
using Domain.Common.ValueObjects.Shared;
using Domain.Users.Repositories;
using MediatR;

namespace Application.Booking;

public sealed class CancelBookingCommandHandler(
    IUnitOfWork unitOfWork,
    IBookingRepository bookingRepository,
    IUserRepository userRepository)
    : IRequestHandler<CancelBookingCommand, Result>
{
    public async Task<Result> Handle(CancelBookingCommand request, CancellationToken ct)
    {
        var sessionIdResult = SessionId.Create(request.SessionId);
        var userIdResult = UserId.Create(request.UserId);

        var validationResult = Result.FirstFailureOrSuccess(sessionIdResult, userIdResult);
        if (validationResult.IsFailure)
            return Result.Failure(validationResult.Error);

        var sessionId = sessionIdResult.Value;
        var userId = userIdResult.Value;

        var user = await userRepository.GetByIdAsync(userId, ct);
        if (user is null)
            return Result.Failure(DomainErrors.User.NotFound);

        if (user.Membership is null)
            return Result.Failure(DomainErrors.User.Ineligible);

        var booking = await bookingRepository.GetSpecificBookingAsync(
            sessionId,
            user.Membership.Id,
            ct);

        if (booking is null)
            return Result.Failure(DomainErrors.Booking.NotFound);

        bookingRepository.Delete(booking);
        await unitOfWork.SaveChangesAsync(ct);

        return Result.Success();
    }
}