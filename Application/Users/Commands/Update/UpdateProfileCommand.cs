using Domain.Common;
using MediatR;
using System;
using System.Collections.Generic;
using System.Text;

namespace Application.Users.Commands.Update;

public sealed record UpdateProfileCommand(
    Guid UserId,
    string FirstName,
    string LastName,
    string Email,
    string? Phone,
    string? ProfileImageUrl
) : IRequest<Result>;