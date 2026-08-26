using ApplicationException = Demo.Model.Domain.Exceptions.ApplicationException;

namespace Demo.Model.Domain.Validation
{
    public class ValidationException : ApplicationException
    {
        public ValidationException(ErrorMessage errorMessage) : base(errorMessage)
        {
        }

        public ValidationException(IEnumerable<ErrorMessage> errorMessages) : base(errorMessages)
        {
        }
    }
}
