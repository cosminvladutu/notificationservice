using Microsoft.Azure.Cosmos;
using Microsoft.Extensions.Options;
using NotificationService.Domain.Enums;
using NotificationService.Domain.Interfaces;
using NotificationService.Domain.Models;
using NotificationService.Persistence.DbModels;
using NotificationService.Persistence.Mappers;

namespace NotificationService.Persistence.Repositories;

public sealed class CosmosNotificationRepository(
    CosmosClient cosmosClient,
    IOptions<CosmosOptions> options) : INotificationRepository
{
    private readonly CosmosOptions _options = options.Value;

    private Container RequestsContainer
        => cosmosClient.GetContainer(_options.DatabaseName, _options.RequestsContainerName);

    private Container ItemsContainer
        => cosmosClient.GetContainer(_options.DatabaseName, _options.ItemsContainerName);

    public async Task SaveRequestAsync(NotificationRequest request, CancellationToken ct)
    {
        var utcNow = DateTimeOffset.UtcNow;
        var entity = NotificationRequestMapper.ToEntity(request, utcNow);
        await RequestsContainer.UpsertItemAsync(entity, new PartitionKey(entity.partitionKey), cancellationToken: ct);
    }

    public async Task<NotificationRequest?> GetRequestByIdAsync(Guid id, CancellationToken ct)
    {
        var query = new QueryDefinition("SELECT * FROM c WHERE c.id = @id")
            .WithParameter("@id", id.ToString());

        var iterator = RequestsContainer.GetItemQueryIterator<NotificationRequestEntity>(
            query,
            requestOptions: new QueryRequestOptions { MaxItemCount = 1 });

        while (iterator.HasMoreResults)
        {
            var response = await iterator.ReadNextAsync(ct);
            var entity = response.FirstOrDefault();
            if (entity is not null)
            {
                return NotificationRequestMapper.ToDomain(entity);
            }
        }

        return null;
    }

    public async Task<IReadOnlyList<NotificationRequest>> GetRequestsByStatusAsync(
        NotificationStatus status,
        int limit,
        CancellationToken ct)
    {
        var query = new QueryDefinition("SELECT * FROM c WHERE c.status = @status")
            .WithParameter("@status", status.ToString());

        var iterator = RequestsContainer.GetItemQueryIterator<NotificationRequestEntity>(
            query,
            requestOptions: new QueryRequestOptions { MaxItemCount = limit });

        var results = new List<NotificationRequest>();
        while (iterator.HasMoreResults && results.Count < limit)
        {
            var response = await iterator.ReadNextAsync(ct);
            results.AddRange(response.Select(NotificationRequestMapper.ToDomain));
        }

        return results;
    }

    public async Task UpdateRequestAsync(NotificationRequest request, CancellationToken ct)
    {
        var existing = await RequestsContainer.ReadItemAsync<NotificationRequestEntity>(
            request.NotificationId.ToString(),
            new PartitionKey(request.App),
            cancellationToken: ct);

        var utcNow = DateTimeOffset.UtcNow;
        var updated = NotificationRequestMapper.ToEntity(request, utcNow, existing.Resource.createdAt);
        await RequestsContainer.UpsertItemAsync(updated, new PartitionKey(updated.partitionKey), cancellationToken: ct);
    }

    public async Task SaveItemAsync(NotificationItem item, CancellationToken ct)
    {
        var utcNow = DateTimeOffset.UtcNow;
        var entity = NotificationItemMapper.ToEntity(item, utcNow);
        await ItemsContainer.UpsertItemAsync(entity, new PartitionKey(entity.partitionKey), cancellationToken: ct);
    }

    public async Task SaveItemsAsync(IEnumerable<NotificationItem> items, CancellationToken ct)
    {
        var utcNow = DateTimeOffset.UtcNow;
        var tasks = items.Select(item =>
        {
            var entity = NotificationItemMapper.ToEntity(item, utcNow);
            return ItemsContainer.UpsertItemAsync(entity, new PartitionKey(entity.partitionKey), cancellationToken: ct);
        });

        await Task.WhenAll(tasks);
    }

    public async Task<IReadOnlyList<NotificationItem>> GetItemsByChannelAndStatusAsync(
        ChannelType channel,
        NotificationStatus status,
        int limit,
        CancellationToken ct)
    {
        var partitionKey = channel.ToString().ToLowerInvariant();
        var query = new QueryDefinition(
            "SELECT * FROM c WHERE c.status = @status")
            .WithParameter("@status", status.ToString());

        var iterator = ItemsContainer.GetItemQueryIterator<NotificationItemEntity>(
            query,
            requestOptions: new QueryRequestOptions
            {
                PartitionKey = new PartitionKey(partitionKey),
                MaxItemCount = limit
            });

        var results = new List<NotificationItem>();
        while (iterator.HasMoreResults && results.Count < limit)
        {
            var response = await iterator.ReadNextAsync(ct);
            results.AddRange(response.Select(NotificationItemMapper.ToDomain));
        }

        return results;
    }

    public async Task UpdateItemAsync(NotificationItem item, CancellationToken ct)
    {
        var partitionKey = item.Channel.ToString().ToLowerInvariant();
        var existing = await ItemsContainer.ReadItemAsync<NotificationItemEntity>(
            item.Id.ToString(),
            new PartitionKey(partitionKey),
            cancellationToken: ct);

        var utcNow = DateTimeOffset.UtcNow;
        var updated = NotificationItemMapper.ToEntity(item, utcNow, existing.Resource.createdAt);
        await ItemsContainer.UpsertItemAsync(updated, new PartitionKey(updated.partitionKey), cancellationToken: ct);
    }
}

public sealed class CosmosOptions
{
    public string DatabaseName { get; set; } = "notificationdb";
    public string RequestsContainerName { get; set; } = "notification-requests";
    public string ItemsContainerName { get; set; } = "notification-items";
}
