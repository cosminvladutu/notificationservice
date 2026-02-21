using Microsoft.Extensions.Logging;
using NotificationService.Domain.Enums;
using NotificationService.Domain.Interfaces;
using NotificationService.Domain.Models;

namespace NotificationService.Application.Services;

public sealed class SendNotificationService(
    INotificationRepository repository,
    ITemplateRenderer templateRenderer,
    ISmsSender smsSender,
    IEmailSender emailSender,
    ILogger<SendNotificationService> logger) : ISendNotificationService
{
    private const int MaxRetryCount = 5;

    public async Task<IReadOnlyList<NotificationItem>> GetReadyItemsAsync(
        ChannelType channel,
        int batchSize,
        CancellationToken ct)
    {
        var pendingItems = await repository.GetItemsByChannelAndStatusAsync(
            channel,
            NotificationStatus.Pending,
            batchSize,
            ct);

        var failedItems = await repository.GetItemsByChannelAndStatusAsync(
            channel,
            NotificationStatus.Failed,
            batchSize,
            ct);

        var retryableFailedItems = failedItems
            .Where(item => item.RetryCount < MaxRetryCount)
            .ToList();

        foreach (var item in retryableFailedItems)
        {
            item.ResetForRetry();
            await repository.UpdateItemAsync(item, ct);
        }

        var timeProvider = TimeProvider.System;

        return pendingItems
            .Concat(retryableFailedItems)
            .Where(item => item.IsReadyToSend(timeProvider))
            .Take(batchSize)
            .ToList()
            .AsReadOnly();
    }

    public string RenderContent(NotificationItem item)
        => templateRenderer.Render(
            item.Channel,
            item.NotificationType,
            item.NotificationVersion,
            new Dictionary<string, string>(item.TemplateData));

    public async Task SendSmsAsync(NotificationItem item, CancellationToken ct)
    {
        try
        {
            await smsSender.SendAsync(item.Recipient, item.RenderedContent!, ct);
            item.MarkDone();
        }
        catch (Exception ex)
        {
            item.MarkFailed(ex.Message);
            logger.LogError(ex, "SMS send failed for item {Id}", item.Id);
            throw;
        }
        finally
        {
            await repository.UpdateItemAsync(item, ct);
        }
    }

    public async Task SendEmailAsync(NotificationItem item, CancellationToken ct)
    {
        try
        {
            await emailSender.SendAsync(
                item.Recipient,
                $"Notification - {item.NotificationType}",
                item.RenderedContent!,
                ct);
            item.MarkDone();
        }
        catch (Exception ex)
        {
            item.MarkFailed(ex.Message);
            logger.LogError(ex, "Email send failed for item {Id}", item.Id);
            throw;
        }
        finally
        {
            await repository.UpdateItemAsync(item, ct);
        }
    }
}
