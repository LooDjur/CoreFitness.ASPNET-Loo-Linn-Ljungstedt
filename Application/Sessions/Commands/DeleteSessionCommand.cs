using Domain.Common;
using MediatR;

namespace Application.Sessions.Commands;

public record DeleteSessionCommand(Guid Id) : IRequest<Result>;
