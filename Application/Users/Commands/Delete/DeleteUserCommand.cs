using Domain.Common;
using MediatR;

namespace Application.Users.Commands.Delete;

public sealed record DeleteUserCommand(Guid UserId) : IRequest<Result>;
