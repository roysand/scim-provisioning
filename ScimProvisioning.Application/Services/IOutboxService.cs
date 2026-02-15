using ScimProvisioning.Core.Common;
using ScimProvisioning.Core.Entities;

namespace ScimProvisioning.Application.Services;

/// <summary>
/// Service for managing outbox messages
/// </summary>
public interface IOutboxService
{
    /// <summary>
    /// Publishes domain events to the outbox
    /// </summary>
    Task PublishDomainEventsAsync(
        IEnumerable<IDomainEvent> domainEvents,
        string? correlationId = null,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets unprocessed outbox messages
    /// </summary>
    Task<IEnumerable<OutboxMessage>> GetUnprocessedMessagesAsync(
        int batchSize = 100,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Marks an outbox message as processed
    /// </summary>
    Task MarkAsProcessedAsync(Guid messageId, CancellationToken cancellationToken = default);
}
