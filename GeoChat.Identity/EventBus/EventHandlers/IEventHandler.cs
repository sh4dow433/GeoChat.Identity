using GeoChat.Identity.Api.EventBus.Events;

namespace GeoChat.Identity.Api.EventBus.EventHandlers;

public interface IEventHandler<TEvent> where TEvent : BaseEvent
{
    Task HandleAsync(TEvent @event);
}
