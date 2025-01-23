using App.Extensions;
using App.Handlers;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace App;

internal static class Program
{
    /// <summary>
    ///  The main entry point for the application.
    /// </summary>
    [STAThread]
    static void Main()
    {
        AssemblyHandler.ResolveCurrentDomainAssembly();

        IHost host = CreateHostBuilder().Build();

        host.InitializeModules();
        host.UseGlobalExceptionHandler();
        host.BootstrapAppConfig();
        host.RunApplication();
    }

    /// <summary>
    /// Create a host builder to build the service provider
    /// </summary>
    /// <returns></returns>
    static IHostBuilder CreateHostBuilder()
    {
        return Host.CreateDefaultBuilder()
            .ConfigureServices((context, services) => {
                services.AddInfrastructure();
                services.AddSingleton<ExceptionHandler>();
                services.AddComponents();
                services.AddForms();
                services.AddEvents();
            });
    }
}