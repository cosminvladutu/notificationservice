using NotificationService.Domain.Enums;
using NotificationService.Domain.Exceptions;

namespace NotificationService.Domain.Models;

public sealed class NotificationItem
{
    public Guid Id { get; }
    public Guid NotificationRequestId { get; }
    public ChannelType Channel { get; }
    public string NotificationType { get; }
    public string NotificationVersion { get; }
    public string Recipient { get; }
    public IReadOnlyDictionary<string, string> TemplateData { get; }
    public DateTimeOffset? SendAt { get; }
    public string? RenderedContent { get; private set; }
    public NotificationStatus Status { get; private set; }
    public string? Error { get; private set; }
    public int RetryCount { get; private set; }
    public string App { get; }
    public string ResponsibleTeam { get; }

    private NotificationItem(
        Guid id,
        Guid notificationRequestId,
        ChannelType channel,
        string notificationType,
        string notificationVersion,
        string recipient,
        IReadOnlyDictionary<string, string> templateData,
        DateTimeOffset? sendAt,
        string app,
        string responsibleTeam)
    {
        Id = id;
        NotificationRequestId = notificationRequestId;
        Channel = channel;
        NotificationType = notificationType;
        NotificationVersion = notificationVersion;
        Recipient = recipient;
        TemplateData = templateData;
        SendAt = sendAt;
        App = app;
        ResponsibleTeam = responsibleTeam;
        Status = NotificationStatus.Pending;
    }

    public static NotificationItem Create(
        Guid notificationRequestId,
        ChannelType channel,
        string notificationType,
        string notificationVersion,
        string recipient,
        IReadOnlyDictionary<string, string> templateData,
        DateTimeOffset? sendAt,
        string app,
        string responsibleTeam)
    {
        if (notificationRequestId == Guid.Empty)
        {
            throw new DomainValidationException("NotificationRequestId is required.");
        }

        ArgumentException.ThrowIfNullOrWhiteSpace(recipient);
        ArgumentException.ThrowIfNullOrWhiteSpace(notificationType);
        ArgumentException.ThrowIfNullOrWhiteSpace(notificationVersion);

        return new NotificationItem(
            Guid.NewGuid(),
            notificationRequestId,
            channel,
            notificationType,
            notificationVersion,
            recipient,
            templateData,
            sendAt,
            app,
            responsibleTeam);
    }

    internal static NotificationItem Reconstitute(
        Guid id,
        Guid notificationRequestId,
        ChannelType channel,
        string notificationType,
        string notificationVersion,
        string recipient,
        IReadOnlyDictionary<string, string> templateData,
        DateTimeOffset? sendAt,
        string? renderedContent,
        NotificationStatus status,
        string? error,
        int retryCount,
        string app,
        string responsibleTeam)
    {
        var item = new NotificationItem(
            id,
            notificationRequestId,
            channel,
            notificationType,
            notificationVersion,
            recipient,
            templateData,
            sendAt,
            app,
            responsibleTeam);

        item.RenderedContent = renderedContent;
        item.Status = status;
        item.Error = error;
        item.RetryCount = retryCount;
        return item;
    }

    public void SetRenderedContent(string content)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(content);
        RenderedContent = content;
    }

    // Replace DateTimeOffset.UtcNow with injected TimeProvider for unit testing
    public bool IsReadyToSend(TimeProvider timeProvider)
        => Status is NotificationStatus.Pending
           && (SendAt is null || SendAt <= timeProvider.GetUtcNow());

    public void MarkDone()
    {
        if (Status is not NotificationStatus.Pending)
        {
            throw new DomainValidationException($"Cannot transition from {Status} to Done.");
        }

        Status = NotificationStatus.Done;
        Error = null;
    }

    public void MarkFailed(string error)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(error);
        Status = NotificationStatus.Failed;
        Error = error;
        RetryCount++;
    }

    public void ResetForRetry()
    {
        if (Status is not NotificationStatus.Failed)
        {
            throw new DomainValidationException("Only failed items can be retried.");
        }

        Status = NotificationStatus.Pending;
        Error = null;
    }
}
