using ScimProvisioning.Core.Common;

namespace ScimProvisioning.Core.Events;

/// <summary>
/// Base class for user domain events
/// </summary>
public abstract class UserDomainEvent : IDomainEvent
{
    public Guid EventId { get; }
    public DateTime OccurredAt { get; }
    public Guid UserId { get; }
    public string ExternalId { get; }
    public string UserName { get; }

    protected UserDomainEvent(Guid userId, string externalId, string userName)
    {
        EventId = Guid.NewGuid();
        OccurredAt = DateTime.UtcNow;
        UserId = userId;
        ExternalId = externalId;
        UserName = userName;
    }
}

/// <summary>
/// Raised when a user is provisioned
/// </summary>
public class UserProvisionedDomainEvent : UserDomainEvent
{
    public UserProvisionedDomainEvent(Guid userId, string externalId, string userName)
        : base(userId, externalId, userName)
    {
    }
}

/// <summary>
/// Raised when a user is updated
/// </summary>
public class UserUpdatedDomainEvent : UserDomainEvent
{
    public UserUpdatedDomainEvent(Guid userId, string externalId, string userName)
        : base(userId, externalId, userName)
    {
    }
}

/// <summary>
/// Raised when a user is deleted
/// </summary>
public class UserDeletedDomainEvent : UserDomainEvent
{
    public UserDeletedDomainEvent(Guid userId, string externalId, string userName)
        : base(userId, externalId, userName)
    {
    }
}
