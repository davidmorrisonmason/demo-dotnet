namespace Demo.DomainServices.DependencyInjection;


public class MediatorException : Exception
{
    public MediatorException(string errorMessage) : base(errorMessage)
    {
    }
}
