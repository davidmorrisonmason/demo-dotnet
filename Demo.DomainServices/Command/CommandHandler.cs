using Demo.DomainServices.Interface.Command;
using Demo.DomainServices.Interface.Orchestration;
using Demo.DomainServices.Interface.Transaction;
using Demo.Model.Domain.Validation;
using FluentValidation;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json.Linq;

using Demo.DomainServices.Context;
using Demo.DomainServices.Interface.Context;

namespace Demo.DomainServices.Command;

public abstract class CommandHandler<TCommand, TCommandValidator> : BaseCommandHandler, IRequestHandler<TCommand>
    where TCommand : ICommand
    where TCommandValidator : IValidator<TCommand>
{
    private readonly TCommandValidator _commandValidator;

    public Guid Id { get; set; }

    public CommandHandler(
        ILogger logger,
        TCommandValidator commandValidator,
        IUnitOfWork unitOfWork,
        IRequestContext requestContext) : base(logger, unitOfWork, requestContext)
    {
        _commandValidator = commandValidator;
    }

    public async Task Handle(TCommand command, CancellationToken cancellationToken)
    {
        Logger.LogTrace("Executing command: {Name}", typeof(TCommand).Name);
        var logPayload = JObject.FromObject(ToLogObject(command));
        LogCommandPayload(logPayload);

        await UnitOfWork.Execute(async () =>
        {
            await CommandPrep(command);
            await Execute(command, cancellationToken);
        }, Transactional);

        Logger.LogTrace("Command execution {Name} completed successfully", typeof(TCommand).Name);
    }

    /// <summary>
    /// Logs the command payload
    /// </summary>
    /// <param name="commandPayload"></param>
    protected void LogCommandPayload(JObject commandPayload)
    {
        if (commandPayload == null)
        {
            Logger.LogTrace("No command payload supplied for logging");
        }
        else
        {
            Logger.LogTrace("{Message}", commandPayload.ToString());
        }
    }

    /// <summary>
    /// Performs all tasks necessary before a command is executed, such as validation and any command specific authorisation
    /// </summary>
    /// <param name="command"></param>
    /// <returns></returns>
    protected async Task CommandPrep(TCommand command)
    {
        await Validate(command);
        await Authorise(command);
    }

    protected virtual async Task Validate(TCommand command)
    {
        var validationResults = await _commandValidator.ValidateAsync(command);

        if (validationResults.Errors.Any())
        {
            var errors = validationResults.Errors
                .Select(x => new ErrorMessage(x.ErrorCode, x.ErrorMessage))
                .ToList();

            throw new Model.Domain.Validation.ValidationException(errors);
        }
    }

    /// <summary>
    /// Hook for overriding the logging behaviour of the command payload. Default is to log entire command, but
    /// individual command handlers can reduce this or remove PII etc by overriding this method
    /// </summary>
    protected virtual dynamic ToLogObject(TCommand command)
    {
        return command;
    }

    /// <summary>
    /// Specific command execution logic to implement in each command handler
    /// </summary>
    protected abstract Task Execute(TCommand command, CancellationToken cancellationToken);

    /// <summary>
    /// Hook for specific authorisation in a command subclass
    /// </summary>
    protected virtual Task Authorise(TCommand command)
    {
        return Task.CompletedTask;
    }
}
