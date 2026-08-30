using Demo.Api.Dto;
using Demo.DomainServices.Interface.Command.Checkout;
using Demo.DomainServices.Interface.Orchestration;
using Demo.DomainServices.Interface.Query.Checkout;
using Demo.Model.Domain.Checkout;
using Microsoft.AspNetCore.Mvc;

namespace Demo.Api.Controllers;

[Route("api/[controller]")]
[ApiController]
public class BasketsController : ApiController
{
    public BasketsController(
        ILogger<BasketsController> logger,
        IMediator mediator) : base(logger, mediator)
    {
    }

    // GET: api/Baskets/5
    [HttpGet("{id}")]
    public async Task<ActionResult<BasketDto>> Get(int id)
    {
        return await ExecuteQuerySingle<GetBasketQuery, Basket, BasketDto>(new GetBasketQuery(id));
    }

    // PUT: api/Baskets/5
    [HttpPut("{id}")]
    public async Task<IActionResult> Put(int id, BasketCreateDto basket)
    {
        var command = new BasketAddItemsCommand(
            id,
            basket.BasketItems.Select(item => new BasketItemCommand(
                item.CategoryId,
                item.ProductId,
                item.Quantity)).ToList());

        return await ExecutePutCommand<BasketAddItemsCommand, Basket>(command);
    }

    // POST: api/Baskets
    [HttpPost]
    public async Task<ActionResult<BasketDto>> Post(BasketCreateDto basket)
    {
        var command = new BasketCreateCommand(
            basket.BasketItems.Select(item => new BasketItemCommand(
                item.CategoryId,
                item.ProductId,
                item.Quantity)).ToList());

        return await ExecutePostCommand<BasketCreateCommand, Basket>(nameof(Post), command);
    }
}
