namespace Demo.Api.Response;

public class ApiProblemDetails
{
    public required string Type { get; set; }
    public required string Title { get; set; }
    public required string Detail { get; set; }
    public required int Status { get; set; }
    public required List<ErrorMessageDto> Errors { get; set; }
}

public record ErrorMessageDto(string ErrorCode, string ErrorDescription);
