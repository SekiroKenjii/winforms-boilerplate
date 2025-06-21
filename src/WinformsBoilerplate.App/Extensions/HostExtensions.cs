using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using WinformsBoilerplate.Core.Abstractions.Host;
using WinformsBoilerplate.Core.Entities.Systems;
using WinformsBoilerplate.Infrastructure.Extensions;

namespace WinformsBoilerplate.App.Extensions;

/// <summary>
/// Extension methods for the <see cref="IHost"/> interface.
/// </summary>
public static class HostExtensions
{
    /// <summary>
    /// Maps and invokes all registered handlers in the application.
    /// </summary>
    /// <param name="host">The <see cref="IHost"/> instance to invoke handlers on.</param>
    public static void MapHandlers(this IHost host)
    {
        IEnumerable<IHandler> handlers = AssemblyReference.Assembly.GetTypes()
            .Where(x => typeof(IHandler).IsAssignableFrom(x) && x.IsClass)
            .Select(Activator.CreateInstance)
            .Cast<IHandler>();

        foreach (IHandler handler in handlers)
        {
            handler.Invoke(host.Services);
        }
    }

    /// <summary>
    /// Initializes application modules by setting up the required infrastructure for the specified host.
    /// </summary>
    /// <remarks>
    /// This method ensures that the necessary infrastructure components are initialized for the
    /// host. It should be called during application startup to prepare the host for module execution.
    /// </remarks>
    /// <param name="host">The <see cref="IHost"/> instance to initialize modules for.</param>
    public static void InitializeModules(this IHost host)
    {
        host.InitializeInfrastructure();
    }

    /// <summary>
    /// Bootstraps the application by setting up the necessary configurations.
    /// </summary>
    /// <param name="host">The <see cref="IHost"/> instance to bootstrap.</param>
    public static void Bootstrap(this IHost host)
    {
        string[] cliArgs = Environment.GetCommandLineArgs();
        AppArguments args = host.Services.GetRequiredService<AppArguments>().Map(cliArgs);

        Application.EnableVisualStyles();
        Application.SetCompatibleTextRenderingDefault(false);
        _ = args.DpiUnaware != null
            ? Application.SetHighDpiMode(HighDpiMode.DpiUnaware)
            : Application.SetHighDpiMode(HighDpiMode.DpiUnawareGdiScaled);
    }

    /// <summary>
    /// Runs the application using the provided <see cref="IHost"/>.
    /// </summary>
    /// <param name="host">The <see cref="IHost"/> instance to run the application.</param>
    public static void RunApplication(this IHost host)
    {
        // This method can be used to run the application.
    }
}
