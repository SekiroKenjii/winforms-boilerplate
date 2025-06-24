using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using WinformsBoilerplate.App.Components.Forms;
using WinformsBoilerplate.Core.Abstractions.Components.Forms;
using WinformsBoilerplate.Core.Abstractions.Services;
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
        IServiceProvider sp = services.BuildServiceProvider();

        ISystemService systemService = sp.GetRequiredService<ISystemService>();
        var checkResult = systemService.CheckAppSettingFile();

        if (checkResult.Exception is not null)
        {
            DialogResult recover = MessageBox.Show(
                "The settings file is corrupted or unreadable.\n\n" +
                "Select 'Try Again' to restart the application.\n" +
                "Select 'Continue' to delete the corrupted file and recreate it with default settings.\n" +
                "Select 'Cancel' to exit the application without making any changes.",
                $"{Application.ProductName} | Setting file corrupted!",
                MessageBoxButtons.CancelTryContinue,
                MessageBoxIcon.Error
            );

            switch (recover)
            {
                case DialogResult.Cancel:
                    // Exit the application without making any changes
                    systemService.ShutdownApplication();
                    return;
                case DialogResult.TryAgain:
                    // Restart the application to try again
                    systemService.RestartApplication();
                    return;
                case DialogResult.Continue:
                    // Delete the corrupted file and recreate it with default settings
                    if (!systemService.CreateDefaultSettingFile(true))
                    {
                        _ = MessageBox.Show(
                            "Failed to create a new settings file. The application will now exit.",
                            $"{Application.ProductName} | Error",
                            MessageBoxButtons.OK,
                            MessageBoxIcon.Error
                        );
                        systemService.ShutdownApplication();
                        return;
                    }

                    break;
            }
        }

        IConfigurationRoot config = new ConfigurationBuilder()
            .SetBasePath(CommonHelpers.AppStartupPath())
            .AddJsonFile(Files.SETTING_FILE, optional: false, reloadOnChange: true)
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
        // Forms
        _ = services.AddSingleton<IMainForm, MainForm>();
    }
}
