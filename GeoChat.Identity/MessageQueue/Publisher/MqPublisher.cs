using GeoChat.Identity.Api.MessageQueue.Events;

namespace GeoChat.Identity.Api.MessageQueue.Publisher;

public class MqPublisher : IMqPublisher
{
    public void PublishEvent(NewAccountCreatedEvent accountCreatedEvent)
    {
        throw new NotImplementedException();
    }
}