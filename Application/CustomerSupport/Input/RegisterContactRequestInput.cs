using System;
using System.Collections.Generic;
using System.Text;

namespace Application.CustomerSupport.Input;

public sealed record RegisterContactRequestInput
(
    string FirstName,
    string LastName,
    string Email,
    string? Phone,
    string Message
);