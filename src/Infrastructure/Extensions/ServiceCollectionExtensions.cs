using Core.Abstractions.Services;
using Core.Abstractions.Stores;
using Infrastructure.Services;
using Infrastructure.Stores;
using Microsoft.Extensions.DependencyInjection;

namespace Infrastructure.Extensions;

public static class ServiceCollectionExtensions
{
    public static void AddServices(this IServiceCollection services)
    {
        services.AddSingleton<ILogService, LogService>();
        services.AddSingleton<ISystemService, SystemService>();
    }

    public static void AddStores(this IServiceCollection services)
    {
        services.AddSingleton<IEventStore, EventStore>();
    }
}
