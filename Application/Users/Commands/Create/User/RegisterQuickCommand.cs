using Domain.Common;
using MediatR;
using System;
using System.Collections.Generic;
using System.Text;

namespace Application.Users.Commands.Create.User;

public sealed record RegisterQuickCommand(string Email) : IRequest<Result<Guid>>;