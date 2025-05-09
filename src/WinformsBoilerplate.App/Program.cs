using Microsoft.Extensions.Hosting;

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
    }

    static IHostBuilder CreateHostBuilder()
    {
        return Host.CreateDefaultBuilder()
            .ConfigureServices((context, services) => {
                //
            });
    }
}