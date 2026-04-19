using Domain.Common;

namespace Application.Commo;

public sealed record AuthResult(bool Succeeded, Error? Error = null)
{
    public static AuthResult Ok() => new(true);
    public static AuthResult Failed(Error error) => new(false, error);
}
