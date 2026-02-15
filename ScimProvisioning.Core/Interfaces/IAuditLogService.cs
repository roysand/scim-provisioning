using ScimProvisioning.Core.Entities;

namespace ScimProvisioning.Core.Interfaces;

/// <summary>
/// Service for managing audit logs
/// </summary>
public interface IAuditLogService
{
    /// <summary>
    /// Creates an audit log entry
    /// </summary>
    Task<AuditLog> CreateAuditLogAsync(
        string action,
        string entityType,
        Guid entityId,
        string? userId = null,
        string? changeDetails = null,
        Guid? outboxMessageId = null,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Updates audit log status to success
    /// </summary>
    Task MarkAsSuccessAsync(Guid auditLogId, CancellationToken cancellationToken = default);

    /// <summary>
    /// Updates audit log status to failed
    /// </summary>
    Task MarkAsFailedAsync(Guid auditLogId, string errorMessage, CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets audit logs by outbox message ID
    /// </summary>
    Task<IEnumerable<AuditLog>> GetByOutboxMessageIdAsync(Guid outboxMessageId, CancellationToken cancellationToken = default);
}
