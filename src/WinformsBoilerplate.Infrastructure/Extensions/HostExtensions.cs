using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using WinformsBoilerplate.Core.Abstractions.Services;

namespace WinformsBoilerplate.Infrastructure.Extensions;

public static class HostExtensions
{
    public static void InitializeInfrastructure(this IHost host)
    {
        host.InitializeLoggerService();
    }

    private static void InitializeLoggerService(this IHost host)
    {
        ILogService logger = host.Services.GetRequiredService<ILogService>();

        logger.CreateFileLoggers();
        logger.CreateControlLogger();
    }
}
