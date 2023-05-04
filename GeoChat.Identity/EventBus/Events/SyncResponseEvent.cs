namespace GeoChat.Identity.Api.EventBus.Events;

public class SyncResponseEvent : BaseEvent
{
    public IEnumerable<NewAccountCreatedEvent> AccountsCreatedOrUpdated { get; set; } = new List<NewAccountCreatedEvent>();
}