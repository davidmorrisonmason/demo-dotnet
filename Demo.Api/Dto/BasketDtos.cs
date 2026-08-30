namespace Demo.Api.Dto;

public class BasketCreateDto
{
    public List<BasketItemCreateDto> BasketItems { get; set; } = [];
}

public class BasketItemCreateDto
{
    public int CategoryId { get; set; }
    public int ProductId { get; set; }
    public int Quantity { get; set; }
}

public class BasketDto : EntityDto
{
    public string BasketId { get; set; } = "";
    public List<BasketItemDto> BasketItems { get; set; } = [];
}

public class BasketItemDto : EntityDto
{
    public int ProductId { get; set; }
    public ProductDto? Product { get; set; }
    public int Quantity { get; set; }
}
