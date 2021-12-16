using Microsoft.EntityFrameworkCore;
using WebUI.Client.Services;

namespace WebUI.Client.Data;

internal class ClientSideDbContext : DbContext
{
    public DbSet<SentMessageBatchDto> SentMessages { get; set; } = default!;

    public ClientSideDbContext(DbContextOptions<ClientSideDbContext> options)
        : base(options)
    {
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.Entity<SentMessageBatchDto>().HasIndex(x => x.Id);
        modelBuilder.Entity<SentMessageBatchDto>().HasIndex(x => x.Created);
        modelBuilder.Entity<SentMessageBatchDto>().HasIndex(x => x.ToAddress);
        modelBuilder.Entity<SentMessageBatchDto>().HasIndex(x => x.FromAddress);
        modelBuilder.Entity<SentMessageBatchDto>().Property(x => x.ToAddress).UseCollation("nocase");
        modelBuilder.Entity<SentMessageBatchDto>().Property(x => x.FromAddress).UseCollation("nocase");
    }
}
