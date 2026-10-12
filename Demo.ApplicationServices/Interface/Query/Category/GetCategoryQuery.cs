namespace Demo.ApplicationServices.Interface.Query.Category;

public record GetCategoryQuery(int Id) : IQuery<Model.Domain.Category?>;
