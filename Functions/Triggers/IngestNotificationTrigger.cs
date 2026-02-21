using System.Text.Json;
using Azure.Messaging.ServiceBus;
using Microsoft.Azure.Functions.Worker;
using NotificationService.Application.Dtos;
using NotificationService.Application.Services;
using NotificationService.Serialization;

namespace NotificationService.Functions.Triggers;

public sealed class IngestNotificationTrigger(IIngestNotificationService ingestService)
{
    [Function(nameof(IngestNotificationTrigger))]
    public async Task Run(
        [ServiceBusTrigger("notificationqueue", Connection = "ServiceBusConnection")]
        ServiceBusReceivedMessage message,
        ServiceBusMessageActions messageActions,
        CancellationToken ct)
    {
        var dto = JsonSerializer.Deserialize<IncomingNotificationDto>(
            message.Body.ToString(),
            JsonDefaults.Options)!;

        await ingestService.IngestAsync(dto, ct);
        await messageActions.CompleteMessageAsync(message, ct);
    }
}
