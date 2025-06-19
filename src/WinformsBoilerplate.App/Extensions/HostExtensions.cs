using Microsoft.Extensions.Hosting;
using WinformsBoilerplate.Core.Abstractions.Host;

namespace WinformsBoilerplate.App.Extensions;

/// <summary>
/// Extension methods for the <see cref="IHost"/> interface.
/// </summary>
public static class HostExtensions
{
    /// <summary>
    /// Maps and invokes all registered handlers in the application.
    /// </summary>
    /// <param name="host">The host instance.</param>
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
}
