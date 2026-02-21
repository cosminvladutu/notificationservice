using Microsoft.Azure.Functions.Worker;
using Microsoft.DurableTask;
using NotificationService.Domain.Models;

namespace NotificationService.Functions.Orchestrations;

public static class SendSmsOrchestration
{
    [Function(nameof(SendSmsOrchestration))]
    public static async Task Run([OrchestrationTrigger] TaskOrchestrationContext ctx)
    {
        var item = ctx.GetInput<NotificationItem>()!;

        var retryOptions = TaskOptions.FromRetryPolicy(new RetryPolicy(
            maxNumberOfAttempts: 3,
            firstRetryInterval: TimeSpan.FromSeconds(10),
            backoffCoefficient: 2.0));

        var rendered = await ctx.CallActivityAsync<string>(
            nameof(Activities.RenderTemplateActivity),
            item,
            retryOptions);

        // Pass rendered content explicitly to the send activity
        var sendInput = new Activities.SendNotificationInput(item, rendered);

        await ctx.CallActivityAsync(
            nameof(Activities.SendSmsActivity),
            sendInput,
            retryOptions);
    }
}
