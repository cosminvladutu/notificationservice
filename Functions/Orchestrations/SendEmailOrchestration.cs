using Microsoft.Azure.Functions.Worker;
using Microsoft.DurableTask;
using NotificationService.Domain.Models;

namespace NotificationService.Functions.Orchestrations;

public static class SendEmailOrchestration
{
    [Function(nameof(SendEmailOrchestration))]
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

        var sendInput = new Activities.SendNotificationInput(item, rendered);

        await ctx.CallActivityAsync(
            nameof(Activities.SendEmailActivity),
            sendInput,
            retryOptions);
    }
}
