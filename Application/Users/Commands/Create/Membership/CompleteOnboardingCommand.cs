using Domain.Common;
using MediatR;
using System;
using System.Collections.Generic;
using System.Text;

namespace Application.Users.Commands.Create.Membership;

public sealed record CompleteOnboardingCommand(
    Guid UserId,
    string FirstName,
    string LastName,
    string? Phone,
    string? PlanType
): IRequest<Result>;