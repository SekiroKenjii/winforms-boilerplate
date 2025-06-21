using Microsoft.Extensions.DependencyInjection;
using WinformsBoilerplate.Core.Abstractions.Serializers;
using WinformsBoilerplate.Core.Abstractions.Services;
using WinformsBoilerplate.Infrastructure.Serializer;
using WinformsBoilerplate.Infrastructure.Services;

namespace WinformsBoilerplate.Infrastructure.Extensions;

/// <summary>
/// Extension methods for configuring services in the dependency injection container.
/// </summary>
public static class ServiceCollectionExtensions
{
    /// <summary>
    /// Adds the application's serializers to the container.
    /// </summary>
    /// <param name="services">The service collection.</param>
    public static void AddSerializer(this IServiceCollection services)
    {
        _ = services.AddSingleton<IJsonSerializer, DefaultSerializer>();
    }

    /// <summary>
    /// Adds the application's services to the container.
    /// </summary>
    /// <param name="services">The service collection.</param>
    public static void AddServices(this IServiceCollection services)
    {
        _ = services
            .AddSingleton<ILogService, LoggerService>()
            .AddSingleton<ISystemService, SystemService>()
            .AddSingleton<ILayoutService, LayoutService>();
    }

    /// <summary>
    /// Adds the application's stores to the container.
    /// </summary>
    /// <param name="services">The service collection.</param>
    public static void AddStores(this IServiceCollection services)
    {
        // Register your stores here
    }
}
