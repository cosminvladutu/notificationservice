using Azure.Communication.Email;
using Microsoft.Extensions.Options;
using NotificationService.Domain.Interfaces;
using Azure;

namespace NotificationService.Infrastructure;

public sealed class EmailSender(IOptions<AcsEmailOptions> options) : IEmailSender
{
    private readonly AcsEmailOptions _options = options.Value;

    public async Task SendAsync(string recipient, string subject, string htmlBody, CancellationToken ct)
    {
        var client = new EmailClient(_options.ConnectionString);
        var message = new EmailMessage(
            _options.FromAddress,
            new EmailRecipients(new[] { new EmailAddress(recipient) }),
            new EmailContent(subject)
            {
                Html = htmlBody
            });

        await client.SendAsync(WaitUntil.Completed, message, cancellationToken: ct);
    }
}

public sealed class AcsEmailOptions
{
    public string ConnectionString { get; set; } = string.Empty;
    public string FromAddress { get; set; } = string.Empty;
}
