namespace Demo.DomainServices.Interface.Command;

public abstract record Command() : ICommand
{
}

public abstract record Command<TResult>() : ICommand<TResult>
{
}


