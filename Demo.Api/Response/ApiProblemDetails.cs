namespace Demo.Api.Response;

public class ApiProblemDetails
{
    public string Type { get; set; }
    public string Title { get; set; }
    public string Detail { get; set; }
    public int Status { get; set; }
    public List<ErrorMessageDto> Errors { get; set; }
}

public record ErrorMessageDto(string ErrorCode, string ErrorDescription);
