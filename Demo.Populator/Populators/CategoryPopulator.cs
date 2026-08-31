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
        int productNumber = 1;

        for (int i = 1; i <= 3; i++)
        {
            var category = await Mediator.Send(new CategoryCreateCommand($"Category {i}"));
            await Mediator.Send(new CategoryAddProductCommand(category.Id, $"Product {productNumber++}", productNumber * 1.3m));
            await Mediator.Send(new CategoryAddProductCommand(category.Id, $"Product {productNumber++}", productNumber * 0.7m));
            await Mediator.Send(new CategoryAddProductCommand(category.Id, $"Product {productNumber++}", productNumber * 6.3m));

            for (int s = 1; s <= 2; s++)
            {
                var subCategory = await Mediator.Send(new CategoryAddSubCategoryCommand(category.Id, $"Category {i} SubCategory {s}"));
                await Mediator.Send(new CategoryAddProductCommand(subCategory.Id, $"Product {productNumber++}", productNumber * 1.3m));
                await Mediator.Send(new CategoryAddProductCommand(subCategory.Id, $"Product {productNumber++}", productNumber * 0.7m));
                await Mediator.Send(new CategoryAddProductCommand(subCategory.Id, $"Product {productNumber++}", productNumber * 6.3m));
            }
        }
    }
}