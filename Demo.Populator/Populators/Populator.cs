using Demo.DomainServices.Interface.Orchestration;
using Demo.Populator.Interfaces;
using Microsoft.Extensions.Logging;

namespace Demo.Populator.Populators;

public abstract class Populator : IPopulator
{
    protected ILogger Logger { get; private set; }
    protected IMediator Mediator { get; private set; }

    protected Populator(ILogger logger, IMediator mediator)
    {
        Logger = logger;
        Mediator = mediator;
    }

    public abstract Task Populate();
}
