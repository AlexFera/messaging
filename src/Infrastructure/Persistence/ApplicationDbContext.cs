using System.Reflection;
using Microsoft.EntityFrameworkCore;
using Messaging.Application.Common.Interfaces;
using Messaging.Domain.Common;
using Messaging.Domain.Entities;

namespace Messaging.Infrastructure.Persistence;

public class ApplicationDbContext : DbContext, IApplicationDbContext
{
    private readonly ICurrentUserService _currentUserService;
    private readonly IDateTimeOffset _dateTimeOffset;
    private readonly IDomainEventService _domainEventService;

    public ApplicationDbContext(
        DbContextOptions<ApplicationDbContext> options,
        ICurrentUserService currentUserService,
        IDomainEventService domainEventService,
        IDateTimeOffset dateTimeOffset) : base(options)
    {
        _currentUserService = currentUserService;
        _domainEventService = domainEventService;
        _dateTimeOffset = dateTimeOffset;
    }

    public DbSet<Message> Messages => Set<Message>();

    public DbSet<MessageThread> MessageThreads => Set<MessageThread>();

    public DbSet<MessageThreadParticipant> MessageThreadParticipants => Set<MessageThreadParticipant>();

    public DbSet<User> Users => Set<User>();

    public override async Task<int> SaveChangesAsync(CancellationToken cancellationToken = new CancellationToken())
    {
        foreach (var entry in ChangeTracker.Entries<AuditableEntity>())
        {
            switch (entry.State)
            {
                case EntityState.Added:
                    entry.Entity.CreatedBy = _currentUserService.UserId!;
                    entry.Entity.Created = _dateTimeOffset.UtcNow;
                    entry.Entity.LastModifiedBy = _currentUserService.UserId!;
                    entry.Entity.LastModified = _dateTimeOffset.UtcNow;
                    break;

                case EntityState.Modified:
                    entry.Entity.LastModifiedBy = _currentUserService.UserId!;
                    entry.Entity.LastModified = _dateTimeOffset.UtcNow;
                    break;
            }
        }

        var events = ChangeTracker.Entries<IHasDomainEvent>()
                .Select(x => x.Entity.DomainEvents)
                .SelectMany(x => x)
                .Where(domainEvent => !domainEvent.IsPublished)
                .ToArray();

        var result = await base.SaveChangesAsync(cancellationToken);

        await DispatchEventsAsync(events);

        return result;
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        ArgumentNullException.ThrowIfNull(modelBuilder);

        modelBuilder.ApplyConfigurationsFromAssembly(Assembly.GetExecutingAssembly());

        base.OnModelCreating(modelBuilder);
    }

    private async Task DispatchEventsAsync(DomainEvent[] events)
    {
        foreach (var @event in events)
        {
            @event.IsPublished = true;
            await _domainEventService.Publish(@event);
        }
    }
}
