using Microsoft.Extensions.DependencyInjection;
using WinformsBoilerplate.Core.Abstractions;

namespace WinformsBoilerplate.App.Extensions;

/// <summary>
/// Provides extension methods for the <see cref="IServiceProvider"/> interface to enhance its functionality.
/// </summary>
/// <remarks>This class contains static methods that extend the capabilities of the <see cref="IServiceProvider"/>
/// interface, allowing for more convenient or advanced usage patterns when working with dependency injection in .NET
/// applications.</remarks>
public static class ServiceProviderExtensions
{
    /// <summary>
    /// Resolves a dependency of the specified type from the provided <see cref="IServiceProvider"/>.
    /// </summary>
    /// <typeparam name="TDependency">The type of the dependency to resolve. Must implement <see cref="IDependency"/> and cannot be null.</typeparam>
    /// <param name="sp">The <see cref="IServiceProvider"/> used to resolve the dependency.</param>
    /// <returns>An instance of the specified dependency type <typeparamref name="TDependency"/>.</returns>
    public static TDependency Resolve<TDependency>(this IServiceProvider sp)
        where TDependency : notnull, IDependency
    {
        return sp.GetRequiredService<TDependency>();
    }
}
