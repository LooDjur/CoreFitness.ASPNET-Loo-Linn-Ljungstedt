using Domain.Common;
using MediatR;

namespace Application.Sessions.Commands.Delete;

public sealed record DeleteSessionCommand(Guid Id, DateTime UtcNow) : IRequest<Result>;
