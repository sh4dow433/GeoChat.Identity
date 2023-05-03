using GeoChat.Identity.Api.EventBus.EventHandlers;
using GeoChat.Identity.Api.EventBus.Events;

namespace GeoChat.Identity.Api.RabbitMqEventBus.EventsManager;

public interface IEventManager
{
    void Subscribe<TEvent, TEventHandler>()
        where TEvent : BaseEvent
        where TEventHandler : IEventHandler<TEvent>;

    void Unsubscribe<TEvent, TEventHandler>()
        where TEvent : BaseEvent
        where TEventHandler : IEventHandler<TEvent>;


    IEnumerable<Type> GetHandlers(string eventName);
    int GetHandlersCount<TEvent>() where TEvent : BaseEvent;


    Type GetEventTypeFromRoutingKey(string exchange, string routingKey);
    EventDetails GetEventDetails<TEvent>() where TEvent : BaseEvent;
}
