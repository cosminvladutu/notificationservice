using Microsoft.Extensions.Logging;
using NotificationService.Application.Dtos;
using NotificationService.Domain.Enums;
using NotificationService.Domain.Interfaces;
using NotificationService.Domain.Models;

namespace NotificationService.Application.Services;

public sealed class IngestNotificationService(
    INotificationRepository repository,
    ILogger<IngestNotificationService> logger) : IIngestNotificationService
{
    public async Task IngestAsync(IncomingNotificationDto dto, CancellationToken ct)
    {
        logger.LogInformation("Ingesting notification {Id} from {App}", dto.NotificationId, dto.App);

        // Check if already exists
        var existing = await repository.GetRequestByIdAsync(dto.NotificationId, ct);
        if (existing is not null)
        {
            logger.LogWarning("Notification {Id} already exists, skipping", dto.NotificationId);
            return;
        }

        var notifications = dto.Notifications.Select(notification =>
            Notification.Create(
                channel: Enum.Parse<ChannelType>(notification.Channel, ignoreCase: true),
                notificationType: notification.NotificationType,
                notificationVersion: notification.NotificationVersion,
                sendAt: notification.SendAt,
                recipients: notification.Recipient))
            .ToList();

        var request = NotificationRequest.Create(
            dto.NotificationId,
            dto.App,
            dto.ResponsibleTeam,
            notifications,
            dto.TemplateData);

        await repository.SaveRequestAsync(request, ct);

        logger.LogInformation("Notification {Id} ingested with status {Status}", request.NotificationId, request.Status);
    }
}
