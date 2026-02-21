namespace NotificationService.Persistence.DbModels;

public sealed class NotificationRequestEntity
{
    public string id { get; set; } = default!;
    public string partitionKey { get; set; } = default!;
    public string app { get; set; } = default!;
    public string responsibleTeam { get; set; } = default!;
    public string status { get; set; } = default!;
    public string? error { get; set; }
    public int retryCount { get; set; }
    public List<NotificationSubEntity> notifications { get; set; } = [];
    public Dictionary<string, string> templateData { get; set; } = [];
    public DateTimeOffset createdAt { get; set; }
    public DateTimeOffset updatedAt { get; set; }
}

public sealed class NotificationSubEntity
{
    public string channel { get; set; } = default!;
    public string notificationType { get; set; } = default!;
    public string notificationVersion { get; set; } = default!;
    public DateTimeOffset? sendAt { get; set; }
    public List<string> recipient { get; set; } = [];
}
