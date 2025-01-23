using App.Forms;
using Core.Abstractions.Events;
using Infrastructure.Extensions;
using Microsoft.Extensions.DependencyInjection;

namespace App.Extensions;

public static class ServiceCollectionExtensions
{
    public static void AddInfrastructure(this IServiceCollection services)
    {
        services.AddServices();
        services.AddStores();
    }

    public static void AddComponents(this IServiceCollection services)
    {

    }

    public static void AddForms(this IServiceCollection services)
    {
        services.AddSingleton<MainForm>();
    }

    public static void AddEvents(this IServiceCollection services)
    {
        services.AddSingleton<IMainFormEvent, MainForm>();
    }
}
