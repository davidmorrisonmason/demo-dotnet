using Demo.DomainServices.Command.Validation;
using Demo.DomainServices.Creation;
using Demo.DomainServices.Interface.Command.Client;
using Demo.DomainServices.Interface.Repository;
using Demo.DomainServices.Interface.Transaction;
using FluentValidation;
using Microsoft.Extensions.Logging;

using Demo.DomainServices.Context;
using Demo.DomainServices.Interface.Context;

namespace Demo.DomainServices.Command.Client;

public class ClientCreateCommandHandler : ResultCommandHandler<ClientCreateCommand, ClientCreateCommandValidator, Model.Domain.Client>
{
    private readonly IClientRepository _clientRepository;
    private readonly IAggregateRootFactory _aggregateRootFactory;

    public ClientCreateCommandHandler(
        ILogger<ClientCreateCommandHandler> logger,
        ClientCreateCommandValidator validator,
        IClientRepository clientRepository,
        IAggregateRootFactory aggregateRootFactory,
        IUnitOfWork unitOfWork,
        IRequestContext requestContext) : base(logger, validator, unitOfWork, requestContext)
    {
        _clientRepository = clientRepository;
        _aggregateRootFactory = aggregateRootFactory;
    }

    protected override async Task<Model.Domain.Client> Execute(ClientCreateCommand request, CancellationToken cancellationToken)
    {
        var client = _aggregateRootFactory.NewClient(request.Name, request.ApiKey);
        await _clientRepository.Add(client);
        return client;
    }
}

public class ClientCreateCommandValidator : CommandValidator<ClientCreateCommand>
{
    private readonly IClientRepository _clientRepository;

    public ClientCreateCommandValidator(IClientRepository clientRepository)
    {
        _clientRepository = clientRepository;

        RuleFor(x => x.Name)
            .NotEmpty().WithError(ClientCommandErrorType.Client_Name_Required)
            .MustAsync(async (name, _) => !(await _clientRepository.GetAllByName(name)).Any())
                .WithError(ClientCommandErrorType.Client_Name_Must_Be_Unique);

        RuleFor(x => x.ApiKey)
            .NotEmpty().WithError(ClientCommandErrorType.Client_ApiKey_Required)
            .MustAsync(async (apiKey, _) => !(await _clientRepository.GetAllByApiKey(apiKey)).Any())
                .WithError(ClientCommandErrorType.Client_ApiKey_Must_Be_Unique);
    }
}
