namespace NotificationService.Persistence.DbModels;

public sealed class NotificationItemEntity
{
    public string id { get; set; } = default!;
    public string partitionKey { get; set; } = default!;
    public Guid notificationRequestId { get; set; }
    public string channel { get; set; } = default!;
    public string notificationType { get; set; } = default!;
    public string notificationVersion { get; set; } = default!;
    public string recipient { get; set; } = default!;
    public Dictionary<string, string> templateData { get; set; } = [];
    public DateTimeOffset? sendAt { get; set; }
    public string? renderedContent { get; set; }
    public string status { get; set; } = default!;
    public string? error { get; set; }
    public int retryCount { get; set; }
    public string app { get; set; } = default!;
    public string responsibleTeam { get; set; } = default!;
    public DateTimeOffset createdAt { get; set; }
    public DateTimeOffset updatedAt { get; set; }
}
