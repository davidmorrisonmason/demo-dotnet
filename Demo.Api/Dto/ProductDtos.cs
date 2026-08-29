namespace Demo.Api.Dto;

public class ProductCreateDto
{
    public string Name { get; set; } = "";
    public decimal Price { get; set; } = 0;
}

public class ProductUpdateDto
{
    public string Name { get; set; } = "";
    public decimal Price { get; set; } = 0;
}

public class ProductDto : EntityDto
{
    public int Id { get; set; } = 0;
    public string Name { get; set; } = "";
    public decimal Price { get; set; } = 0;
}
