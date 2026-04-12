using Application.Sessions.Output;
using Domain.Common;
using MediatR;

namespace Application.Sessions.Queries;

public record GetSessionsQuery(Guid? UserId = null) : IRequest<Result<List<SessionResponse>>>;
public record GetSessionByIdQuery(Guid Id, Guid? UserId = null) : IRequest<Result<SessionResponse>>;