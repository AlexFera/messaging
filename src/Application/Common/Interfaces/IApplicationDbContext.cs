using Messaging.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace Messaging.Application.Common.Interfaces;

public interface IApplicationDbContext
{
    DbSet<Message> Messages { get; }

    DbSet<MessageThread> MessageThreads { get; }

    DbSet<MessageThreadParticipant> MessageThreadParticipants { get; }

    DbSet<User> Users { get; }

    Task<int> SaveChangesAsync(CancellationToken cancellationToken);
}
