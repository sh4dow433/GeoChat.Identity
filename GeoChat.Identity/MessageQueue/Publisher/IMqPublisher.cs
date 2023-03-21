using GeoChat.Identity.Api.MessageQueue.Events;

namespace GeoChat.Identity.Api.MessageQueue.Publisher;

public interface IMqPublisher
{
    void PublishEvent(NewAccountCreatedEvent accountCreatedEvent);
}
