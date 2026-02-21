namespace NotificationService.Application.Dtos;

public sealed record IncomingNotificationDto(
    Guid NotificationId,
    string App,
    string ResponsibleTeam,
    List<IncomingNotificationEntryDto> Notifications,
    Dictionary<string, string> TemplateData);

public sealed record IncomingNotificationEntryDto(
    string Channel,
    string NotificationType,
    string NotificationVersion,
    DateTimeOffset? SendAt,
    List<string> Recipient);
