using System;
using System.Collections.Generic;
using System.Text;

namespace Domain.Common;

public record Error(string Code, string Description, ErrorType Type)
{
    public static readonly Error None = new(string.Empty, string.Empty, ErrorType.Failure);
    public static Error NotFound(string code, string description) => new(code, description, ErrorType.NotFound);
    public static Error Validation(string code, string description) => new(code, description, ErrorType.Validation);
    public static Error Conflict(string code, string description) => new(code, description, ErrorType.Conflict);
    public static Error Failure(string code, string description) => new(code, description, ErrorType.Failure);
}

public static class FaqErrors
{
    public static readonly Error NotFound = Error.NotFound(
        "Faq.NotFound",
        "The FAQ items could not be found.");
}

public static class DomainErrors
{
    public static class Authentication
    {
        public static readonly Error InvalidCredentials = Error.Validation("Auth.InvalidCredentials", "Incorrect email address or password.");
        public static readonly Error ExternalLoginFailed = Error.Failure("Auth.ExternalLoginFailed", "Could not link external login provider.");
        public static readonly Error LockedOut = Error.Failure("Auth.LockedOut", "User account has been temporary locked.");
    }

    public static class Validation
    {
        public static readonly Error Required = Error.Validation("Validation.Required", "This field is required.");
        public static readonly Error TooLong = Error.Validation("Validation.TooLong", "The input is too long.");
        public static readonly Error InvalidFormat = Error.Validation("Validation.Invalid", "The input format is incorrect.");
    }

    public static class Session
    {
        public static readonly Error NotFound = Error.NotFound("Session.NotFound", "The session was not found.");
        public static readonly Error InvalidDate = Error.Validation("Session.InvalidDate", "Date is invalid.");
        public static readonly Error ActionNotAllowed = Error.Failure("Session.NotAllowed", "Action not allowed.");
        public static readonly Error InvalidCapacity = Error.Validation("Session.InvalidCapacity", "Capacity 10-20.");
    }

    public static class User
    {
        public static readonly Error NotFound = Error.NotFound("User.NotFound", "The user was not found.");
        public static readonly Error Ineligible = Error.Failure("User.Ineligible", "User status error.");
        public static readonly Error LimitReached = Error.Conflict("User.Limit", "Limit exceeded.");
        public static readonly Error EmailInvalid = Error.Validation("User.EmailInvalid", "Email is invalid.");
        public static readonly Error SaveError = Error.Failure("User.SaveError", "Kunde inte spara användaren i databasen.");
    }

    public static class Booking
    {
        public static readonly Error NotFound = Error.Failure("Booking.NotFound", "The booking was not found.");
    }
}
