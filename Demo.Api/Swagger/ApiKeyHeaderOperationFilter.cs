using Microsoft.OpenApi;
using Swashbuckle.AspNetCore.SwaggerGen;
using System.Text.Json.Nodes;

namespace Demo.Api.Swagger;

public sealed class ApiKeyHeaderOperationFilter : IOperationFilter
{
    private const string ApiKeyHeaderName = "x-api-key";
    private const string DefaultApiKey = "test-api-key";

    public void Apply(OpenApiOperation operation, OperationFilterContext context)
    {
        operation.Parameters ??= new List<IOpenApiParameter>();

        if (operation.Parameters.Any(parameter =>
                parameter.In == ParameterLocation.Header &&
                string.Equals(parameter.Name, ApiKeyHeaderName, StringComparison.OrdinalIgnoreCase)))
        {
            return;
        }

        operation.Parameters.Add(new OpenApiParameter
        {
            Name = ApiKeyHeaderName,
            In = ParameterLocation.Header,
            Required = true,
            Description = "API key used to authorize the request.",
            Schema = new OpenApiSchema
            {
                Type = JsonSchemaType.String,
                Default = JsonValue.Create(DefaultApiKey)
            }
        });
    }
}
