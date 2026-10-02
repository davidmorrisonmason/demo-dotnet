using Demo.Api.Configuration;
using Demo.DomainServices.Command.Category;
using Demo.DomainServices.Configuration;
using Demo.Infrastructure.Data;
using Demo.Infrastructure.Logging;
using Demo.Infrastructure.Query.Category;
using System.Reflection;

var builder = WebApplication.CreateBuilder(args);
var configuration = builder.Configuration;

builder.Services.Configure<BasketSettings>(configuration.GetSection(nameof(BasketSettings)));

LoggingUtilities.ConfigureLogging(builder.Services, configuration);

// Add global services to the container.
ApiConfigurator.ConfigureServices(builder.Services, typeof(GetCategoriesQueryHandler).Assembly, typeof(CategoryCreateCommandHandler).Assembly, Assembly.GetExecutingAssembly());

builder.Services.AddApplicationDatabase(configuration);


var app = builder.Build();

await ApiConfigurator.ConfigureApplication(app, "api");

app.Run();
