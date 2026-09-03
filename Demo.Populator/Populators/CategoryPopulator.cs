using Demo.DomainServices.Interface.Command.Category;
using Demo.DomainServices.Interface.Context;
using Demo.DomainServices.Interface.Orchestration;
using Demo.Model.Domain;
using Demo.Populator.Interfaces;
using Microsoft.Extensions.Logging;

namespace Demo.Populator.Populators;

public class CategoryPopulator : Populator, ICategoryPopulator
{
    private Dictionary<string, Client> _clientsByPlainTextApiKey = [];
    private readonly IRequestContext _requestContext;

    public CategoryPopulator(ILogger<CategoryPopulator> logger, IMediator mediator, IRequestContext requestContext) : base(logger, mediator)
    {
        _requestContext = requestContext;
    }

    public void SetClients(Dictionary<string, Client> clientsByPlainTextApiKey)
    {
        _clientsByPlainTextApiKey = clientsByPlainTextApiKey;
    }

    public override async Task Populate()
    {
        int clientNumber = 1;

        foreach (var clientKeyValuePair in _clientsByPlainTextApiKey)
        {
            _requestContext.SetClient(clientKeyValuePair.Value);

            int productNumber;

            for (int i = 1; i <= 3; i++)
            {
                productNumber = 1;
                var category = await Mediator.Send(new CategoryCreateCommand($"Client {clientNumber} Category {i}"));
                await Mediator.Send(new CategoryAddProductCommand(category.Id, $"Client {clientNumber} Category {i} Product {productNumber++}", productNumber * 1.3m));
                await Mediator.Send(new CategoryAddProductCommand(category.Id, $"Client {clientNumber} Category {i} Product {productNumber++}", productNumber * 0.7m));
                await Mediator.Send(new CategoryAddProductCommand(category.Id, $"Client {clientNumber} Category {i} Product {productNumber++}", productNumber * 6.3m));

                for (int s = 1; s <= 2; s++)
                {
                    productNumber = 1;
                    var subCategory = await Mediator.Send(new CategoryAddSubCategoryCommand(category.Id, $"Client {clientNumber} Category {i} SubCategory {s}"));
                    await Mediator.Send(new CategoryAddProductCommand(subCategory.Id, $"Client {clientNumber} Category {i} SubCategory {s} Product {productNumber++}", productNumber * 1.3m));
                    await Mediator.Send(new CategoryAddProductCommand(subCategory.Id, $"Client {clientNumber} Category {i} SubCategory {s} Product {productNumber++}", productNumber * 0.7m));
                    await Mediator.Send(new CategoryAddProductCommand(subCategory.Id, $"Client {clientNumber} Category {i} SubCategory {s} Product {productNumber++}", productNumber * 6.3m));
                }
            }

            clientNumber++;
        }
    }
}