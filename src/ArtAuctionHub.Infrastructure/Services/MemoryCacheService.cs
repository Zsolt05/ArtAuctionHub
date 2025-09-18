using ArtAuctionHub.Application.Interfaces;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Logging;

namespace ArtAuctionHub.Infrastructure.Services
{
    /// <summary>
    /// A concrete implementation of <see cref="ICacheService"/> using <see cref="IMemoryCache"/>.
    /// </summary>
    /// <remarks>
    /// This service provides a simple way to store and retrieve data in memory 
    /// within the lifetime of the application. It is most suitable for small 
    /// to medium applications or scenarios where distributed caching is not required.
    /// </remarks>
    public class MemoryCacheService : ICacheService
    {
        private readonly IMemoryCache _cache;
        private readonly ILogger<MemoryCacheService> _logger;

        /// <summary>
        /// Initializes a new instance of the <see cref="MemoryCacheService"/> class.
        /// </summary>
        /// <param name="cache">
        /// The <see cref="IMemoryCache"/> instance provided by dependency injection.
        /// </param>
        /// <param name="logger">
        /// The <see cref="ILogger{MemoryCacheService}"/> instance for logging cache operations.
        /// </param>
        public MemoryCacheService(IMemoryCache cache, ILogger<MemoryCacheService> logger)
        {
            _cache = cache;
            _logger = logger;
        }

        /// <inheritdoc/>
        public async Task<T?> GetOrCreateAsync<T>(
            string cacheKey,
            Func<Task<T>> factory,
            TimeSpan? absoluteExpireTime = null,
            TimeSpan? slidingExpireTime = null)
        {
            // Try to retrieve the cached value by key
            if (_cache.TryGetValue(cacheKey, out T? value))
            {
                _logger.LogInformation("Cache hit for key: {CacheKey}", cacheKey);
                return value;
            }

            // If not found in cache, generate it using the provided factory function
            value = await factory();

            // Configure cache entry options
            var options = new MemoryCacheEntryOptions();
            if (absoluteExpireTime.HasValue)
                options.SetAbsoluteExpiration(absoluteExpireTime.Value);

            if (slidingExpireTime.HasValue)
                options.SetSlidingExpiration(slidingExpireTime.Value);

            _logger.LogInformation("Cache miss for key: {CacheKey}. Caching new value.", cacheKey);
            // Store the generated value in cache
            _cache.Set(cacheKey, value, options);

            return value;
        }

        /// <inheritdoc/>
        public void Remove(string cacheKey)
        {
            _logger.LogInformation("Removing cache entry for key: {CacheKey}", cacheKey);
            _cache.Remove(cacheKey);
        }
    }
}