using Domain.Common.Exceptions;

namespace Domain.Common.ValueObjects.Abstractions;

public abstract record DateValueObject
{
    public DateTime Value { get; }

    protected DateValueObject(DateTime value, DateTime min, DateTime max, Error error)
    {
        if (value < min || value > max)
        {
            throw new DomainException(error);
        }

        Value = value;
    }

    public static implicit operator DateTime(DateValueObject dateObject) => dateObject.Value;
}