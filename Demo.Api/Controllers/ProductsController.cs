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
        return await ExecutePostCommand<CategoryAddProductCommand, Product>("Get", command);
    }

    // DELETE: api/Categories/5
    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(int id)
    {
        var command = new CategoryDeleteCommand(id);
        return await ExecuteDeleteCommand<CategoryDeleteCommand, Category>(command);
    }
}
