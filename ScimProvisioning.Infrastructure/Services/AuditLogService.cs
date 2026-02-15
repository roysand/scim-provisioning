using Microsoft.EntityFrameworkCore;
using ScimProvisioning.Core.Entities;
using ScimProvisioning.Core.Interfaces;
using ScimProvisioning.Infrastructure.Persistence;

namespace ScimProvisioning.Infrastructure.Services;

/// <summary>
/// Service implementation for managing audit logs
/// </summary>
public class AuditLogService : IAuditLogService
{
    private readonly ScimProvisioningDbContext _context;

    public AuditLogService(ScimProvisioningDbContext context)
    {
        _context = context;
    }

    public async Task<AuditLog> CreateAuditLogAsync(
        string action,
        string entityType,
        Guid entityId,
        string? userId = null,
        string? changeDetails = null,
        Guid? outboxMessageId = null,
        CancellationToken cancellationToken = default)
    {
        var auditLog = new AuditLog(action, entityType, entityId, userId, changeDetails, outboxMessageId);
        await _context.AuditLogs.AddAsync(auditLog, cancellationToken);
        return auditLog;
    }

    public async Task MarkAsSuccessAsync(Guid auditLogId, CancellationToken cancellationToken = default)
    {
        var auditLog = await _context.AuditLogs.FindAsync(new object[] { auditLogId }, cancellationToken);
        if (auditLog != null)
        {
            auditLog.MarkAsSuccess();
            await _context.SaveChangesAsync(cancellationToken);
        }
    }

    public async Task MarkAsFailedAsync(Guid auditLogId, string errorMessage, CancellationToken cancellationToken = default)
    {
        var auditLog = await _context.AuditLogs.FindAsync(new object[] { auditLogId }, cancellationToken);
        if (auditLog != null)
        {
            auditLog.MarkAsFailed(errorMessage);
            await _context.SaveChangesAsync(cancellationToken);
        }
    }

    public async Task<IEnumerable<AuditLog>> GetByOutboxMessageIdAsync(Guid outboxMessageId, CancellationToken cancellationToken = default)
    {
        return await _context.AuditLogs
            .Where(a => a.OutboxMessageId == outboxMessageId)
            .ToListAsync(cancellationToken);
    }
}
