using Messaging.Application.Common.Interfaces;
using Messaging.Application.Common.Models;
using Messaging.Domain.Common;
using MediatR;
using Microsoft.Extensions.Logging;

namespace Messaging.Infrastructure.Services;

public class DomainEventService : IDomainEventService
{
    private readonly ILogger<DomainEventService> _logger;
    private readonly IPublisher _mediator;

    public DomainEventService(ILogger<DomainEventService> logger, IPublisher mediator)
    {
        _logger = logger;
        _mediator = mediator;
    }

    public async Task Publish(DomainEvent domainEvent)
    {
        ArgumentNullException.ThrowIfNull(domainEvent);

        _logger.LogInformation("Publishing domain event. Event - {Event}", domainEvent.GetType().Name);
        await _mediator.Publish(GetNotificationCorrespondingToDomainEvent(domainEvent));
    }

    private static INotification GetNotificationCorrespondingToDomainEvent(DomainEvent domainEvent)
    {
        return (INotification)Activator.CreateInstance(
            typeof(DomainEventNotification<>).MakeGenericType(domainEvent.GetType()), domainEvent)!;
    }
}
