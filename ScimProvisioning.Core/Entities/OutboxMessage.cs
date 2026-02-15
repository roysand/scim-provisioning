namespace ScimProvisioning.Core.Entities;

/// <summary>
/// Represents an outbox message for reliable event publishing
/// </summary>
public class OutboxMessage
{
    /// <summary>
    /// Gets the unique identifier for this outbox message
    /// </summary>
    public Guid Id { get; private set; }

    /// <summary>
    /// Gets the aggregate identifier that generated the event
    /// </summary>
    public Guid AggregateId { get; private set; }

    /// <summary>
    /// Gets the type of event
    /// </summary>
    public string EventType { get; private set; }

    /// <summary>
    /// Gets the serialized event content
    /// </summary>
    public string Content { get; private set; }

    /// <summary>
    /// Gets the date and time when the message was created
    /// </summary>
    public DateTime CreatedAt { get; private set; }

    /// <summary>
    /// Gets a value indicating whether the message has been processed
    /// </summary>
    public bool Processed { get; private set; }

    /// <summary>
    /// Gets the date and time when the message was processed
    /// </summary>
    public DateTime? ProcessedAt { get; private set; }

    /// <summary>
    /// Gets the correlation identifier for tracing
    /// </summary>
    public string CorrelationId { get; private set; }

    // EF Core constructor
    private OutboxMessage() { }

    /// <summary>
    /// Creates a new outbox message
    /// </summary>
    public OutboxMessage(Guid aggregateId, string eventType, string content, string? correlationId = null)
    {
        if (aggregateId == Guid.Empty)
            throw new ArgumentException("Aggregate ID cannot be empty", nameof(aggregateId));

        if (string.IsNullOrWhiteSpace(eventType))
            throw new ArgumentException("Event type is required", nameof(eventType));

        if (string.IsNullOrWhiteSpace(content))
            throw new ArgumentException("Content is required", nameof(content));

        Id = Guid.NewGuid();
        AggregateId = aggregateId;
        EventType = eventType;
        Content = content;
        CreatedAt = DateTime.UtcNow;
        Processed = false;
        CorrelationId = correlationId ?? Guid.NewGuid().ToString();
    }

    /// <summary>
    /// Marks the message as processed
    /// </summary>
    public void MarkAsProcessed()
    {
        Processed = true;
        ProcessedAt = DateTime.UtcNow;
    }
}
