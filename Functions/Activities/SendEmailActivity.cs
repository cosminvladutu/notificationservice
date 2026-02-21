using Microsoft.Azure.Functions.Worker;
using NotificationService.Application.Services;

namespace NotificationService.Functions.Activities;

public sealed class SendEmailActivity(ISendNotificationService sendService)
{
    [Function(nameof(SendEmailActivity))]
    public async Task Run([ActivityTrigger] SendNotificationInput input)
    {
        input.Item.SetRenderedContent(input.RenderedContent);
        await sendService.SendEmailAsync(input.Item, default);
    }
}
