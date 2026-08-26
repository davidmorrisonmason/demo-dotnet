using Microsoft.Extensions.Logging;

namespace Demo.Model.Logging;

public interface IDomainContext
{
    ILogger<T> CreateLogger<T>();
}
