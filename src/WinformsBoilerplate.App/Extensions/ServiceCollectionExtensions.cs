using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using WinformsBoilerplate.App.Components.Controls;
using WinformsBoilerplate.App.Components.Forms;
using WinformsBoilerplate.Core.Abstractions;
using WinformsBoilerplate.Core.Abstractions.Components;
using WinformsBoilerplate.Core.Abstractions.Components.Controls;
using WinformsBoilerplate.Core.Abstractions.Components.Forms;
using WinformsBoilerplate.Core.Abstractions.Services;
using WinformsBoilerplate.Core.Constants;
using WinformsBoilerplate.Core.Entities.Settings;
using WinformsBoilerplate.Core.Helpers;
using WinformsBoilerplate.Core.Wrappers;
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
    public static IServiceCollection BindSettings(this IServiceCollection services)
    {
        IServiceProvider sp = services.BuildServiceProvider();

        IAppSettingService appSettingService = sp.Resolve<IAppSettingService>();
        ISystemService systemService = sp.Resolve<ISystemService>();

        ThrowableFunction<AppSetting?, Exception> checkResult = systemService.CheckAppSettingFile();

        if (checkResult.Exception is not null)
        {
            DialogResult recoverOption = MessageBox.Show(
                "The settings file is corrupted or unreadable.\n\n" +
                "Select 'Try Again' to restart the application.\n" +
                "Select 'Continue' to delete the corrupted file and recreate it with default settings.\n" +
                "Select 'Cancel' to exit the application without making any changes.",
                $"{Application.ProductName} | Setting file corrupted!",
                MessageBoxButtons.CancelTryContinue,
                MessageBoxIcon.Warning
            );

            switch (recoverOption)
            {
                case DialogResult.Cancel:
                    // Exit the application without making any changes
                    systemService.ShutdownApplication();
                    return services;
                case DialogResult.TryAgain:
                    // Restart the application to try again
                    systemService.RestartApplication();
                    return services;
                case DialogResult.Continue:
                    // Delete the corrupted file and recreate it with default settings
                    if (!appSettingService.CreateDefaultSettingFile(true))
                    {
                        _ = MessageBox.Show(
                            "Failed to create a new settings file. The application will now exit.",
                            $"{Application.ProductName} | Error",
                            MessageBoxButtons.OK,
                            MessageBoxIcon.Error
                        );
                        systemService.ShutdownApplication();
                        return services;
                    }

                    break;
            }
        }

        IConfigurationRoot config = new ConfigurationBuilder()
            .SetBasePath(CommonHelpers.AppStartupPath())
            .AddJsonFile(Files.SETTING_FILE, optional: false, reloadOnChange: true)
            .Build();

        return services.Configure<AppSetting>(config.GetSection(nameof(AppSetting)));
    }

    /// <summary>
    /// Adds application-specific components to the specified service collection.
    /// </summary>
    /// <param name="services">The <see cref="IServiceCollection"/> to which the components will be added.</param>
    public static void AddComponents(this IServiceCollection services)
    {
        #region Controls
        services.AddControls<IOverlayControl, OverlayControl>();
        #endregion

        #region Modals
        // Adds modal components to the service collection.
        #endregion

        #region Dialogs
        // Adds dialog components to the service collection.
        #endregion

        #region Forms
        services.AddForms<IMainForm, MainForm>();
        #endregion
    }

    private static void AddForms<TService, TImplementation>(this IServiceCollection services)
        where TService : class, IForm, ISingletonDependency
        where TImplementation : class, TService
    {
        _ = services.AddSingleton<TService, TImplementation>();
    }

    private static void AddDialogs<TService, TImplementation>(this IServiceCollection services)
        where TService : class, IDialog, ISingletonDependency
        where TImplementation : class, TService
    {
        _ = services.AddSingleton<TService, TImplementation>();
    }

    private static void AddModals<TService, TImplementation>(this IServiceCollection services)
        where TService : class, IModal, ITransientDependency
        where TImplementation : class, TService
    {
        _ = services.AddTransient<TService, TImplementation>();
    }

    private static void AddControls<TService, TImplementation>(this IServiceCollection services)
        where TService : class, IControl, ITransientDependency
        where TImplementation : class, TService
    {
        _ = services.AddTransient<TService, TImplementation>();
    }
}
