using Domain.Common;
using MediatR;

namespace Application.Users.Commands.Create.Membership;

public sealed record CompleteOnboardingCommand(
    Guid UserId,
    string FirstName,
    string LastName,
    string? Phone,
    string? PlanType
) : IRequest<Result>;