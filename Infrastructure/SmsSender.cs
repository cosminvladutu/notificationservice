using Microsoft.Extensions.Options;
using NotificationService.Domain.Interfaces;
using Twilio;
using Twilio.Rest.Api.V2010.Account;
using Twilio.Types;
using System.ComponentModel.DataAnnotations;

namespace NotificationService.Infrastructure;

public sealed class SmsSender(IOptions<TwilioOptions> options) : ISmsSender
{
    private readonly TwilioOptions _options = options.Value;

    public async Task SendAsync(string recipient, string body, CancellationToken ct)
    {
        TwilioClient.Init(_options.AccountSid, _options.AuthToken);
        await MessageResource.CreateAsync(
            to: new PhoneNumber(recipient),
            from: new PhoneNumber(_options.FromNumber),
            body: body);
    }
}

public sealed class TwilioOptions
{
    [Required]
    public string AccountSid { get; set; } = string.Empty;

    [Required]
    public string AuthToken { get; set; } = string.Empty;

    [Required]
    public string FromNumber { get; set; } = string.Empty;
}
