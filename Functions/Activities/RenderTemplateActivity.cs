using Microsoft.Azure.Functions.Worker;
using NotificationService.Application.Services;
using NotificationService.Domain.Models;

namespace NotificationService.Functions.Activities;

public sealed class RenderTemplateActivity(ISendNotificationService sendService)
{
    [Function(nameof(RenderTemplateActivity))]
    public string Run([ActivityTrigger] NotificationItem item)
        => sendService.RenderContent(item);
}
