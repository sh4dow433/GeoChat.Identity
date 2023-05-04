namespace GeoChat.Identity.Api.EventBus.Events;

public class NewAccountCreatedEvent : BaseEvent
{
    public string UserId { get; set; } = null!;
    public string UserName { get; set; } = null!;

}
