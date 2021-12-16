namespace Messaging.Domain.Common;

public abstract class AuditableEntity
{
    public DateTimeOffset Created { get; set; }

    public string CreatedBy { get; set; } = null!;

    public DateTimeOffset LastModified { get; set; }

    public string LastModifiedBy { get; set; } = null!;
}
