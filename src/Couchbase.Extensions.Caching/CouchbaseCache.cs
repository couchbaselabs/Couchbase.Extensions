using System;
using System.Threading;
using System.Threading.Tasks;
using Couchbase.Core.Exceptions.KeyValue;
using Couchbase.Core.IO.Transcoders;
using Couchbase.KeyValue;
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Options;

namespace Couchbase.Extensions.Caching
{
    /// <inheritdoc />
    public class CouchbaseCache : ICouchbaseCache
    {
        internal static readonly TimeSpan InfiniteLifetime = TimeSpan.Zero;

        private readonly ITypeTranscoder _transcoder = new LegacyTranscoder();

        /// <inheritdoc />
        public ICouchbaseCacheCollectionProvider CollectionProvider { get; }

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
        public CouchbaseCache(ICouchbaseCacheCollectionProvider provider, IOptions<CouchbaseCacheOptions> options)
        {
            Options = options.Value;
            CollectionProvider = provider ?? throw new ArgumentNullException(nameof(provider));
        }

        /// <summary>
        /// Gets a cache item by its key, returning null if the item does not exist within the Cache.
        /// </summary>
        /// <param name="key">The key to lookup the item.</param>
        /// <returns>The cache item if found, otherwise null.</returns>
        public byte[]? Get(string key) =>
            GetAsync(key).GetAwaiter().GetResult();

        /// <summary>
        /// Gets a cache item by its key asynchronously, returning null if the item does not exist within the Cache.
        /// </summary>
        /// <param name="key">The key to lookup the item.</param>
        /// <param name="token">The <see cref="CancellationToken"/> for the operation.</param>
        /// <returns>The cache item if found, otherwise null.</returns>
        public async Task<byte[]?> GetAsync(string key, CancellationToken token = new CancellationToken())
        {
            token.ThrowIfCancellationRequested();
            if (key == null)
            {
                throw new ArgumentNullException(nameof(key));
            }

            try
            {
                var collection = await CollectionProvider.GetCollectionAsync().ConfigureAwait(false);
                var result = await collection.GetAsync(key, new GetOptions().Transcoder(_transcoder))
                    .ConfigureAwait(false);
                return result.ContentAs<byte[]>();
            }
            catch (DocumentNotFoundException)
            {
                return null;
            }
        }

        /// <summary>
        /// Sets a cache item using its key. If the key exists, it will not be updated.
        /// </summary>
        /// <param name="key">The key for the cache item.</param>
        /// <param name="value">An array of bytes representing the item.</param>
        /// <param name="options">The <see cref="DistributedCacheEntryOptions"/> for the item; note that only sliding expiration is currently supported.</param>
        public void Set(string key, byte[] value, DistributedCacheEntryOptions? options = null) =>
            SetAsync(key, value, options).GetAwaiter().GetResult();

        /// <summary>
        /// Sets a cache item using its key asynchronously. If the key exists, it will not be updated.
        /// </summary>
        /// <param name="key">The key for the cache item.</param>
        /// <param name="value">An array of bytes representing the item.</param>
        /// <param name="options">The <see cref="DistributedCacheEntryOptions"/> for the item; note that only sliding expiration is currently supported.</param>
        /// <param name="token">The <see cref="CancellationToken"/> for the operation.</param>
        public async Task SetAsync(string key, byte[] value, DistributedCacheEntryOptions? options,
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

            var collection = await CollectionProvider.GetCollectionAsync().ConfigureAwait(false);
            await collection.UpsertAsync(key, value, new UpsertOptions().Transcoder(_transcoder).Expiry(GetLifetime(options)))
                .ConfigureAwait(false);
        }

        /// <summary>
        /// Refreshes or "touches" a key updating it's lifetime expiration.
        /// </summary>
        /// <param name="key">The key for the cache item.</param>
        public void Refresh(string key) =>
            RefreshAsync(key).GetAwaiter().GetResult();

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

            var collection = await CollectionProvider.GetCollectionAsync().ConfigureAwait(false);
            await collection.TouchAsync(key, GetLifetime()).ConfigureAwait(false);
        }

        /// <summary>
        /// Removes an item from the cache by it's key.
        /// </summary>
        /// <param name="key">The key for the cache item.</param>
        public void Remove(string key) =>
            RemoveAsync(key).GetAwaiter().GetResult();

        /// <summary>
        /// Removes an item from the cache by it's key asynchronously.
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

            try
            {
                var collection = await CollectionProvider.GetCollectionAsync().ConfigureAwait(false);
                await collection.RemoveAsync(key).ConfigureAwait(false);
            }
            catch (DocumentNotFoundException)
            {
                // Ignore
            }
        }

        /// <inheritdoc />
        public TimeSpan GetLifetime(DistributedCacheEntryOptions? options = null)
        {
            return CouchbaseCacheExtensions.GetLifetime(this, options);
        }
    }
}
