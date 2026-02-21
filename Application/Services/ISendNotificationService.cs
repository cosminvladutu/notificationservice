using NotificationService.Domain.Enums;
using NotificationService.Domain.Models;

namespace NotificationService.Application.Services;

public interface ISendNotificationService
{
    Task<IReadOnlyList<NotificationItem>> GetReadyItemsAsync(
        ChannelType channel,
        int batchSize,
        CancellationToken ct);

    string RenderContent(NotificationItem item);

    Task SendSmsAsync(NotificationItem item, CancellationToken ct);
    Task SendEmailAsync(NotificationItem item, CancellationToken ct);
}
