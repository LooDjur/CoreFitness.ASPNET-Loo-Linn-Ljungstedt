using Application.Sessions.DTOs;
using Domain.Common;
using MediatR;
using System;
using System.Collections.Generic;
using System.Text;

namespace Application.Sessions.Queries;

public record GetSessionsQuery() : IRequest<Result<List<SessionDto>>>;