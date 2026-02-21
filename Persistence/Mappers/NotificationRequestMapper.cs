using NotificationService.Domain.Enums;
using NotificationService.Domain.Models;
using NotificationService.Persistence.DbModels;

namespace NotificationService.Persistence.Mappers;

public static class NotificationRequestMapper
{
    public static NotificationRequestEntity ToEntity(NotificationRequest request, DateTimeOffset utcNow)
        => new()
        {
            id = request.NotificationId.ToString(),
            partitionKey = request.App,
            app = request.App,
            responsibleTeam = request.ResponsibleTeam,
            status = request.Status.ToString(),
            error = request.Error,
            retryCount = request.RetryCount,
            templateData = request.TemplateData.ToDictionary(entry => entry.Key, entry => entry.Value),
            notifications = request.Notifications.Select(notification => new NotificationSubEntity
            {
                channel = notification.Channel.ToString(),
                notificationType = notification.NotificationType,
                notificationVersion = notification.NotificationVersion,
                sendAt = notification.SendAt,
                recipient = notification.Recipients.ToList()
            }).ToList(),
            createdAt = utcNow,
            updatedAt = utcNow
        };

    public static NotificationRequestEntity ToEntity(NotificationRequest request, DateTimeOffset utcNow, DateTimeOffset createdAt)
        => new()
        {
            id = request.NotificationId.ToString(),
            partitionKey = request.App,
            app = request.App,
            responsibleTeam = request.ResponsibleTeam,
            status = request.Status.ToString(),
            error = request.Error,
            retryCount = request.RetryCount,
            templateData = request.TemplateData.ToDictionary(entry => entry.Key, entry => entry.Value),
            notifications = request.Notifications.Select(notification => new NotificationSubEntity
            {
                channel = notification.Channel.ToString(),
                notificationType = notification.NotificationType,
                notificationVersion = notification.NotificationVersion,
                sendAt = notification.SendAt,
                recipient = notification.Recipients.ToList()
            }).ToList(),
            createdAt = createdAt,
            updatedAt = utcNow
        };

    public static NotificationRequest ToDomain(NotificationRequestEntity entity)
    {
        var notifications = entity.notifications.Select(notification => Notification.Create(
            channel: Enum.Parse<ChannelType>(notification.channel, ignoreCase: true),
            notificationType: notification.notificationType,
            notificationVersion: notification.notificationVersion,
            sendAt: notification.sendAt,
            recipients: notification.recipient)).ToList();

        return NotificationRequest.Reconstitute(
            notificationId: Guid.Parse(entity.id),
            app: entity.app,
            responsibleTeam: entity.responsibleTeam,
            notifications: notifications,
            templateData: entity.templateData,
            status: Enum.Parse<NotificationStatus>(entity.status, ignoreCase: true),
            error: entity.error,
            retryCount: entity.retryCount);
    }
}
