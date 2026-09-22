using Demo.Api.HealthChecks;
using Demo.Api.Response;
using Demo.Api.Swagger;
using Demo.Api.Validation;
using Demo.DomainServices.Context;
using Demo.DomainServices.Creation;
using Demo.DomainServices.DependencyInjection;
using Demo.DomainServices.Encryption;
using Demo.DomainServices.Interface.Context;
using Demo.DomainServices.Interface.Encryption;
using Demo.DomainServices.Interface.Repository;
using Demo.DomainServices.Interface.Time;
using Demo.DomainServices.Interface.Transaction;
using Demo.DomainServices.Time;
using Demo.Infrastructure.Data;
using Demo.Infrastructure.Repository;
using Demo.Model.Domain.Exceptions;
using Demo.Model.Domain.Validation;
using Demo.Model.Logging;
using Demo.Populator.Core;
using Demo.Populator.Core.Interfaces;
using Demo.Populator.Core.Populators;
using FluentValidation;
using Mapster;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.HttpLogging;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Net.Http.Headers;
using System.Reflection;
using System.Text.Json;

namespace Demo.Api.Configuration
{
    /// <summary>
    /// Configuration helper for a web API
    /// </summary>
    public static class ApiConfigurator
    {
        /// <summary>
        /// Global service configuration for a standard web API. 
        /// Adds controllers and swagger options.
        /// </summary>
        /// <param name="services">The service collection</param>
        /// <param name="queryAssembly">The query assembly</param>
        /// <param name="commandAssembly">The command assembly</param>
        /// <param name="apiAssembly">The API assembly</param>
        public static void ConfigureServices(IServiceCollection services, Assembly queryAssembly, Assembly commandAssembly, Assembly apiAssembly)
        {
            string assemblyName = apiAssembly.GetName().Name!;

            services.AddMediator(cfg =>
            {
                cfg.RegisterServicesFromAssembly(queryAssembly);
                cfg.RegisterServicesFromAssembly(commandAssembly);
            });

            services.AddValidatorsFromAssembly(queryAssembly);
            services.AddValidatorsFromAssembly(commandAssembly);

            services.AddScoped<ICategoryRepository, CategoryRepository>();
            services.AddScoped<IBasketRepository, BasketRepository>();
            services.AddScoped<ICheckoutCompletionRepository, CheckoutCompletionRepository>();
            services.AddScoped<IClientRepository, ClientRepository>();
            services.AddSingleton<IAggregateRootFactory, AggregateRootFactory>();
            services.AddSingleton<ITimeService, TimeService>();
            services.AddSingleton<IEncryptionService, EncryptionService>();
            services.AddScoped<IRequestContext, RequestContext>();
            services.AddScoped<IUnitOfWork, UnitOfWork>();
            services.AddScoped<IPopulationManager, PopulationManager>();
            services.AddScoped<IClientPopulator, ClientPopulator>();
            services.AddScoped<ICategoryPopulator, CategoryPopulator>();

            services.AddControllers()
            .ConfigureApiBehaviorOptions(options =>
            {
                options.InvalidModelStateResponseFactory = context =>
                {
                    List<ErrorMessageDto> errorMessages = [];

                    foreach (var entry in context.ModelState.Values)
                    {
                        if (entry.Errors.Any())
                        {
                            foreach (var error in entry.Errors)
                            {
                                ErrorMessage parsedErrorMessage = new("", "");

                                if (error.ErrorMessage.TryGetValidationErrorMessage(out parsedErrorMessage))
                                {
                                    errorMessages.Add(parsedErrorMessage.Adapt<ErrorMessageDto>());
                                }
                                else
                                {
                                    errorMessages.Add(new ErrorMessageDto("UNKNOWN", error.ErrorMessage));
                                }
                            }
                        }

                    }
                    var problemDetails = new ApiProblemDetails
                    {
                        Type = $"DemoAPI/ValidationError",
                        Title = "One or more validation errors occurred",
                        Detail = "The request contains invalid parameters. More information can be found in the errors.",
                        Status = StatusCodes.Status400BadRequest,
                        Errors = errorMessages
                    };

                    return new BadRequestObjectResult(problemDetails);
                };
            });

            // Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
            services.AddEndpointsApiExplorer();
            services.AddSwaggerGen(options =>
            {
                options.OperationFilter<ApiKeyHeaderOperationFilter>();
                var xmlFilename = $"{assemblyName}.xml";
                options.IncludeXmlComments(Path.Combine(AppContext.BaseDirectory, xmlFilename));
                options.EnableAnnotations();
            });

            // Logging
            services.AddHttpLogging(logging =>
            {
                logging.LoggingFields = HttpLoggingFields.RequestPath;
            });

            // Health checks
            services.AddHealthChecks().AddCheck<BasicHealthCheck>("Basic Health Check");
        }

        /// <summary>
        /// Helper method to configure an API application
        /// </summary>
        /// <param name="app">The web application</param>
        /// <param name="baseRoute">The base route</param>
        public static async Task ConfigureApplication(this WebApplication app, string baseRoute)
        {
            // Logging provider for getting loggers in situations where DI isn't possible
            DomainContext.Setup(app.Services);

            // Configure the HTTP request pipeline.
            if (app.Environment.IsDevelopment())
            {
                app.UseSwagger();
                app.UseSwaggerUI();
                app.UseDeveloperExceptionPage();
            }

            app.UseHttpLogging();

            app.ConfigureExceptionHandling();

            app.UseWhen(
                context => context.Request.Path.StartsWithSegments($"/{baseRoute}"),
                branch => branch.UseMiddleware<Demo.Api.Middleware.ApiKeyMiddleware>());

            app.UseHttpsRedirection();

            app.UseAuthorization();

            app.MapControllers();

            app.MapHealthChecks($"{baseRoute}/healthcheck");

            // Database. In a real system there would be no dependency on a populator project from the prod code or DI plumbing for the populators (see above),
            // but this is just so we an seed an empty SQLite database within the appdata directory with data on application startup when deploying somewhere,
            // without the need to create a database server etc. Purely for demo purposes.
            if (app.Environment.IsProduction())
            {
                using (var scope = app.Services.CreateScope())
                {
                    var db = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
                    await db.Database.MigrateAsync();

                    var populator = scope.ServiceProvider.GetRequiredService<IPopulationManager>();
                    await populator.DoPopulation();
                }
            }
        }

        private static void ConfigureExceptionHandling(this WebApplication app)
        {
            app.UseExceptionHandler(errorApp =>
            {
                errorApp.Run(async context =>
                {
                    var errorFeature = context.Features.Get<IExceptionHandlerFeature>();
                    var exception = errorFeature!.Error;

                    // https://tools.ietf.org/html/rfc7807#section-3.1
                    var problemDetails = new ApiProblemDetails
                    {
                        Errors = [],
                        Status = StatusCodes.Status400BadRequest,
                        Type = $"{exception.GetType().Name}",
                        Title = "An unexpected error occurred",
                        Detail = "Something went wrong",
                    };

                    switch (exception)
                    {
                        case Model.Domain.Exceptions.ApplicationException applicationException:
                            problemDetails.Status = applicationException is AuthorisationException ? StatusCodes.Status401Unauthorized : StatusCodes.Status400BadRequest;
                            problemDetails.Title = "One or more validation errors occurred";
                            problemDetails.Detail = "The request contains invalid parameters. More information can be found in the errors.";
                            problemDetails.Errors = [.. applicationException.ErrorMessages.Select(x => x.Adapt<ErrorMessageDto>())];
                            break;
                    }

                    context.Response.ContentType = "application/problem+json";
                    context.Response.StatusCode = problemDetails.Status;
                    context.Response.GetTypedHeaders().CacheControl = new CacheControlHeaderValue()
                    {
                        NoCache = true,
                    };
                    await JsonSerializer.SerializeAsync(context.Response.Body, problemDetails, new JsonSerializerOptions() { PropertyNamingPolicy = JsonNamingPolicy.CamelCase });
                });
            });
        }
    }
}
