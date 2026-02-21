using NotificationService.Domain.Enums;
using NotificationService.Domain.Exceptions;

namespace NotificationService.Domain.Models;

public sealed class Notification
{
    public ChannelType Channel { get; }
    public string NotificationType { get; }
    public string NotificationVersion { get; }
    public DateTimeOffset? SendAt { get; }
    public IReadOnlyList<string> Recipients { get; }

    private Notification(
        ChannelType channel,
        string notificationType,
        string notificationVersion,
        DateTimeOffset? sendAt,
        IReadOnlyList<string> recipients)
    {
        Channel = channel;
        NotificationType = notificationType;
        NotificationVersion = notificationVersion;
        SendAt = sendAt;
        Recipients = recipients;
    }

    public static Notification Create(
        ChannelType channel,
        string notificationType,
        string notificationVersion,
        DateTimeOffset? sendAt,
        IReadOnlyList<string> recipients)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(notificationType);
        ArgumentException.ThrowIfNullOrWhiteSpace(notificationVersion);

        if (recipients is null || recipients.Count == 0)
        {
            throw new DomainValidationException("At least one recipient is required.");
        }

        foreach (var recipient in recipients)
        {
            ArgumentException.ThrowIfNullOrWhiteSpace(recipient);
        }

        return new Notification(channel, notificationType, notificationVersion, sendAt, recipients);
    }
}
