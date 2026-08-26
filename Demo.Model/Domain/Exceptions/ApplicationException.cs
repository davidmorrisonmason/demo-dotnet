using Demo.Model.Domain.Validation;

namespace Demo.Model.Domain.Exceptions
{
    public class ApplicationException : Exception
    {
        public IEnumerable<ErrorMessage> ErrorMessages { get; private set; }

        public ApplicationException(ErrorMessage errorMessage) : this(new List<ErrorMessage> { errorMessage })
        {
        }

        public ApplicationException(IEnumerable<ErrorMessage> errorMessages)
        {
            ErrorMessages = errorMessages;
        }
    }
}
