namespace GeoChat.Identity.Api.RabbitMqEventBus.ConnectionManager;

public class MessageEventArgs : EventArgs
{
    public MessageEventArgs(string message, string routing, string exchange)
    {
        Message = message;
        Routing = routing;
        Exchange = exchange;
    }

    public string? Message { get; }
    public string? Routing { get; }
    public string Exchange { get; }
}
