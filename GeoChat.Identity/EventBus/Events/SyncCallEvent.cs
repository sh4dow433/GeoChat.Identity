namespace GeoChat.Identity.Api.EventBus.Events;

public class SyncCallEvent : BaseEvent
{
    public DateTime LastSyncCall { get; set; }
}
