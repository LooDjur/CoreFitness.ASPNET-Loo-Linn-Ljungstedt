using Domain.Common;
using MediatR;

namespace Application.Users.Commands.Create.User;

public sealed record RegisterQuickCommand(string Email, string Password, string Role) : IRequest<Result<Guid>>;