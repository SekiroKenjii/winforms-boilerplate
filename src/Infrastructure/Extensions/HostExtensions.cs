using Core.Abstractions.Services;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace Infrastructure.Extensions;

public static class HostExtensions
{
    public static void InitializeInfrastructure(this IHost host)
    {
        host.InitializeLoggerService();
    }

    private static void InitializeLoggerService(this IHost host)
    {
        ILogService logService = host.Services.GetRequiredService<ILogService>();

        logService.CreateFileLoggers();
        logService.CreateControlLogger();
    }
}
