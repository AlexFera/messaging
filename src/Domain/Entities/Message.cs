namespace Messaging.Domain.Entities;

public class Message : AuditableEntity, IHasDomainEvent
{
    public int Id { get; set; }

    public string ToAddress { get; set; } = null!;

    public string FromAddress { get; set; } = null!;

    public string Content { get; set; } = null!;

    public int SendingUserId { get; set; }

    public User SendingUser { get; set; } = null!;

    public int MessageThreadId { get; set; }

    public MessageThread MessageThread { get; set; } = null!;

    public List<DomainEvent> DomainEvents { get; set; } = new List<DomainEvent>();
}
