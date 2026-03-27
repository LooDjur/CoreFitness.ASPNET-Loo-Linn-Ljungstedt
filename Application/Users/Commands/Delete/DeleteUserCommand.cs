using Domain.Common;
using MediatR;
using System;
using System.Collections.Generic;
using System.Text;

namespace Application.Users.Commands.Delete;

public sealed record DeleteUserCommand(Guid UserId) : IRequest<Result>;
