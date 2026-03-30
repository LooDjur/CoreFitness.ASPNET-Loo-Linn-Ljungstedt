using Domain.Common;
using MediatR;

namespace Application.Users.Commands.Update;

public sealed record UpdateProfileCommand(
    Guid UserId,
    string FirstName,
    string LastName,
    string Email,
    string? Phone,
    string? ProfileImageUrl
) : IRequest<Result>;