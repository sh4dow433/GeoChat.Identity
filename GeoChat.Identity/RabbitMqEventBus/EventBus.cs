using GeoChat.Identity.Api.EventBus;
using GeoChat.Identity.Api.EventBus.EventHandlers;
using GeoChat.Identity.Api.EventBus.Events;
using GeoChat.Identity.Api.RabbitMqEventBus.ConnectionManager;
using GeoChat.Identity.Api.RabbitMqEventBus.EventsManager;
using Microsoft.Extensions.DependencyInjection;
using RabbitMQ.Client;
using System.Text;
using System.Text.Json;

namespace GeoChat.Identity.Api.RabbitMqEventBus;

internal class EventBus : IEventBus
{
    private readonly IEventManager _eventManager;
    private readonly IRabbitMqConnectionManager _mqConnectionManager;
    private readonly IServiceScopeFactory _scopeFactory;
    private readonly Dictionary<Type, string> _eventsConsumers = new();

    public EventBus(IEventManager eventManager,
        IRabbitMqConnectionManager mqConnectionManager,
        IServiceScopeFactory scopeFactory)
    {
        _eventManager = eventManager;

        _mqConnectionManager = mqConnectionManager;
        _mqConnectionManager.EventReceived += ProcessEvent;

        _scopeFactory = scopeFactory;
    }

    public void PublishEvent<TEvent>(TEvent @event, string exchange, string exchangeType, string routingKey = "")
        where TEvent : BaseEvent
    {
        var channel = _mqConnectionManager.Channel;
        channel.ExchangeDeclare(exchange, exchangeType);

        var message = JsonSerializer.Serialize(@event);
        var body = Encoding.UTF8.GetBytes(message);
        channel.BasicPublish(
                exchange: exchange,
                routingKey: routingKey,
                basicProperties: null,
                body: body
            );
    }

    public void Subscribe<TEvent, TEventHandler>()
            where TEvent : BaseEvent
            where TEventHandler : IEventHandler<TEvent>
    {
        _eventManager.Subscribe<TEvent, TEventHandler>();

        var details = _eventManager.GetEventDetails<TEvent>();
        var consumerTag = _mqConnectionManager.StartConsumer(details.Exchange,
            details.ExchangeType,
            details.Queue,
            details.RoutingKey,
            details.AutoDelete);

        _eventsConsumers.Add(typeof(TEvent), consumerTag);
    }

    public void Unsubscribe<TEvent, TEventHandler>()
        where TEvent : BaseEvent
        where TEventHandler : IEventHandler<TEvent>
    {

        _eventManager.Unsubscribe<TEvent, TEventHandler>();
        if (_eventManager.GetHandlersCount<TEvent>() == 0)
        {
            var consumerTag = _eventsConsumers[typeof(TEvent)];
            if (consumerTag == null)
            {
                throw new Exception("Consumer couldn't be found");
            }
            _mqConnectionManager.StopConsumer(consumerTag);
        }
    }

    private async Task ProcessEvent(object sender, MessageEventArgs ea)
    {
        var body = ea.Message;
        var routingKey = ea.Routing;
        var exchange = ea.Exchange;

        if (body == null)
        {
            throw new Exception("Missing message");
        }

        if (routingKey == null)
        {
            throw new Exception("Missing routing key");
        }

        using var scope = _scopeFactory.CreateScope();
        var eventType = _eventManager.GetEventTypeFromRoutingKey(exchange, routingKey);
        foreach (var handlerType in _eventManager.GetHandlers(eventType.Name))
        {
            var handler = scope.ServiceProvider.GetService(handlerType);
            var actualEvent = JsonSerializer.Deserialize(body, eventType);
            var handlerConcreteType = typeof(IEventHandler<>).MakeGenericType(handlerType);
            if (handler == null || actualEvent == null || handlerConcreteType == null)
            {
                throw new Exception("Couldn't find or couldn't build the handler");
            }
            await (Task)handlerConcreteType.GetMethod("HandleAsync")?.Invoke(handler, new object[] { actualEvent })!;
        }
    }

    public void Dispose()
    {
        _mqConnectionManager.EventReceived -= ProcessEvent;
    }
}
