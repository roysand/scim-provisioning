using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using ScimProvisioning.Core.Entities;
using ScimProvisioning.Core.ValueObjects;

namespace ScimProvisioning.Infrastructure.Persistence.Configurations;

/// <summary>
/// Entity configuration for ScimUser
/// </summary>
public class ScimUserConfiguration : IEntityTypeConfiguration<ScimUser>
{
    public void Configure(EntityTypeBuilder<ScimUser> builder)
    {
        builder.ToTable("Users");

        builder.HasKey(u => u.Id);

        builder.Property(u => u.ExternalId)
            .IsRequired()
            .HasMaxLength(255);

        builder.Property(u => u.UserName)
            .IsRequired()
            .HasMaxLength(255);

        builder.Property(u => u.DisplayName)
            .IsRequired()
            .HasMaxLength(255);

        builder.Property(u => u.PrimaryEmail)
            .IsRequired()
            .HasMaxLength(254);

        builder.Property(u => u.Active)
            .IsRequired();

        builder.Property(u => u.CreatedAt)
            .IsRequired();

        builder.Property(u => u.ModifiedAt)
            .IsRequired();

        // Indexes
        builder.HasIndex(u => u.ExternalId).IsUnique();
        builder.HasIndex(u => u.UserName).IsUnique();
        builder.HasIndex(u => u.PrimaryEmail);

        // Ignore domain events and value objects for now (can be stored in separate tables if needed)
        builder.Ignore(u => u.DomainEvents);
        builder.Ignore(u => u.Emails);
        builder.Ignore(u => u.PhoneNumbers);
    }
}

/// <summary>
/// Entity configuration for ScimGroup
/// </summary>
public class ScimGroupConfiguration : IEntityTypeConfiguration<ScimGroup>
{
    public void Configure(EntityTypeBuilder<ScimGroup> builder)
    {
        builder.ToTable("Groups");

        builder.HasKey(g => g.Id);

        builder.Property(g => g.ExternalId)
            .IsRequired()
            .HasMaxLength(255);

        builder.Property(g => g.DisplayName)
            .IsRequired()
            .HasMaxLength(255);

        builder.Property(g => g.CreatedAt)
            .IsRequired();

        builder.Property(g => g.ModifiedAt)
            .IsRequired();

        // Indexes
        builder.HasIndex(g => g.ExternalId).IsUnique();

        // Ignore domain events
        builder.Ignore(g => g.DomainEvents);

        // Configure Members collection
        builder.HasMany(g => g.Members)
            .WithOne()
            .HasForeignKey(m => m.GroupId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}

/// <summary>
/// Entity configuration for GroupMember
/// </summary>
public class GroupMemberConfiguration : IEntityTypeConfiguration<GroupMember>
{
    public void Configure(EntityTypeBuilder<GroupMember> builder)
    {
        builder.ToTable("GroupMembers");

        builder.HasKey(m => new { m.GroupId, m.UserId });

        builder.Property(m => m.DisplayName)
            .IsRequired()
            .HasMaxLength(255);

        builder.Property(m => m.AddedAt)
            .IsRequired();

        // Indexes
        builder.HasIndex(m => m.UserId);
    }
}

/// <summary>
/// Entity configuration for OutboxMessage
/// </summary>
public class OutboxMessageConfiguration : IEntityTypeConfiguration<OutboxMessage>
{
    public void Configure(EntityTypeBuilder<OutboxMessage> builder)
    {
        builder.ToTable("OutboxMessages");

        builder.HasKey(m => m.Id);

        builder.Property(m => m.AggregateId)
            .IsRequired();

        builder.Property(m => m.EventType)
            .IsRequired()
            .HasMaxLength(255);

        builder.Property(m => m.Content)
            .IsRequired();

        builder.Property(m => m.CreatedAt)
            .IsRequired();

        builder.Property(m => m.Processed)
            .IsRequired();

        builder.Property(m => m.ProcessedAt);

        builder.Property(m => m.CorrelationId)
            .IsRequired()
            .HasMaxLength(255);

        // Indexes
        builder.HasIndex(m => m.Processed);
        builder.HasIndex(m => m.CreatedAt);
        builder.HasIndex(m => m.CorrelationId);
    }
}

/// <summary>
/// Entity configuration for AuditLog
/// </summary>
public class AuditLogConfiguration : IEntityTypeConfiguration<AuditLog>
{
    public void Configure(EntityTypeBuilder<AuditLog> builder)
    {
        builder.ToTable("AuditLogs");

        builder.HasKey(a => a.Id);

        builder.Property(a => a.OutboxMessageId);

        builder.Property(a => a.Action)
            .IsRequired()
            .HasMaxLength(100);

        builder.Property(a => a.EntityType)
            .IsRequired()
            .HasMaxLength(100);

        builder.Property(a => a.EntityId)
            .IsRequired();

        builder.Property(a => a.UserId)
            .HasMaxLength(255);

        builder.Property(a => a.ChangeDetails);

        builder.Property(a => a.Timestamp)
            .IsRequired();

        builder.Property(a => a.Status)
            .IsRequired();

        builder.Property(a => a.ErrorMessage);

        // Indexes
        builder.HasIndex(a => a.OutboxMessageId);
        builder.HasIndex(a => a.EntityId);
        builder.HasIndex(a => a.Timestamp);
        builder.HasIndex(a => a.Status);
    }
}
