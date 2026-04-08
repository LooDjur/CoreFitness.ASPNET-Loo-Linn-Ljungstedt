using System;
using System.Collections.Generic;
using System.Text;

namespace Application.Users.Output;

public sealed record UserResponse(
    Guid UserId,
    Guid? MemberId,
    string Email,
    string? FirstName,
    string? LastName,
    string? Phone,
    string? ProfileImageUrl,
    string? MembershipPlan,
    DateTime? ExpiryDate,
    bool IsActive
);