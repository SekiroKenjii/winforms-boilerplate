using System.Reflection;
using System.Runtime.Loader;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using Microsoft.Extensions.Primitives;

namespace WinformsBoilerplate.Infrastructure.Tests;

/// <summary>
/// Module initializer to ensure Microsoft.Extensions assemblies are loaded early.
/// </summary>
internal static class ModuleInitializer
{
    [System.Runtime.CompilerServices.ModuleInitializer]
    internal static void Initialize()
    {
        // Force load Microsoft.Extensions assemblies to prevent runtime loading issues
        try
        {
            // These calls will force the assemblies to be loaded into the AppDomain
            _ = typeof(ServiceCollection);
            _ = typeof(IOptions<>);
            _ = typeof(StringValues);
            _ = typeof(Microsoft.Extensions.Logging.ILogger);
            _ = typeof(Microsoft.Extensions.Configuration.IConfiguration);
            _ = typeof(Microsoft.Extensions.DependencyInjection.ServiceDescriptor);
        }
        catch
        {
            // Ignore any loading errors - this is just to preload assemblies
        }
    }
}
