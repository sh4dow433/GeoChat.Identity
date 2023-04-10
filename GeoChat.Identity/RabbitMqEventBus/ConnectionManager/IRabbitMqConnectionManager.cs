using RabbitMQ.Client;
using RabbitMQ.Client.Events;

namespace GeoChat.Identity.Api.RabbitMqEventBus.ConnectionManager;

public interface IRabbitMqConnectionManager : IDisposable
{
    IModel Channel { get; }
    event AsyncEventHandler<MessageEventArgs>? EventReceived;
    string StartConsumer(string exchange, string exchangeType, string queue, string routing, bool autodelete = false);
    void StopConsumer(string consumerTag);
}
