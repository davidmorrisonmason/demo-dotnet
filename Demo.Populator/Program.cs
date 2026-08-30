using Demo.Api.Logging;
using Demo.DomainServices.Creation;
using Demo.DomainServices.DependencyInjection;
using Demo.DomainServices.Interface.Command.Category;
using Demo.DomainServices.Interface.Query.Category;
using Demo.DomainServices.Interface.Repository;
using Demo.DomainServices.Interface.Transaction;
using Demo.Infrastructure.Data;
using Demo.Infrastructure.Query.Category;
using Demo.Infrastructure.Repository;
using Demo.Populator;
using Demo.Populator.Interfaces;
using Demo.Populator.Populators;
using FluentValidation;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

var builder = Host.CreateApplicationBuilder(args);
var configuration = builder.Configuration;

LoggingUtilities.ConfigureLogging(builder.Services, configuration);

builder.Services.AddDbContext<ApplicationDbContext>(options =>
{
    options.UseSqlite(configuration.GetConnectionString("DefaultConnection"));
});

builder.Services.AddMediator(cfg =>
{
    cfg.RegisterServicesFromAssembly(typeof(CategoryCreateCommand).Assembly);
    cfg.RegisterServicesFromAssembly(typeof(GetCategoriesQueryHandler).Assembly);
});

builder.Services.AddValidatorsFromAssembly(typeof(GetCategoriesQuery).Assembly);
builder.Services.AddValidatorsFromAssembly(typeof(CategoryCreateCommand).Assembly);

builder.Services.AddScoped<ICategoryRepository, CategoryRepository>();
builder.Services.AddScoped<IBasketRepository, BasketRepository>();
builder.Services.AddSingleton<IAggregateRootFactory, AggregateRootFactory>();

builder.Services.AddScoped<IPopulationManager, PopulationManager>();
builder.Services.AddScoped<ICategoryPopulator, CategoryPopulator>();

// Global services
builder.Services.AddScoped<IUnitOfWork, UnitOfWork>();

var provider = builder.Services.BuildServiceProvider();

var populationManager = provider.GetRequiredService<IPopulationManager>();
await populationManager.DoPopulation();
