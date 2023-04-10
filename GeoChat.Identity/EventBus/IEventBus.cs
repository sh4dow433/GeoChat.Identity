using GeoChat.Identity.Api.EventBus.EventHandlers;
using GeoChat.Identity.Api.EventBus.Events;

namespace GeoChat.Identity.Api.EventBus;

public interface IEventBus : IDisposable
{
    void PublishEvent<TEvent>(TEvent @event, string exchange, string exchangeType, string routingKey = "")
        where TEvent : BaseEvent;

    void Subscribe<TEvent, TEventHandler>()
        where TEvent : BaseEvent
        where TEventHandler : IEventHandler<TEvent>;

    void Unsubscribe<TEvent, TEventHandler>()
        where TEvent : BaseEvent
        where TEventHandler : IEventHandler<TEvent>;
}
