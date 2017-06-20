using System;
using System.Threading.Tasks;
using Couchbase.Core;
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Options;

namespace Couchbase.Extensions.Caching
{
    /// <summary>
    /// A <see cref="IDistributedCache"/> implementation for Couchbase Server.
    /// </summary>
    public class CouchbaseCache : ICouchbaseCache
    {
        internal static readonly TimeSpan InfiniteLifetime = TimeSpan.Zero;

        /// <summary>
        /// Gets the Couchbase bucket as the backing store.
        /// </summary>
        /// <value>
        /// The bucket.
        /// </value>
        public IBucket Bucket { get; }

        /// <summary>
        /// Gets the options used by the Cache.
        /// </summary>
        /// <value>
        /// The options.
        /// </value>
        /// <inheritdoc />
        public CouchbaseCacheOptions Options { get; }

        /// <summary>
        /// Constructor for <see cref="CouchbaseCache"/> - if the <see cref="CouchbaseCacheOptions.Bucket"/> field is null,
        /// the bucket will attempted to be retrieved from <see cref="ClusterHelper"/>. If <see cref="ClusterHelper"/> has
        /// not been initialized, then an exception will be thrown.
        /// </summary>
        /// <param name="provider"></param>
        /// <param name="options"></param>
        public CouchbaseCache(ICouchbaseCacheBucketProvider provider, IOptions<CouchbaseCacheOptions> options)
        {
            Options = options.Value;
            Bucket = provider.GetBucket();
        }

        /// <summary>
        /// Gets a cache item by its key, returning null if the item does not exist within the Cache.
        /// </summary>
        /// <param name="key">The key to lookup the item.</param>
        /// <returns>The cache item if found, otherwise null.</returns>
        public byte[] Get(string key)
        {
            if (key == null)
            {
                throw new ArgumentNullException(nameof(key));
            }
            return Bucket.Get<byte[]>(key).Value;
        }

        /// <summary>
        /// Gets a cache item by its key asynchronously, returning null if the item does not exist within the Cache.
        /// </summary>
        /// <param name="key">The key to lookup the item.</param>
        /// <returns>The cache item if found, otherwise null.</returns>
        async Task<byte[]> IDistributedCache.GetAsync(string key)
        {
            if (key == null)
            {
                throw new ArgumentNullException(nameof(key));
            }

            return (await Bucket.GetAsync<byte[]>(key)).Value;
        }

        /// <summary>
        /// Gets a cache item by its key asynchronously, returning null if the item does not exist within the Cache.
        /// </summary>
        /// <param name="key">The key to lookup the item.</param>
        /// <returns>The cache item if found, otherwise null.</returns>
        public async Task<byte[]> GetAsync(string key)
        {
            if (key == null)
            {
                throw new ArgumentNullException(nameof(key));
            }
            return (await Bucket.GetAsync<byte[]>(key)).Value;
        }

        /// <summary>
        /// Sets a cache item using its key. If the key exists, it will not be updated.
        /// </summary>
        /// <param name="key">The key for the cache item.</param>
        /// <param name="value">An array of bytes representing the item.</param>
        /// <param name="options">The <see cref="DistributedCacheEntryOptions"/> for the item; note that only sliding expiration is currently supported.</param>
        public void Set(string key, byte[] value, DistributedCacheEntryOptions options = null)
        {
            if (key == null)
            {
                throw new ArgumentNullException(nameof(key));
            }
            if (value == null)
            {
                throw new ArgumentNullException(nameof(value));
            }

            var lifeTime = GetLifetime(options);
            Bucket.Upsert(key, value, lifeTime);
        }

        /// <summary>
        /// Sets a cache item using its key asynchronously. If the key exists, it will not be updated.
        /// </summary>
        /// <param name="key">The key for the cache item.</param>
        /// <param name="value">An array of bytes representing the item.</param>
        /// <param name="options">The <see cref="DistributedCacheEntryOptions"/> for the item; note that only sliding expiration is currently supported.</param>
        public Task SetAsync(string key, byte[] value, DistributedCacheEntryOptions options = null)
        {
            if (key == null)
            {
                throw new ArgumentNullException(nameof(key));
            }
            if (value == null)
            {
                throw new ArgumentNullException(nameof(value));
            }

            var lifeTime = GetLifetime(options);
            return Bucket.UpsertAsync(key, value, lifeTime);
        }

        /// <summary>
        /// Refreshes or "touches" a key updating it's lifetime expiration.
        /// </summary>
        /// <param name="key">The key for the cache item.</param>
        public void Refresh(string key)
        {
            if (key == null)
            {
                throw new ArgumentNullException(nameof(key));
            }

            var lifeTime = GetLifetime();
            Bucket.Touch(key, lifeTime);
        }

        /// <summary>
        /// Refreshes or "touches" a key updating it's lifetime expiration asynchronously.
        /// </summary>
        /// <param name="key">The key for the cache item.</param>
        public async Task RefreshAsync(string key)
        {
            if (key == null)
            {
                throw new ArgumentNullException(nameof(key));
            }

            var lifeTime = GetLifetime();
            await Bucket.TouchAsync(key, lifeTime);
        }

        /// <summary>
        /// Removes an item from the cache by it's key.
        /// </summary>
        /// <param name="key">The key for the cache item.</param>
        public void Remove(string key)
        {
            if (key == null)
            {
                throw new ArgumentNullException(nameof(key));
            }

            Bucket.Remove(key);
        }

        /// <summary>
        /// Removes an item from the cache by it's key asynchonously.
        /// </summary>
        /// <param name="key">The key for the cache item.</param>
        public async Task RemoveAsync(string key)
        {
            if (key == null)
            {
                throw new ArgumentNullException(nameof(key));
            }

            await Bucket.RemoveAsync(key);
        }

        /// <summary>
        /// Gets the lifetime or expiration from the <see cref="DistributedCacheEntryOptions"/>. Only
        /// sliding expiration is currently supported. If <see cref="DistributedCacheEntryOptions.SlidingExpiration"/>
        /// if not set, then the <see cref="CouchbaseCacheOptions.LifeSpan"/> will be used. If it is empty then the
        /// default lifespan of zero (0) will be used which is infinite expiration.
        /// </summary>
        /// <param name="options"></param>
        /// <returns></returns>
        public TimeSpan GetLifetime(DistributedCacheEntryOptions options = null)
        {
            if (options?.SlidingExpiration != null)
            {
                return options.SlidingExpiration.Value;
            }

            return Options.Value.LifeSpan ?? InfiniteLifetime;
        }
    }
}
