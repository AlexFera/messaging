namespace Messaging.Domain.Entities;

public class MessageThread : AuditableEntity, IHasDomainEvent
{
    public int Id { get; set; }

    public List<DomainEvent> DomainEvents { get; set; } = new List<DomainEvent>();
}

