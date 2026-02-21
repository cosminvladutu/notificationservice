using NotificationService.Domain.Enums;

namespace NotificationService.Domain.Exceptions;

public sealed class TemplateNotFoundException(ChannelType channel, string type, string version)
    : Exception($"Template not found for {channel}.{type}_{version}");
