using Demo.Api.Dto;
using Demo.DomainServices.Interface.Command.Category;
using Demo.DomainServices.Interface.Orchestration;
using Demo.Model.Domain;
using Microsoft.AspNetCore.Mvc;

namespace Demo.Api.Controllers;

[Route("api/Categories/{categoryId}/[controller]")]
[ApiController]
public class ProductsController : ApiController
{
    public ProductsController(
        ILogger<ProductsController> logger,
        IMediator mediator) : base(logger, mediator)
    {
    }

    // POST: api/Categories/5/Products
    [HttpPost]
    public async Task<ActionResult<EntityDto>> Post(
        int categoryId,
        ProductCreateDto product)
    {
        var command = new CategoryAddProductCommand(categoryId, product.Name, product.Price);
        return await ExecutePostCommand<CategoryAddProductCommand, Product>("Post", command);
    }

    // PUT: api/Categories/5/Products/6
    [HttpPut("{productId}")]
    public async Task<IActionResult> Put(
        int categoryId,
        int productId,
        ProductUpdateDto product)
    {
        var command = new CategoryUpdateProductCommand(categoryId, productId, product.Name, product.Price);
        return await ExecutePutCommand<CategoryUpdateProductCommand, Product>(command);
    }
}
