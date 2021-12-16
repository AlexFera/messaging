namespace Messaging.Domain.Events;

public class MessageCreatedEvent : DomainEvent
{
    public MessageCreatedEvent(Message item)
    {
        Item = item;
    }

    public Message Item { get; }
}