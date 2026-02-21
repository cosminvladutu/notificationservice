using Microsoft.Azure.Cosmos;
using Microsoft.Extensions.Diagnostics.HealthChecks;

namespace NotificationService.HealthChecks;

public sealed class CosmosDbHealthCheck(string connectionString) : IHealthCheck
{
    private readonly string _connectionString = connectionString;

    public async Task<HealthCheckResult> CheckHealthAsync(
        HealthCheckContext context,
        CancellationToken cancellationToken = default)
    {
        try
        {
            using var client = new CosmosClient(_connectionString);
            await client.ReadAccountAsync();
            return HealthCheckResult.Healthy();
        }
        catch (Exception ex)
        {
            return HealthCheckResult.Unhealthy("Cosmos DB check failed", ex);
        }
    }
}
