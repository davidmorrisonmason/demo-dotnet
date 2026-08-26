using Demo.Api.Dto;
using Demo.DomainServices.Interface.Command.Category;
using Demo.DomainServices.Interface.Orchestration;
using Demo.DomainServices.Interface.Query.Category;
using Demo.Model.Domain;
using Microsoft.AspNetCore.Mvc;

namespace Demo.Api.Controllers;

[Route("api/[controller]")]
[ApiController]
public class CategoriesController : ApiController
{
    public CategoriesController(
        ILogger<CategoriesController> logger,
        IMediator mediator) : base(logger, mediator)
    {
    }

    // GET: api/Categories
    [HttpGet]
    public async Task<ActionResult<IEnumerable<CategoryDto>>> GetCategories()
    {
        return await ExecuteQuery<GetCategoriesQuery, Category, CategoryDto>(new GetCategoriesQuery());
    }

    // GET: api/Categories/5
    [HttpGet("{id}")]
    public async Task<ActionResult<CategoryDto>> Get(int id)
    {
        return await ExecuteQuerySingle<GetCategoryQuery, Category, CategoryDto>(new GetCategoryQuery(id));
    }

    // PUT: api/Categories/5
    [HttpPut("{id}")]
    public async Task<IActionResult> Put(int id, CategoryUpdateDto category)
    {
        var command = new CategoryUpdateCommand(id, category.Name);
        return await ExecutePutCommand<CategoryUpdateCommand, Category>(command);
    }


    // POST: api/Categories
    [HttpPost]
    public async Task<ActionResult<EntityDto>> Post(CategoryCreateDto category)
    {
        var command = new CategoryCreateCommand(category.Name);
        return await ExecutePostCommand<CategoryCreateCommand, Category>(nameof(Get), command);
    }

    // DELETE: api/Categories/5
    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(int id)
    {
        var command = new CategoryDeleteCommand(id);
        return await ExecuteDeleteCommand<CategoryDeleteCommand, Category>(command);
    }
}
