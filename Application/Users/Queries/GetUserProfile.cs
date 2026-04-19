using Application.Users.Output;
using Domain.Common;
using MediatR;

namespace Application.Users.Queries;

public sealed record GetUserProfileQuery(Guid UserId, DateTime UtcNow) : IRequest<Result<UserResponse>>;
