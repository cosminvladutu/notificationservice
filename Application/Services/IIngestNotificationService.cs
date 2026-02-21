using NotificationService.Application.Dtos;

namespace NotificationService.Application.Services;

public interface IIngestNotificationService
{
    Task IngestAsync(IncomingNotificationDto dto, CancellationToken ct);
}
