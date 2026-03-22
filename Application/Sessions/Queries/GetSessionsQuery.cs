using Application.Sessions.Output;
using Domain.Common;
using MediatR;
using System;
using System.Collections.Generic;
using System.Text;

namespace Application.Sessions.Queries;

public record GetSessionsQuery() : IRequest<Result<List<SessionOutput>>>;
public record GetSessionByIdQuery(Guid Id) : IRequest<Result<SessionOutput>>;