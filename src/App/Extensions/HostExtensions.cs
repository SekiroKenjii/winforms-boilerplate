using App.Forms;
using App.Handlers;
using Infrastructure.Extensions;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace App.Extensions;

public static class HostExtensions
{
    public static T Resolve<T>(this IHost host) where T : class
    {
        return host.Services.GetRequiredService<T>();
    }

    public static void UseGlobalExceptionHandler(this IHost host)
    {
        host.Resolve<ExceptionHandler>().Register();
    }

    public static void InitializeModules(this IHost host)
    {
        host.InitializeInfrastructure();
    }

    public static void BootstrapAppConfig(this IHost host)
    {
        Application.EnableVisualStyles();
        Application.SetCompatibleTextRenderingDefault(false);

        // TODO: detect HiDPI setting
        Application.SetHighDpiMode(HighDpiMode.PerMonitorV2);
    }

    public static void RunApplication(this IHost host)
    {
        Application.Run(host.Resolve<MainForm>());
    }
}
