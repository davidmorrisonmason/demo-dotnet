namespace Demo.DomainServices.Interface.Query.Category;

public record GetCategoriesQuery : IQuery<IEnumerable<Model.Domain.Category>>;

