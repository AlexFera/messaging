using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Messaging.Domain.Entities;

namespace Messaging.Infrastructure.Persistence.Configurations;

public class MessageThreadConfiguration : IEntityTypeConfiguration<MessageThread>
{
    public void Configure(EntityTypeBuilder<MessageThread> builder)
    {
        ArgumentNullException.ThrowIfNull(builder);

        builder.Ignore(e => e.DomainEvents);
    }
}
