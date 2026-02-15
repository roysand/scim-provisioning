using Azure.Messaging.ServiceBus;
using Microsoft.Extensions.Options;
using ScimProvisioning.Core.Entities;

namespace ScimProvisioning.Infrastructure.Messaging;

/// <summary>
/// Configuration options for Azure Service Bus
/// </summary>
public class ServiceBusOptions
{
    public string ConnectionString { get; set; } = string.Empty;
    public string QueueName { get; set; } = "scim-events";
}

/// <summary>
/// Publisher for sending outbox messages to Azure Service Bus
/// </summary>
public class AzureServiceBusOutboxPublisher
{
    private readonly ServiceBusClient _serviceBusClient;
    private readonly ServiceBusSender _sender;
    private readonly ServiceBusOptions _options;

    public AzureServiceBusOutboxPublisher(IOptions<ServiceBusOptions> options)
    {
        _options = options.Value;
        _serviceBusClient = new ServiceBusClient(_options.ConnectionString);
        _sender = _serviceBusClient.CreateSender(_options.QueueName);
    }

    /// <summary>
    /// Publishes an outbox message to Service Bus
    /// </summary>
    public async Task PublishAsync(OutboxMessage message, CancellationToken cancellationToken = default)
    {
        var serviceBusMessage = new ServiceBusMessage(message.Content)
        {
            MessageId = message.Id.ToString(),
            CorrelationId = message.CorrelationId,
            Subject = message.EventType
        };

        serviceBusMessage.ApplicationProperties.Add("AggregateId", message.AggregateId.ToString());
        serviceBusMessage.ApplicationProperties.Add("EventType", message.EventType);
        serviceBusMessage.ApplicationProperties.Add("CreatedAt", message.CreatedAt);

        await _sender.SendMessageAsync(serviceBusMessage, cancellationToken);
    }

    /// <summary>
    /// Publishes multiple outbox messages in a batch
    /// </summary>
    public async Task PublishBatchAsync(IEnumerable<OutboxMessage> messages, CancellationToken cancellationToken = default)
    {
        using var messageBatch = await _sender.CreateMessageBatchAsync(cancellationToken);

        foreach (var message in messages)
        {
            var serviceBusMessage = new ServiceBusMessage(message.Content)
            {
                MessageId = message.Id.ToString(),
                CorrelationId = message.CorrelationId,
                Subject = message.EventType
            };

            serviceBusMessage.ApplicationProperties.Add("AggregateId", message.AggregateId.ToString());
            serviceBusMessage.ApplicationProperties.Add("EventType", message.EventType);
            serviceBusMessage.ApplicationProperties.Add("CreatedAt", message.CreatedAt);

            if (!messageBatch.TryAddMessage(serviceBusMessage))
            {
                // Send current batch and create new one
                await _sender.SendMessagesAsync(messageBatch, cancellationToken);
                messageBatch.Dispose();
                
                var newBatch = await _sender.CreateMessageBatchAsync(cancellationToken);
                newBatch.TryAddMessage(serviceBusMessage);
            }
        }

        if (messageBatch.Count > 0)
        {
            await _sender.SendMessagesAsync(messageBatch, cancellationToken);
        }
    }

    public async ValueTask DisposeAsync()
    {
        await _sender.DisposeAsync();
        await _serviceBusClient.DisposeAsync();
    }
}
