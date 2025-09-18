namespace ArtAuctionHub.Application.Interfaces
{
    /// <summary>
    /// Defines a contract for a cache service abstraction.
    /// </summary>
    /// <remarks>
    /// This interface allows controllers or services to store and retrieve
    /// frequently accessed data in an in-memory cache. It helps improve 
    /// performance by avoiding repeated calls to databases or APIs.
    /// </remarks>
    public interface ICacheService
    {
        /// <summary>
        /// Retrieves a value from the cache by its key, or creates it using the provided factory function if not found.
        /// </summary>
        /// <typeparam name="T">
        /// The type of object that will be stored and retrieved from the cache.
        /// </typeparam>
        /// <param name="cacheKey">
        /// A unique string key used to identify the cached item.
        /// </param>
        /// <param name="factory">
        /// A function that produces the value if the key is not found in the cache. 
        /// This function will typically call a database, API, or other expensive resource.
        /// </param>
        /// <param name="absoluteExpireTime">
        /// (Optional) A fixed time interval after which the cached item will expire, 
        /// regardless of usage. This ensures stale data is eventually discarded.
        /// </param>
        /// <param name="slidingExpireTime">
        /// (Optional) A time interval that resets every time the cached item is accessed. 
        /// If the item is not used within this interval, it will expire.
        /// </param>
        /// <returns>
        /// The cached value of type <typeparamref name="T"/>. If it was not already cached,
        /// the value is created using the <paramref name="factory"/> function, added to the cache,
        /// and then returned.
        /// </returns>
        Task<T?> GetOrCreateAsync<T>(
            string cacheKey,
            Func<Task<T>> factory,
            TimeSpan? absoluteExpireTime = null,
            TimeSpan? slidingExpireTime = null);

        /// <summary>
        /// Removes an entry from the cache by its key.
        /// </summary>
        /// <param name="cacheKey">
        /// The unique string key used to identify the cached item to be removed.
        /// </param>
        /// <remarks>
        /// This is useful when data changes (e.g., an item is updated or deleted in the database),
        /// and the cached value needs to be invalidated so that future requests will retrieve fresh data.
        /// </remarks>
        void Remove(string cacheKey);
    }
}