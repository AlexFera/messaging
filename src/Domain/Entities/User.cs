namespace Messaging.Domain.Entities;

public class User : AuditableEntity, IHasDomainEvent
{
    public int Id { get; set; }

    public string? EmailAddress { get; set; }

    public string? PhoneNumber { get; set; }

    public ICollection<Message> Messages { get; set; } = new List<Message>();

    public List<DomainEvent> DomainEvents { get; set; } = new List<DomainEvent>();
}
