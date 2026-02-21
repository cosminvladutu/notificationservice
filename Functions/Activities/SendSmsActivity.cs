using Microsoft.Azure.Functions.Worker;
using NotificationService.Application.Services;

namespace NotificationService.Functions.Activities;

public sealed class SendSmsActivity(ISendNotificationService sendService)
{
    [Function(nameof(SendSmsActivity))]
    public async Task Run([ActivityTrigger] SendNotificationInput input)
    {
        input.Item.SetRenderedContent(input.RenderedContent);
        await sendService.SendSmsAsync(input.Item, default);
    }
}
