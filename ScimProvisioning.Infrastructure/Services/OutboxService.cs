using Microsoft.EntityFrameworkCore;
using ScimProvisioning.Application.Services;
using ScimProvisioning.Core.Common;
using ScimProvisioning.Core.Entities;
using ScimProvisioning.Infrastructure.Persistence;
using System.Text.Json;

namespace ScimProvisioning.Infrastructure.Services;

/// <summary>
/// Service implementation for managing outbox messages
/// </summary>
public class OutboxService : IOutboxService
{
    private readonly ScimProvisioningDbContext _context;

    public OutboxService(ScimProvisioningDbContext context)
    {
        _context = context;
    }

    public async Task PublishDomainEventsAsync(
        IEnumerable<IDomainEvent> domainEvents,
        string? correlationId = null,
        CancellationToken cancellationToken = default)
    {
        foreach (var domainEvent in domainEvents)
        {
            var eventType = domainEvent.GetType().Name;
            var content = JsonSerializer.Serialize(domainEvent);
            
            var outboxMessage = new OutboxMessage(
                GetAggregateId(domainEvent),
                eventType,
                content,
                correlationId);

            await _context.OutboxMessages.AddAsync(outboxMessage, cancellationToken);
        }
    }

    public async Task<IEnumerable<OutboxMessage>> GetUnprocessedMessagesAsync(
        int batchSize = 100,
        CancellationToken cancellationToken = default)
    {
        return await _context.OutboxMessages
            .Where(m => !m.Processed)
            .OrderBy(m => m.CreatedAt)
            .Take(batchSize)
            .ToListAsync(cancellationToken);
    }

    public async Task MarkAsProcessedAsync(Guid messageId, CancellationToken cancellationToken = default)
    {
        var message = await _context.OutboxMessages.FindAsync(new object[] { messageId }, cancellationToken);
        if (message != null)
        {
            message.MarkAsProcessed();
            await _context.SaveChangesAsync(cancellationToken);
        }
    }

    private Guid GetAggregateId(IDomainEvent domainEvent)
    {
        // Extract aggregate ID from event based on type
        var eventType = domainEvent.GetType();
        
        var userIdProperty = eventType.GetProperty("UserId");
        if (userIdProperty != null)
            return (Guid)userIdProperty.GetValue(domainEvent)!;

        var groupIdProperty = eventType.GetProperty("GroupId");
        if (groupIdProperty != null)
            return (Guid)groupIdProperty.GetValue(domainEvent)!;

        throw new InvalidOperationException($"Could not extract aggregate ID from event type {eventType.Name}");
    }
}
