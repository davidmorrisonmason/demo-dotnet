using System.Reflection;

namespace Demo.ApplicationServices.Interface.Orchestration;

public interface IMediatorServiceConfiguration
{
    /// <summary>
    /// Registers all mediator requests and handlers defined within the supplied assembly
    /// </summary>
    /// <param name="assembly"></param>
    void RegisterServicesFromAssembly(Assembly assembly);
}
