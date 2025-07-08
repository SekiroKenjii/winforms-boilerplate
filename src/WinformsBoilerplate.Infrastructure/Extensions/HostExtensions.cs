using Microsoft.Extensions.Hosting;

namespace WinformsBoilerplate.Infrastructure.Extensions;

/// <summary>
/// Extension methods for the <see cref="IHost"/> interface.
/// </summary>
public static class HostExtensions
{
    /// <summary>
    /// Initializes the infrastructure services required by the application.
    /// </summary>
    /// <remarks>This method sets up essential services, such as logging, for the provided <see cref="IHost"/>
    /// instance. It should be called during application startup to ensure proper configuration of infrastructure
    /// components.</remarks>
    /// <param name="host">The <see cref="IHost"/> instance to initialize infrastructure services for.</param>
    public static void InitializeInfrastructure(this IHost host)
    {
        // TODO: Implement any additional infrastructure initialization logic here if needed.
    }
}
