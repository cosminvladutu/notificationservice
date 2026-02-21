namespace NotificationService.Domain.Interfaces;

public interface ISmsSender
{
    Task SendAsync(string recipient, string body, CancellationToken ct);
}
