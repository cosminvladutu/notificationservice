using Microsoft.Azure.Functions.Worker;
using NotificationService.Application.Services;

namespace NotificationService.Functions.Triggers;

public sealed class DispatchNotificationsTrigger(IDispatchNotificationService dispatchService)
{
    [Function(nameof(DispatchNotificationsTrigger))]
    public async Task Run(
        [TimerTrigger("0 0 8-22 * * *")] TimerInfo timer,
        CancellationToken ct)
    {
        await dispatchService.DispatchPendingAsync(batchSize: 100, ct);
    }
}
