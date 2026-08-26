using FluentValidation;

namespace Demo.DomainServices.Command.Validation;

public abstract class CommandValidator<TCommand> : AbstractValidator<TCommand>
{
}
