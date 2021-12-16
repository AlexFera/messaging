using Messaging.Domain.Common;

namespace Messaging.Application.Common.Interfaces;

public interface IDomainEventService
{
    Task Publish(DomainEvent domainEvent);
}
