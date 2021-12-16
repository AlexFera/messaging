using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Messaging.Domain.Entities;

namespace Messaging.Infrastructure.Persistence.Configurations;

public class MessageThreadParticipantConfiguration : IEntityTypeConfiguration<MessageThreadParticipant>
{
    public void Configure(EntityTypeBuilder<MessageThreadParticipant> builder)
    {
        ArgumentNullException.ThrowIfNull(builder);

        builder.Ignore(e => e.DomainEvents);

        builder.HasKey(nameof(MessageThreadParticipant.MessageThreadId), nameof(MessageThreadParticipant.UserId));
    }
}
