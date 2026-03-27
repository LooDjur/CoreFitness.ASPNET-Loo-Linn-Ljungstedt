using Application.Users.Output;
using Domain.Common;
using MediatR;
using System;
using System.Collections.Generic;
using System.Text;

namespace Application.Users.Queries;

public sealed record GetUserProfileQuery(Guid UserId) : IRequest<Result<UserResponse>>;
