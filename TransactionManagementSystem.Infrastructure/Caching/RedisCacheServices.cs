
using Microsoft.Extensions.Caching.Distributed;
using System.Text.Json;

namespace TransactionManagementSystem.Infrastructure.Caching
{
    public class RedisCacheServices : ICacheService
    {
        private readonly IDistributedCache _distributedCache;
        private readonly DistributedCacheEntryOptions _defaultOptions;
        

        public RedisCacheServices(IDistributedCache distributedCache)
        {
            _distributedCache = distributedCache;
            _defaultOptions = new DistributedCacheEntryOptions
            {
                AbsoluteExpirationRelativeToNow = TimeSpan.FromMinutes(60), // Set default expiration time
                SlidingExpiration = TimeSpan.FromMinutes(30) // Optional: sliding expiration
            };
        }
        public async Task<T> GetAsync<T>(string key) where T : class
        {
            var cachedData = await _distributedCache.GetStringAsync(key);
            if (cachedData == null)
                return null;

            return JsonSerializer.Deserialize<T>(cachedData);

        }

        public async Task SetAsync<T>(string key, T value, TimeSpan? expiry = null) where T : class
        {
            var options = expiry.HasValue 
                ? new DistributedCacheEntryOptions { AbsoluteExpirationRelativeToNow = expiry.Value } 
                : _defaultOptions;

            var serializedValue = JsonSerializer.Serialize(value);
            await _distributedCache.SetStringAsync(key, serializedValue, options);
        }

        public async Task RemoveAsync(string key)
        {
            await _distributedCache.RemoveAsync(key);
        }

        public Task RemoveByPatternAsync(string pattern)
        {
            // Note: This requires a Redis-specific implementation
            // For the basic IDistributedCache interface, we can't implement pattern removal
            // You'd need to use a Redis-specific client like StackExchange.Redis
            throw new NotImplementedException("Pattern-based removal requires Redis-specific implementation");

        }
    }
}
