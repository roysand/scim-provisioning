namespace ScimProvisioning.Core.Common;

/// <summary>
/// Marker interface for domain events
/// </summary>
public interface IDomainEvent
{
    /// <summary>
    /// Gets the unique identifier for this event
    /// </summary>
    Guid EventId { get; }

    /// <summary>
    /// Gets the date and time when the event occurred
    /// </summary>
    DateTime OccurredAt { get; }
}
