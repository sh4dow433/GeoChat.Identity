using GeoChat.Identity.Api.EventBus;
using GeoChat.Identity.Api.RabbitMqEventBus.ConnectionManager;
using GeoChat.Identity.Api.RabbitMqEventBus.EventsManager;

namespace GeoChat.Identity.Api.RabbitMqEventBus;

public static class ServiceCollectionExtensions
{
    public static void RegisterEventBus(this IServiceCollection services)
    {
        services.AddSingleton<IEventManager, EventManager>();
        services.AddSingleton<IRabbitMqConnectionManager, RabbitMqConnectionManager>();
        services.AddSingleton<IEventBus, EventBus>();
    }
}
