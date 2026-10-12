namespace Demo.ApplicationServices.Interface.Query.Category;

public record GetCategoriesQuery : IQuery<IEnumerable<Model.Domain.Category>>;

