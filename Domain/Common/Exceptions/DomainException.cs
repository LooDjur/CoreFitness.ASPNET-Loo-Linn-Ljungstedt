using System;
using System.Collections.Generic;
using System.Text;

namespace Domain.Common.Exceptions;

public class DomainException(string message) : Exception(message);
