using NotificationService.Domain.Enums;
using NotificationService.Domain.Models;

namespace NotificationService.Domain.Interfaces;

public interface INotificationRepository
{
    Task SaveRequestAsync(NotificationRequest request, CancellationToken ct);
    Task<NotificationRequest?> GetRequestByIdAsync(Guid id, CancellationToken ct);
    Task<IReadOnlyList<NotificationRequest>> GetRequestsByStatusAsync(
        NotificationStatus status,
        int limit,
        CancellationToken ct);
    Task UpdateRequestAsync(NotificationRequest request, CancellationToken ct);

    Task SaveItemAsync(NotificationItem item, CancellationToken ct);
    Task SaveItemsAsync(IEnumerable<NotificationItem> items, CancellationToken ct);
    Task<IReadOnlyList<NotificationItem>> GetItemsByChannelAndStatusAsync(
        ChannelType channel,
        NotificationStatus status,
        int limit,
        CancellationToken ct);
    Task UpdateItemAsync(NotificationItem item, CancellationToken ct);
}
