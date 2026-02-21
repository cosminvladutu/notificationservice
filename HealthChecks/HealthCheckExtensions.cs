using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Diagnostics.HealthChecks;

namespace NotificationService.HealthChecks;

public static class HealthCheckExtensions
{
    public static IHealthChecksBuilder AddCosmosDb(
        this IHealthChecksBuilder builder,
        string connectionString)
    {
        builder.AddCheck("cosmosdb", new CosmosDbHealthCheck(connectionString));
        return builder;
    }

    public static IHealthChecksBuilder AddAzureServiceBusQueue(
        this IHealthChecksBuilder builder,
        string connectionString,
        string queueName)
    {
        builder.AddCheck($"servicebus-{queueName}", new ServiceBusQueueHealthCheck(connectionString, queueName));
        return builder;
    }
}
