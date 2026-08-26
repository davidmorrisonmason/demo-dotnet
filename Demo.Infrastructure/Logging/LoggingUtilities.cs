using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Serilog;
namespace Demo.Api.Logging;

public static class LoggingUtilities
{
    private const string OutputTemplate = "{Timestamp:yyyy-MM-dd HH:mm:ss.fff zzz} [{Level}] {SourceContext} {Message}{NewLine}{Exception}";

    public static void ConfigureLogging(
        IServiceCollection services,
        ConfigurationManager configuration)
    {
        Log.Logger = new LoggerConfiguration()
                        .ReadFrom.Configuration(configuration)
                        .Enrich.FromLogContext()
                        .WriteTo.Console(outputTemplate: OutputTemplate)
                        .WriteTo.File("logs/log.txt", rollingInterval: RollingInterval.Day, outputTemplate: OutputTemplate)
                        .CreateLogger();

        services.AddSerilog();
    }
}
