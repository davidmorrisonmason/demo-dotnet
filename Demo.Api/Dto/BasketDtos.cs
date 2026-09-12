namespace Demo.Api.Dto;

public class BasketCreateDto
{
    public List<BasketItemCreateDto> BasketItems { get; set; } = [];
}

public class CheckoutCompleteDto
{
    public string Recipient { get; set; } = "";
    public string AddressLine1 { get; set; } = "";
    public string? AddressLine2 { get; set; }
    public string? AddressLine3 { get; set; }
    public string? AddressLine4 { get; set; }
    public string City { get; set; } = "";
    public string PostCode { get; set; } = "";
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
    public DateTime BasketExpirationTime { get; set; } = DateTime.UtcNow;
    public List<BasketItemDto> BasketItems { get; set; } = [];
    public decimal TotalPrice { get; set; } = 0;
}

public class BasketItemDto : EntityDto
{
    public int ProductId { get; set; }
    public ProductDto? Product { get; set; }
    public int Quantity { get; set; }
}
