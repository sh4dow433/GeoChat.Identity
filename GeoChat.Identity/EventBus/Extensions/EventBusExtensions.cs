﻿using GeoChat.Identity.Api.EventBus.Events;

namespace GeoChat.Identity.Api.EventBus.Extensions;

public static class EventBusExtensions
{
    public static void PublishNewAccountCreatedEvent(this IEventBus eventBus, IConfiguration configuration, NewAccountCreatedEvent @event)
    {
        var baseCfg = $"RabbitMq:PublishRoutings:{nameof(NewAccountCreatedEvent)}";

        var exchange = configuration[$"{baseCfg}:Exchange"];
        var exchangeType = configuration[$"{baseCfg}:ExchangeType"];
        if (exchange == null || exchangeType == null) 
        {
            throw new Exception("The exchange or exchange type wasn't configured in the appsettings.json file.");
        }
        eventBus.PublishEvent(@event, exchange, exchangeType);
    }

    public static void PublishSyncResponseEvent(this IEventBus eventBus, IConfiguration configuration, SyncResponseEvent @event)
    {
        var baseCfg = $"RabbitMq:PublishRoutings:{nameof(SyncResponseEvent)}";

        var exchange = configuration[$"{baseCfg}:Exchange"];
        var exchangeType = configuration[$"{baseCfg}:ExchangeType"];
        var routingKey = configuration[$"{baseCfg}:RoutingKey"];
        if (exchange == null || exchangeType == null)
        {
            throw new Exception("The exchange or exchange type wasn't configured in the appsettings.json file.");
        }

        eventBus.PublishEvent(@event, exchange, exchangeType, routingKey);
    }
}
