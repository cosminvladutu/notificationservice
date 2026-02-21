using System.Reflection;
using NotificationService.Domain.Enums;
using NotificationService.Domain.Interfaces;
using NotificationService.Domain.Exceptions;

namespace NotificationService.Templates.Services;

public sealed class EmbeddedTemplateRenderer : ITemplateRenderer
{
    private static readonly Dictionary<string, string> s_cache = LoadAll();

    public string Render(
        ChannelType channel,
        string notificationType,
        string notificationVersion,
        Dictionary<string, string> templateData)
    {
        var key = BuildKey(channel, notificationType, notificationVersion);

        if (!s_cache.TryGetValue(key, out var template))
        {
            // Try fallback to v1 or throw detailed error
            var fallbackKey = BuildKey(channel, notificationType, "v1");
            if (!s_cache.TryGetValue(fallbackKey, out template))
            {
                throw new TemplateNotFoundException(channel, notificationType, notificationVersion);
            }
        }

        foreach (var (placeholder, value) in templateData)
        {
            template = template.Replace($"{{{{{placeholder}}}}}", value);
        }

        return template;
    }

    private static string BuildKey(ChannelType channel, string type, string version)
        => $"{channel}.{type}_{version}".ToLowerInvariant();

    private static Dictionary<string, string> LoadAll()
    {
        var assembly = Assembly.GetExecutingAssembly();
        var dict = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);

        foreach (var resourceName in assembly.GetManifestResourceNames())
        {
            var key = ParseResourceName(resourceName);
            if (key is null)
            {
                continue;
            }

            using var stream = assembly.GetManifestResourceStream(resourceName);
            if (stream is null)
            {
                continue;
            }

            using var reader = new StreamReader(stream);
            dict[key] = reader.ReadToEnd();
        }

        return dict;
    }

    private static string? ParseResourceName(string resourceName)
    {
        var parts = resourceName.Split('.');
        var index = Array.IndexOf(parts, "Templates");
        if (index < 0 || index + 3 > parts.Length)
        {
            return null;
        }

        var channel = parts[index + 1];
        var file = parts[index + 2];
        return $"{channel}.{file}".ToLowerInvariant();
    }
}
