namespace Messaging.Domain.Entities;

public class MessageThreadParticipant : AuditableEntity, IHasDomainEvent
{
    public int MessageThreadId { get; set; }

    public int UserId { get; set; }

    public  MessageThread MessageThread { get; set; } = null!;

    public User User { get; set; } = null!;

    public List<DomainEvent> DomainEvents { get; set; } = new List<DomainEvent>();
}

