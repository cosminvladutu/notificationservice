using NotificationService.Domain.Enums;
using NotificationService.Domain.Exceptions;

namespace NotificationService.Domain.Models;

public sealed class NotificationRequest
{
    private readonly List<Notification> _notifications = [];

    public Guid NotificationId { get; }
    public string App { get; }
    public string ResponsibleTeam { get; }
    public IReadOnlyList<Notification> Notifications => _notifications.AsReadOnly();
    public IReadOnlyDictionary<string, string> TemplateData { get; }
    public NotificationStatus Status { get; private set; }
    public string? Error { get; private set; }
    public int RetryCount { get; private set; }

    private NotificationRequest(
        Guid notificationId,
        string app,
        string responsibleTeam,
        IReadOnlyDictionary<string, string> templateData)
    {
        NotificationId = notificationId;
        App = app;
        ResponsibleTeam = responsibleTeam;
        TemplateData = templateData;
        Status = NotificationStatus.Started;
    }

    public static NotificationRequest Create(
        Guid notificationId,
        string app,
        string responsibleTeam,
        IEnumerable<Notification> notifications,
        IDictionary<string, string> templateData)
    {
        if (notificationId == Guid.Empty)
        {
            throw new DomainValidationException("NotificationId is required.");
        }

        ArgumentException.ThrowIfNullOrWhiteSpace(app);
        ArgumentException.ThrowIfNullOrWhiteSpace(responsibleTeam);
        ArgumentNullException.ThrowIfNull(templateData);

        var request = new NotificationRequest(
            notificationId,
            app,
            responsibleTeam,
            new Dictionary<string, string>(templateData));

        foreach (var notification in notifications)
        {
            request._notifications.Add(notification);
        }

        if (request._notifications.Count == 0)
        {
            throw new DomainValidationException("At least one notification is required.");
        }

        return request;
    }

    internal static NotificationRequest Reconstitute(
        Guid notificationId,
        string app,
        string responsibleTeam,
        IEnumerable<Notification> notifications,
        IReadOnlyDictionary<string, string> templateData,
        NotificationStatus status,
        string? error,
        int retryCount)
    {
        var request = new NotificationRequest(notificationId, app, responsibleTeam, templateData);
        request._notifications.AddRange(notifications);
        request.Status = status;
        request.Error = error;
        request.RetryCount = retryCount;
        return request;
    }

    public void MarkInProgress()
    {
        if (Status is not NotificationStatus.Started)
        {
            throw new DomainValidationException($"Cannot transition from {Status} to InProgress.");
        }

        Status = NotificationStatus.InProgress;
        Error = null;
    }

    public void MarkDone()
    {
        if (Status is not NotificationStatus.InProgress)
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

    public IReadOnlyList<NotificationItem> FanOutItems()
    {
        if (Status is not NotificationStatus.InProgress)
        {
            throw new DomainValidationException("Request must be InProgress before fan-out.");
        }

        return Notifications
            .SelectMany(notification =>
                notification.Recipients.Select(recipient =>
                    NotificationItem.Create(
                        notificationRequestId: NotificationId,
                        channel: notification.Channel,
                        notificationType: notification.NotificationType,
                        notificationVersion: notification.NotificationVersion,
                        recipient: recipient,
                        templateData: TemplateData,
                        sendAt: notification.SendAt,
                        app: App,
                        responsibleTeam: ResponsibleTeam)))
            .ToList()
            .AsReadOnly();
    }
}
