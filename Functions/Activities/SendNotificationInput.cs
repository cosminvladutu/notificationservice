using NotificationService.Domain.Models;

namespace NotificationService.Functions.Activities;

public sealed record SendNotificationInput(NotificationItem Item, string RenderedContent);
