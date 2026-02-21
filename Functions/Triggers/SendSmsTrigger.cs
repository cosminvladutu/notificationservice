using Microsoft.Azure.Functions.Worker;
using Microsoft.DurableTask.Client;
using NotificationService.Application.Services;
using NotificationService.Domain.Enums;

namespace NotificationService.Functions.Triggers;

public sealed class SendSmsTrigger(
    ISendNotificationService sendService,
    DurableTaskClient durableClient)
{
    [Function(nameof(SendSmsTrigger))]
    public async Task Run(
        [TimerTrigger("0 0 8-22 * * *")] TimerInfo timer,
        CancellationToken ct)
    {
        var items = await sendService.GetReadyItemsAsync(ChannelType.Sms, batchSize: 100, ct);

        foreach (var item in items)
        {
            await durableClient.ScheduleNewOrchestrationInstanceAsync(
                nameof(Orchestrations.SendSmsOrchestration),
                item,
                ct);
        }
    }
}
