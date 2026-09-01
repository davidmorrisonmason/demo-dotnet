using Demo.Api.Configuration;
using Demo.Api.Logging;
using Demo.DomainServices.Command.Category;
using Demo.DomainServices.Configuration;
using Demo.DomainServices.Creation;
using Demo.DomainServices.Interface.Repository;
using Demo.DomainServices.Interface.Time;
using Demo.DomainServices.Time;
using Demo.Infrastructure.Data;
using Demo.Infrastructure.Query.Category;
using Demo.Infrastructure.Repository;
using Microsoft.EntityFrameworkCore;
using System.Reflection;

var builder = WebApplication.CreateBuilder(args);
var configuration = builder.Configuration;

builder.Services.Configure<BasketSettings>(configuration.GetSection(nameof(BasketSettings)));

LoggingUtilities.ConfigureLogging(builder.Services, configuration);

// Add global services to the container.
ApiConfigurator.ConfigureServices(builder.Services, typeof(GetCategoriesQueryHandler).Assembly, typeof(CategoryCreateCommandHandler).Assembly, Assembly.GetExecutingAssembly());

builder.Services.AddDbContext<ApplicationDbContext>(options =>
{
    options.UseSqlite(configuration.GetConnectionString("DefaultConnection"));
});

builder.Services.AddScoped<ICategoryRepository, CategoryRepository>();
builder.Services.AddScoped<IBasketRepository, BasketRepository>();
builder.Services.AddSingleton<IAggregateRootFactory, AggregateRootFactory>();
builder.Services.AddSingleton<ITimeService, TimeService>();

var app = builder.Build();

ApiConfigurator.ConfigureApplication(app, "api");

app.Run();

// this is needed to enable integration tests to set up the test environment
public partial class Program
{ }
