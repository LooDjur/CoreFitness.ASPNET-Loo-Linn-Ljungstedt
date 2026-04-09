using Domain.Common;
using MediatR;

namespace Application.Users.Commands.Delete.Membership;

public record CancelMembershipCommand(Guid UserId) : IRequest<Result>;