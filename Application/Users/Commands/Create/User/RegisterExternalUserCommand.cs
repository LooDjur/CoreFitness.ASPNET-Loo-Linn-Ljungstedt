using Domain.Common;
using MediatR;
using System;
using System.Collections.Generic;
using System.Text;

namespace Application.Users.Commands.Create.User;

public sealed record RegisterExternalUserCommand(
    string Email,
    string Provider,
    string ProviderKey
) : IRequest<Result<Guid>>;
