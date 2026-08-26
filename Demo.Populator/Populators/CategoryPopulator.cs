using Demo.DomainServices.Interface.Command.Category;
using Demo.DomainServices.Interface.Orchestration;
using Demo.Populator.Interfaces;
using Microsoft.Extensions.Logging;

namespace Demo.Populator.Populators;

public class CategoryPopulator : Populator, ICategoryPopulator
{
    public CategoryPopulator(ILogger<CategoryPopulator> logger, IMediator mediator) : base(logger, mediator)
    {
    }

    public override async Task Populate()
    {
        await Mediator.Send(new CategoryCreateCommand("Top Priority"));
        await Mediator.Send(new CategoryCreateCommand("Important"));
        await Mediator.Send(new CategoryCreateCommand("Standard"));
    }
}
