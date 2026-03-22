using Domain.Common;
using MediatR;
using System;
using System.Collections.Generic;
using System.Text;

namespace Application.CustomerSupport.Commands;

public sealed record RegisterContactCommand(
    string FirstName,
    string LastName,
    string Email,
    string? Phone,
    string Message
) : IRequest<Result>;
