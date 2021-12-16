using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Messaging.Domain.Entities;

namespace Messaging.Infrastructure.Persistence.Configurations;

public class MessageConfiguration : IEntityTypeConfiguration<Message>
{
    public void Configure(EntityTypeBuilder<Message> builder)
    {
        ArgumentNullException.ThrowIfNull(builder);

        builder.Ignore(e => e.DomainEvents);
    }
}
