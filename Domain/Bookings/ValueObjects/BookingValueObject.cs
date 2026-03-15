using Domain.Common;
using Domain.Common.ValueObjects.Abstractions;
using System;
using System.Collections.Generic;
using System.Text;

namespace Domain.Bookings.ValueObjects;

public record class SessionId(Guid Value) : GuidValueObject(Value, DomainErrors.Validation.Required);