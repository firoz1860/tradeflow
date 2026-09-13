using System.Text.Json;
using Microsoft.Extensions.Caching.Distributed;

namespace TradeFlow.Infrastructure.Caching;

public sealed class RedisCacheService(IDistributedCache cache)
{
    public async Task SetAsync<T>(string key, T value, TimeSpan expiresIn, CancellationToken cancellationToken)
    {
        var json = JsonSerializer.Serialize(value);
        await cache.SetStringAsync(key, json, new DistributedCacheEntryOptions { AbsoluteExpirationRelativeToNow = expiresIn }, cancellationToken);
    }

    public async Task<T?> GetAsync<T>(string key, CancellationToken cancellationToken)
    {
        var json = await cache.GetStringAsync(key, cancellationToken);
        return json is null ? default : JsonSerializer.Deserialize<T>(json);
    }
}
