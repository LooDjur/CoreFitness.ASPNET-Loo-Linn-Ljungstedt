using System;
using System.Collections.Generic;
using System.Text;

namespace Application.CustomerSupport.Output;

public sealed record ContactRequest
(
    string Id,
    string FirstName,
    string LastName,
    string Email,
    string? Phone,
    string Message,
    DateTime CreatedAt
);