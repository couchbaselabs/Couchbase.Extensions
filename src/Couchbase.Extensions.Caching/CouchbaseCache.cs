using System;
using System.Threading;
using System.Threading.Tasks;
using Couchbase.Core;
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Options;

namespace Couchbase.Extensions.Caching
{
    /// <inheritdoc />
    public class CouchbaseCache : ICouchbaseCache
    {
        internal static readonly TimeSpan InfiniteLifetime = TimeSpan.Zero;

        /// <inheritdoc />
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
        /// Standard CTOR for CouchbaseCache
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
        /// <param name="token">The <see cref="CancellationToken"/> for the operation.</param>
        /// <returns>The cache item if found, otherwise null.</returns>
        public async Task<byte[]> GetAsync(string key, CancellationToken token = new CancellationToken())
        {
            token.ThrowIfCancellationRequested();
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
        /// <param name="token">The <see cref="CancellationToken"/> for the operation.</param>
        /// <returns>The cache item if found, otherwise null.</returns>
        async Task<byte[]> IDistributedCache.GetAsync(string key, CancellationToken token)
        {
            token.ThrowIfCancellationRequested();
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
        /// <param name="token">The <see cref="CancellationToken"/> for the operation.</param>
        public Task SetAsync(string key, byte[] value, DistributedCacheEntryOptions options,
            CancellationToken token = new CancellationToken())
        {
            token.ThrowIfCancellationRequested();
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
        /// <param name="token">The <see cref="CancellationToken"/> for the operation.</param>
        public async Task RefreshAsync(string key, CancellationToken token = new CancellationToken())
        {
            token.ThrowIfCancellationRequested();
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
        /// <param name="token">The <see cref="CancellationToken"/> for the operation.</param>
        public async Task RemoveAsync(string key, CancellationToken token = new CancellationToken())
        {
            token.ThrowIfCancellationRequested();
            if (key == null)
            {
                throw new ArgumentNullException(nameof(key));
            }

            await Bucket.RemoveAsync(key);
        }

        /// <inheritdoc />
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
