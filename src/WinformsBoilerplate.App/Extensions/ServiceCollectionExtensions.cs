using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using WinformsBoilerplate.Core.Constants;
using WinformsBoilerplate.Core.Entities.Settings;
using WinformsBoilerplate.Core.Helpers;
using WinformsBoilerplate.Infrastructure.Extensions;

namespace WinformsBoilerplate.App.Extensions;

/// <summary>
/// Extension methods for configuring services in the dependency injection container.
/// </summary>
public static class ServiceCollectionExtensions
{
    /// <summary>
    /// Adds infrastructure services to the service collection.
    /// </summary>
    /// <param name="services">The <see cref="IServiceCollection"/> to which infrastructure services will be added.</param>
    public static void AddInfrastructure(this IServiceCollection services)
    {
        services.AddSerializer();
        services.AddServices();
        services.AddStores();
    }

    /// <summary>
    /// Binds application settings from a JSON file to the service collection.
    /// </summary>
    /// <param name="services">The <see cref="IServiceCollection"/> to which application settings will be bound.</param>
    public static void BindSettings(this IServiceCollection services)
    {
        IConfigurationRoot config = new ConfigurationBuilder()
            .SetBasePath(CommonHelpers.AppStartupPath())
            .AddJsonFile(Files.SETTING_FILE, optional: true, reloadOnChange: true)
            .Build();

        _ = services.Configure<AppSettings>(config.GetSection(nameof(AppSettings)));

        AppSettings appSettings = config.GetSection(nameof(AppSettings)).Get<AppSettings>() ?? new AppSettings();
        _ = services.AddSingleton(appSettings);
    }

    /// <summary>
    /// Adds application-specific components to the specified service collection.
    /// </summary>
    /// <param name="services">The <see cref="IServiceCollection"/> to which the components will be added.</param>
    public static void AddComponents(this IServiceCollection services)
    {
        // Register your components here
        // For example: services.AddSingleton<MyComponent>();
    }
}
