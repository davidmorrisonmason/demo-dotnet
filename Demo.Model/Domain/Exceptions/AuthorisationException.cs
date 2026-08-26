using Demo.Model.Domain.Validation;

namespace Demo.Model.Domain.Exceptions;

public class AuthorisationException : ApplicationException
{
    public AuthorisationException(ErrorMessage errorMessage) : base(errorMessage)
    {
    }

    public AuthorisationException(IEnumerable<ErrorMessage> errorMessages) : base(errorMessages)
    {
    }
}