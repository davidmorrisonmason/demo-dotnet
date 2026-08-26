using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace Demo.Model.Logging;

/// <summary>
/// Singleton context for the provision of domain services where these can't be injected (e.g. in domain layer where
/// objects are coming back from the DB)
/// </summary>
public class DomainContext : IDomainContext
{
    private readonly IServiceProvider _serviceProvider;
    public static IDomainContext Instance { get; private set; }

    protected DomainContext(IServiceProvider serviceProvider)
    {
        _serviceProvider = serviceProvider;
    }

    /// <summary>
    /// Setup for use in live code
    /// </summary>
    public static void Setup(IServiceProvider serviceProvider)
    {
        Instance = new DomainContext(serviceProvider);
    }

    /// <summary>
    /// Setup for use in test code
    /// </summary>
    public static void Setup(IDomainContext mockDomainContext)
    {
        Instance = mockDomainContext;
    }

    public ILogger<T> CreateLogger<T>()
    {
        return _serviceProvider.GetService<ILogger<T>>();
    }
}
