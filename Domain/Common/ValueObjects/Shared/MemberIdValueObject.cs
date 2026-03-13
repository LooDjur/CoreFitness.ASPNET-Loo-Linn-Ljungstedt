using Domain.Common.ValueObjects.Abstractions;
using System;
using System.Collections.Generic;
using System.Text;

namespace Domain.Common.ValueObjects.Shared;

public record MemberId(Guid Value) : GuidValueObject(Value, DomainErrors.Validation.Required);