using FluentValidation;

namespace Demo.ApplicationServices.Command.Validation;

public abstract class CommandValidator<TCommand> : AbstractValidator<TCommand>
{
}
