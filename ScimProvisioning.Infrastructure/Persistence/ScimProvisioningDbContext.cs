using Microsoft.EntityFrameworkCore;
using ScimProvisioning.Core.Entities;
using ScimProvisioning.Infrastructure.Persistence.Configurations;

namespace ScimProvisioning.Infrastructure.Persistence;

/// <summary>
/// Database context for SCIM provisioning
/// </summary>
public class ScimProvisioningDbContext : DbContext
{
    public ScimProvisioningDbContext(DbContextOptions<ScimProvisioningDbContext> options)
        : base(options)
    {
    }

    public DbSet<ScimUser> Users => Set<ScimUser>();
    public DbSet<ScimGroup> Groups => Set<ScimGroup>();
    public DbSet<GroupMember> GroupMembers => Set<GroupMember>();
    public DbSet<OutboxMessage> OutboxMessages => Set<OutboxMessage>();
    public DbSet<AuditLog> AuditLogs => Set<AuditLog>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.ApplyConfiguration(new ScimUserConfiguration());
        modelBuilder.ApplyConfiguration(new ScimGroupConfiguration());
        modelBuilder.ApplyConfiguration(new GroupMemberConfiguration());
        modelBuilder.ApplyConfiguration(new OutboxMessageConfiguration());
        modelBuilder.ApplyConfiguration(new AuditLogConfiguration());
    }
}
