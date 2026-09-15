namespace Demo.Api.Dto;

public class CategoryCreateDto
{
    public required string Name { get; set; }
}

public class CategoryUpdateDto
{
    public required string Name { get; set; }
}

public class CategoryDto : EntityDto
{
    public required string Name { get; set; }
    public required List<ProductDto> Products { get; set; } = [];
    public required List<SubCategoryDto> SubCategories { get; set; } = [];
}

public class SubCategoryDto : EntityDto
{
    public required string Name { get; set; }
}