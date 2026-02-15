using ScimProvisioning.Core.Common;

namespace ScimProvisioning.Core.Events;

/// <summary>
/// Base class for group domain events
/// </summary>
public abstract class GroupDomainEvent : IDomainEvent
{
    public Guid EventId { get; }
    public DateTime OccurredAt { get; }
    public Guid GroupId { get; }
    public string ExternalId { get; }
    public string DisplayName { get; }

    protected GroupDomainEvent(Guid groupId, string externalId, string displayName)
    {
        EventId = Guid.NewGuid();
        OccurredAt = DateTime.UtcNow;
        GroupId = groupId;
        ExternalId = externalId;
        DisplayName = displayName;
    }
}

/// <summary>
/// Raised when a group is provisioned
/// </summary>
public class GroupProvisionedDomainEvent : GroupDomainEvent
{
    public GroupProvisionedDomainEvent(Guid groupId, string externalId, string displayName)
        : base(groupId, externalId, displayName)
    {
    }
}

/// <summary>
/// Raised when a group is updated
/// </summary>
public class GroupUpdatedDomainEvent : GroupDomainEvent
{
    public GroupUpdatedDomainEvent(Guid groupId, string externalId, string displayName)
        : base(groupId, externalId, displayName)
    {
    }
}

/// <summary>
/// Raised when a group is deleted
/// </summary>
public class GroupDeletedDomainEvent : GroupDomainEvent
{
    public GroupDeletedDomainEvent(Guid groupId, string externalId, string displayName)
        : base(groupId, externalId, displayName)
    {
    }
}
