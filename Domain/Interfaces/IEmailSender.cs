namespace NotificationService.Domain.Interfaces;

public interface IEmailSender
{
    Task SendAsync(string recipient, string subject, string htmlBody, CancellationToken ct);
}
