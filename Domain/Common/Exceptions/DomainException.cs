using System;
using System.Collections.Generic;
using System.Text;

namespace Domain.Common.Exceptions;

public class DomainException(Error error) : Exception(error.Description)
{
    public Error Error { get; } = error;
}
