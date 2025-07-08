using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using WinformsBoilerplate.App.Extensions;
using WinformsBoilerplate.App.Helpers;
using WinformsBoilerplate.Core.Entities.Systems;
using WinformsBoilerplate.Infrastructure.Logging;

namespace WinformsBoilerplate.App;

internal static class Program
{
    /// <summary>
    ///  The main entry point for the application.
    /// </summary>
    [STAThread]
    static void Main()
    {
        if (!AssemblyHelpers.ValidateLibVersions(out Version requiredVersion))
        {
            _ = MessageBox.Show(
                $"One or more dependent libraries are out of date. Minimum required version is {requiredVersion}.",
                Application.ProductName,
                MessageBoxButtons.OK,
                MessageBoxIcon.Error);

            return;
        }

        if (AssemblyHelpers.DetectCurrentInstance())
        {
            // TODO: Implement a mechanism to handle multiple instances if needed

            return;
        }

        AssemblyHelpers.ResolveCurrentDomainAssembly();

        IHost host = BuildAppHost();

        host.MapHandlers();
        host.InitializeModules();
        host.Bootstrap();
        host.RunApplication();
    }

    /// <summary>
    /// Builds and configures an <see cref="IHost"/> for the application.
    /// </summary>
    /// <returns>An <see cref="IHost"/> instance that represents the configured application host.</returns>
    static IHost BuildAppHost()
    {
        HostApplicationBuilder app = Host.CreateApplicationBuilder();

        app.Services
            .AddSingleton<AppArguments>()
            .AddInfrastructure();

        app.Logging
            .ClearProviders()
            .SetMinimumLevel(LogLevel.Information)
            .AddLogService(options => {
                if (!app.Environment.IsProduction())
                {
                    options.MinimumLogLevel = LogLevel.Debug;
                    options.LogLevelOverrides.Add("Microsoft", LogLevel.Debug);
                    options.LogLevelOverrides.Add("System", LogLevel.Debug);

                    return;
                }

                options.MinimumLogLevel = LogLevel.Warning;
                options.LogLevelOverrides.Add("Microsoft", LogLevel.Warning);
                options.LogLevelOverrides.Add("System", LogLevel.Warning);
            });

        app.Services
            .BindSettings()
            .AddComponents();

        return app.Build();
    }
}
