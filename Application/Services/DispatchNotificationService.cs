using Microsoft.Extensions.Logging;
using NotificationService.Domain.Enums;
using NotificationService.Domain.Interfaces;

namespace NotificationService.Application.Services;

public sealed class DispatchNotificationService(
    INotificationRepository repository,
    ILogger<DispatchNotificationService> logger) : IDispatchNotificationService
{
    public async Task DispatchPendingAsync(int batchSize, CancellationToken ct)
    {
        var requests = await repository.GetRequestsByStatusAsync(
            NotificationStatus.Started,
            batchSize,
            ct);

        logger.LogInformation("Dispatching {Count} notification requests", requests.Count);

        foreach (var request in requests)
        {
            try
            {
                request.MarkInProgress();
                await repository.UpdateRequestAsync(request, ct);

                var items = request.FanOutItems();
                await repository.SaveItemsAsync(items, ct);

                request.MarkDone();
                await repository.UpdateRequestAsync(request, ct);

                logger.LogInformation(
                    "Request {Id} dispatched into {Count} items",
                    request.NotificationId,
                    items.Count);
            }
            catch (Exception ex)
            {
                request.MarkFailed(ex.Message);
                await repository.UpdateRequestAsync(request, ct);

                logger.LogError(ex, "Failed to dispatch request {Id}", request.NotificationId);
            }
        }
    }
}
