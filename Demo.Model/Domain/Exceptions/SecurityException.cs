using Demo.Model.Domain.Validation;

namespace Demo.Model.Domain.Exceptions;

public class SecurityException : ApplicationException
{
    public SecurityException(ErrorMessage errorMessage) : base(errorMessage)
    {
    }
}
