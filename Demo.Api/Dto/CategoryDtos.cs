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
    public string Name { get; set; }
    public List<ProductDto> Products { get; set; } = [];
}