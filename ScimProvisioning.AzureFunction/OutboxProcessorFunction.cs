using Microsoft.Azure.Functions.Worker;
using Microsoft.Extensions.Logging;
using ScimProvisioning.Application.Services;
using ScimProvisioning.Core.Interfaces;
using System.Text.Json;

namespace ScimProvisioning.AzureFunction;

/// <summary>
/// Azure Function for processing SCIM outbox messages from Service Bus
/// </summary>
public class OutboxProcessorFunction
{
    private readonly IOutboxService _outboxService;
    private readonly IAuditLogService _auditLogService;
    private readonly ILogger<OutboxProcessorFunction> _logger;

    public OutboxProcessorFunction(
        IOutboxService outboxService,
        IAuditLogService auditLogService,
        ILogger<OutboxProcessorFunction> logger)
    {
        _outboxService = outboxService;
        _auditLogService = auditLogService;
        _logger = logger;
    }

    [Function("OutboxProcessor")]
    public async Task Run(
        [ServiceBusTrigger("scim-events", Connection = "ServiceBusConnection")]
        string message,
        FunctionContext context)
    {
        var messageId = context.BindingContext.BindingData["MessageId"]?.ToString();
        var correlationId = context.BindingContext.BindingData["CorrelationId"]?.ToString();

        _logger.LogInformation(
            "Processing outbox message. MessageId: {MessageId}, CorrelationId: {CorrelationId}",
            messageId,
            correlationId);

        try
        {
            // Deserialize message
            var eventData = JsonSerializer.Deserialize<Dictionary<string, object>>(message);
            
            _logger.LogInformation("Outbox message processed successfully. MessageId: {MessageId}", messageId);

            // Mark message as processed if messageId is a valid Guid
            if (Guid.TryParse(messageId, out var messageGuid))
            {
                await _outboxService.MarkAsProcessedAsync(messageGuid);
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(
                ex,
                "Failed to process outbox message. MessageId: {MessageId}, CorrelationId: {CorrelationId}",
                messageId,
                correlationId);
            
            throw; // Re-throw to trigger retry or dead-letter
        }
    }
}
