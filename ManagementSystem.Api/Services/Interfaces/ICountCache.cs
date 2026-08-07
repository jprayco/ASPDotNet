namespace ManagementSystem.Api.Services.Interfaces;

public interface ICountCache
{
    Task<int> GetOrAddCountAsync<T>(string cacheKey, IQueryable<T> query, CancellationToken ct = default);
    Task InvalidateAsync(string cacheKey, CancellationToken ct = default);

}
