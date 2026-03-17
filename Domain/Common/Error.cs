using System;
using System.Collections.Generic;
using System.Text;

namespace Domain.Common;

public record Error(string Code, string Description)
{
    public static readonly Error None = new(string.Empty, string.Empty);
}

public static class DomainErrors
{
    public static class Validation
    {
        public static readonly Error Required = new("Validation.Required", "This field is required.");
        public static readonly Error TooLong = new("Validation.TooLong", "The input is too long.");
        public static readonly Error InvalidFormat = new("Validation.Invalid", "The input format is incorrect.");
    }

    public static class Session
    {
        public static readonly Error NotFound = new("Session.NotFound", "The session was not found.");

        public static readonly Error InvalidDate = new("Session.InvalidDate", "Date is either in the past or invalid.");
        public static readonly Error ActionNotAllowed = new("Session.NotAllowed", "This action cannot be performed on the current session state.");

        public static readonly Error InvalidCapacity = new("Session.InvalidCapacity", "Capacity must be between 10 and 40.");
    }

    public static class Membership
    {
        public static readonly Error NotFound = new("Membership.NotFound", "The member was not found.");

        public static readonly Error Ineligible = new("Membership.Ineligible", "Membership status does not allow this action.");
        public static readonly Error LimitReached = new("Membership.Limit", "The requested change exceeds the allowed limit.");
    }
}
