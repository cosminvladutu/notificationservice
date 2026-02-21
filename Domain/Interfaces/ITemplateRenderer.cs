using NotificationService.Domain.Enums;

namespace NotificationService.Domain.Interfaces;

public interface ITemplateRenderer
{
    string Render(
        ChannelType channel,
        string notificationType,
        string notificationVersion,
        Dictionary<string, string> templateData);
}
