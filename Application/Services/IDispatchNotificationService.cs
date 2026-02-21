namespace NotificationService.Application.Services;

public interface IDispatchNotificationService
{
    Task DispatchPendingAsync(int batchSize, CancellationToken ct);
}
