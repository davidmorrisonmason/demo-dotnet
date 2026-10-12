namespace Demo.ApplicationServices.Orchestration;


public class MediatorException : Exception
{
    public MediatorException(string errorMessage) : base(errorMessage)
    {
    }
}
