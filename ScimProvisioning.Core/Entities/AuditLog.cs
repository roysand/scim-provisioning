namespace ScimProvisioning.Core.Entities;

/// <summary>
/// Represents an audit log entry for provisioning operations
/// </summary>
public class AuditLog
{
    /// <summary>
    /// Gets the unique identifier for this audit log entry
    /// </summary>
    public Guid Id { get; private set; }

    /// <summary>
    /// Gets the outbox message identifier (for correlation)
    /// </summary>
    public Guid? OutboxMessageId { get; private set; }

    /// <summary>
    /// Gets the action performed (Created, Updated, Deleted, etc.)
    /// </summary>
    public string Action { get; private set; }

    /// <summary>
    /// Gets the entity type (User, Group, etc.)
    /// </summary>
    public string EntityType { get; private set; }

    /// <summary>
    /// Gets the entity identifier
    /// </summary>
    public Guid EntityId { get; private set; }

    /// <summary>
    /// Gets the user identifier who performed the action
    /// </summary>
    public string? UserId { get; private set; }

    /// <summary>
    /// Gets the change details (JSON format)
    /// </summary>
    public string? ChangeDetails { get; private set; }

    /// <summary>
    /// Gets the timestamp of the action
    /// </summary>
    public DateTime Timestamp { get; private set; }

    /// <summary>
    /// Gets the status of the operation (Pending, Success, Failed)
    /// </summary>
    public AuditStatus Status { get; private set; }

    /// <summary>
    /// Gets the error message if the operation failed
    /// </summary>
    public string? ErrorMessage { get; private set; }

    // EF Core constructor
    private AuditLog() { }

    /// <summary>
    /// Creates a new audit log entry
    /// </summary>
    public AuditLog(
        string action,
        string entityType,
        Guid entityId,
        string? userId = null,
        string? changeDetails = null,
        Guid? outboxMessageId = null)
    {
        if (string.IsNullOrWhiteSpace(action))
            throw new ArgumentException("Action is required", nameof(action));

        if (string.IsNullOrWhiteSpace(entityType))
            throw new ArgumentException("Entity type is required", nameof(entityType));

        if (entityId == Guid.Empty)
            throw new ArgumentException("Entity ID cannot be empty", nameof(entityId));

        Id = Guid.NewGuid();
        Action = action;
        EntityType = entityType;
        EntityId = entityId;
        UserId = userId;
        ChangeDetails = changeDetails;
        OutboxMessageId = outboxMessageId;
        Timestamp = DateTime.UtcNow;
        Status = AuditStatus.Pending;
    }

    /// <summary>
    /// Marks the audit log entry as successful
    /// </summary>
    public void MarkAsSuccess()
    {
        Status = AuditStatus.Success;
    }

    /// <summary>
    /// Marks the audit log entry as failed
    /// </summary>
    public void MarkAsFailed(string errorMessage)
    {
        Status = AuditStatus.Failed;
        ErrorMessage = errorMessage;
    }
}

/// <summary>
/// Represents the status of an audit log entry
/// </summary>
public enum AuditStatus
{
    Pending = 0,
    Success = 1,
    Failed = 2
}
