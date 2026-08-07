using ManagementSystem.Api.Services.Interfaces;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Memory;

namespace ManagementSystem.Api.Services.Implementation;

public class CountCache<T> : ICountCache
{
    private readonly IMemoryCache _cache;
    private static readonly TimeSpan Ttl = TimeSpan.FromMinutes(5);

    public CountCache(IMemoryCache cache)
    {
        _cache = cache;
    }

    public async Task<int> GetOrAddCountAsync<T>(string cacheKey, IQueryable<T> query, CancellationToken ct = default)
    {
        return await _cache.GetOrCreateAsync(cacheKey, async entry =>
        {
            entry.AbsoluteExpirationRelativeToNow = Ttl;
            return await query.CountAsync(ct);
        });
    }

    public Task InvalidateAsync(string cacheKey, CancellationToken ct = default)
    {
        _cache.Remove(cacheKey);
        return Task.CompletedTask;
    }

}
