using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using WinformsBoilerplate.App.Extensions;
using WinformsBoilerplate.Core.Entities.Settings;
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
        IHost host = CreateHostBuilder().Build();

        host.MapHandlers();
    }

    /// <summary>
    /// Creates a host builder for the application.
    /// </summary>
    /// <returns>A program initialization builder for the application.</returns>
    static IHostBuilder CreateHostBuilder()
    {
        return Host.CreateDefaultBuilder()
            .ConfigureServices((context, services) => {
                _ = services.AddSingleton<AppArguments>()
                            .AddSingleton<AppSettings>();
            });
    }
}
