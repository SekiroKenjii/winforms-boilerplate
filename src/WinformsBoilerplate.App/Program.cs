using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using WinformsBoilerplate.App.Extensions;
using WinformsBoilerplate.App.Helpers;
using WinformsBoilerplate.Core.Entities.Systems;

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

        AssemblyHelpers.ResolveCurrentDomainAssembly();

        IHost host = CreateHostBuilder().Build();

        host.MapHandlers();
        host.InitializeModules();
        host.Bootstrap();
        host.RunApplication();
    }

    /// <summary>
    /// Creates a host builder for the application.
    /// </summary>
    /// <returns>A program initialization builder for the application.</returns>
    static IHostBuilder CreateHostBuilder()
    {
        return Host.CreateDefaultBuilder()
            .ConfigureServices((context, services) => {
                services.AddSingleton<AppArguments>()
                        .AddInfrastructure();

                services.BindSettings();
            });
    }
}
