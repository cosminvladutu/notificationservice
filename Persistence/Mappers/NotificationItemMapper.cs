using NotificationService.Domain.Enums;
using NotificationService.Domain.Models;
using NotificationService.Persistence.DbModels;

namespace NotificationService.Persistence.Mappers;

public static class NotificationItemMapper
{
    public static NotificationItemEntity ToEntity(NotificationItem item, DateTimeOffset utcNow)
        => new()
        {
            id = item.Id.ToString(),
            partitionKey = item.Channel.ToString().ToLowerInvariant(),
            notificationRequestId = item.NotificationRequestId,
            channel = item.Channel.ToString(),
            notificationType = item.NotificationType,
            notificationVersion = item.NotificationVersion,
            recipient = item.Recipient,
            templateData = item.TemplateData.ToDictionary(entry => entry.Key, entry => entry.Value),
            sendAt = item.SendAt,
            renderedContent = item.RenderedContent,
            status = item.Status.ToString(),
            error = item.Error,
            retryCount = item.RetryCount,
            app = item.App,
            responsibleTeam = item.ResponsibleTeam,
            createdAt = utcNow,
            updatedAt = utcNow
        };

    public static NotificationItemEntity ToEntity(NotificationItem item, DateTimeOffset utcNow, DateTimeOffset createdAt)
        => new()
        {
            id = item.Id.ToString(),
            partitionKey = item.Channel.ToString().ToLowerInvariant(),
            notificationRequestId = item.NotificationRequestId,
            channel = item.Channel.ToString(),
            notificationType = item.NotificationType,
            notificationVersion = item.NotificationVersion,
            recipient = item.Recipient,
            templateData = item.TemplateData.ToDictionary(entry => entry.Key, entry => entry.Value),
            sendAt = item.SendAt,
            renderedContent = item.RenderedContent,
            status = item.Status.ToString(),
            error = item.Error,
            retryCount = item.RetryCount,
            app = item.App,
            responsibleTeam = item.ResponsibleTeam,
            createdAt = createdAt,
            updatedAt = utcNow
        };

    public static NotificationItem ToDomain(NotificationItemEntity entity)
        => NotificationItem.Reconstitute(
            id: Guid.Parse(entity.id),
            notificationRequestId: entity.notificationRequestId,
            channel: Enum.Parse<ChannelType>(entity.channel, ignoreCase: true),
            notificationType: entity.notificationType,
            notificationVersion: entity.notificationVersion,
            recipient: entity.recipient,
            templateData: entity.templateData,
            sendAt: entity.sendAt,
            renderedContent: entity.renderedContent,
            status: Enum.Parse<NotificationStatus>(entity.status, ignoreCase: true),
            error: entity.error,
            retryCount: entity.retryCount,
            app: entity.app,
            responsibleTeam: entity.responsibleTeam);
}
