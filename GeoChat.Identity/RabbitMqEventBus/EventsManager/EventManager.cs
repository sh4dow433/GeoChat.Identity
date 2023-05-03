using GeoChat.Identity.Api.EventBus.EventHandlers;
using GeoChat.Identity.Api.EventBus.Events;
using RabbitMQ.Client;

namespace GeoChat.Identity.Api.RabbitMqEventBus.EventsManager;

public class EventManager : IEventManager
{
    private readonly IConfiguration _configuration;
    private readonly Dictionary<string, List<Type>> _eventHandlers = new();
    private readonly List<(Type, EventDetails)> _eventDetails = new();
    public EventManager(IConfiguration configuration)
    {
        _configuration = configuration;
    }
    public EventDetails GetEventDetails<TEvent>() where TEvent : BaseEvent
    {
        foreach (var (eventType, details) in _eventDetails)
        {
            if (eventType == typeof(TEvent))
            {
                return details;
            }
        }
        throw new Exception("Couldn't find any event details for the given event");
    }

    public Type GetEventTypeFromRoutingKey(string exchange, string routingKey)
    {
        foreach (var (eventType, details) in _eventDetails)
        {
            if (details.Exchange == exchange && (details.RoutingKey == routingKey || details.ExchangeType == ExchangeType.Fanout))
            {
                return eventType;
            }
        }
        throw new Exception("Couldn't find any event type for the given routing key");
    }

    public IEnumerable<Type> GetHandlers(string eventName)
    {
        if (_eventHandlers.ContainsKey(eventName) == false)
        {
            throw new Exception($"Event type of {eventName} couldn't be found");
        }

        return _eventHandlers[eventName].AsReadOnly();
    }

    public int GetHandlersCount<TEvent>() where TEvent : BaseEvent
    {
        string eventName = typeof(TEvent).Name;
        if (_eventHandlers.ContainsKey(eventName) == false)
        {
            throw new Exception($"Event type of {eventName} couldn't be found");
        }
        return _eventHandlers[eventName].Count;

    }

    public void Subscribe<TEvent, TEventHandler>()
        where TEvent : BaseEvent
        where TEventHandler : IEventHandler<TEvent>
    {
        var eventType = typeof(TEvent);
        var eventName = eventType.Name;
        var eventHandlerType = typeof(TEventHandler);

        var eventDetails = _configuration.GetSection($"RabbitMq:SubscribeRoutings:{eventName}").Get<EventDetails>();
        if (eventDetails == null)
        {
            throw new Exception("Details about the event couldn't be found in the configuration file");
        }
        _eventDetails.Add((eventType, eventDetails));

        if (_eventHandlers.ContainsKey(eventName) == false)
        {
            _eventHandlers[eventName] = new();
        }
        _eventHandlers[eventName].Add(eventHandlerType);
    }

    public void Unsubscribe<TEvent, TEventHandler>()
        where TEvent : BaseEvent
        where TEventHandler : IEventHandler<TEvent>
    {
        var eventType = typeof(TEvent);
        var eventName = eventType.Name;
        var eventHandlerType = typeof(TEventHandler);

        if (_eventHandlers.ContainsKey(eventName) == false)
        {
            return;
        }
        _eventHandlers[eventName].RemoveAll(eh => eh == eventHandlerType);

        if (_eventHandlers[eventName].Count == 0)
        {
            _eventDetails.RemoveAll(typeAndDetails => typeAndDetails.Item1 == eventType);
            _eventHandlers.Remove(eventName);
        }
    }
}
