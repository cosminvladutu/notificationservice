using Azure.Messaging.ServiceBus.Administration;
using Microsoft.Extensions.Diagnostics.HealthChecks;

namespace NotificationService.HealthChecks;

public sealed class ServiceBusQueueHealthCheck(string connectionString, string queueName) : IHealthCheck
{
    private readonly string _connectionString = connectionString;
    private readonly string _queueName = queueName;

    public async Task<HealthCheckResult> CheckHealthAsync(
        HealthCheckContext context,
        CancellationToken cancellationToken = default)
    {
        try
        {
            var client = new ServiceBusAdministrationClient(_connectionString);
            await client.GetQueueRuntimePropertiesAsync(_queueName, cancellationToken);
            return HealthCheckResult.Healthy();
        }
        catch (Exception ex)
        {
            return HealthCheckResult.Unhealthy("Service Bus queue check failed", ex);
        }
    }
}
