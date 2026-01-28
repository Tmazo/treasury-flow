using Microsoft.Extensions.Caching.Memory;
using TreasuryFlow.Application.Shared.Caching.Interfaces;

namespace TreasuryFlow.Infrastructure.Shared.Caching.Services
{
    public class MemoryCacheService(IMemoryCache cache) : ICacheService
    {
        public Task<T?> GetAsync<T>(
            string key,
            CancellationToken cancellationToken)
        {
            cache.TryGetValue(key, out T? value);
            return Task.FromResult(value);
        }

        public Task SetAsync<T>(
            string key,
            T value,
            TimeSpan ttl,
            CancellationToken cancellationToken)
        {
            var options = new MemoryCacheEntryOptions
            {
                AbsoluteExpirationRelativeToNow = ttl
            };

            cache.Set(key, value, options);
            return Task.CompletedTask;
        }

        public Task RemoveAsync(
            string key,
            CancellationToken cancellationToken)
        {
            cache.Remove(key);
            return Task.CompletedTask;
        }
    }
}
