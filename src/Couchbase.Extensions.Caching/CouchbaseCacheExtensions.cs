using System;
using System.Threading;
using System.Threading.Tasks;
using Couchbase.Core;
using Microsoft.Extensions.Caching.Distributed;

namespace Couchbase.Extensions.Caching
{
    /// <summary>
    /// Extension methods for <see cref="CouchbaseCache"/> that provide "get and touch" semantics and generic overloads of <see cref="IDistributedCache"/> methods.
    /// </summary>
    public static class CouchbaseCacheExtensions
    {
        /// <summary>
        /// Sets a cache item using its key. If the key exists, it will not be updated.
        /// </summary>
        /// <param name="cache">The <see cref="CouchbaseCache"/> cache.</param>
        /// <param name="key">The key for the cache item.</param>
        /// <param name="value">An array of bytes representing the item.</param>
        /// <param name="options">The <see cref="DistributedCacheEntryOptions"/> for the item; note that only sliding expiration is currently supported.</param>
        public static void Set<T>(this IDistributedCache cache, string key, T value, DistributedCacheEntryOptions options)
        {
            if (key == null)
            {
                throw new ArgumentNullException(nameof(key));
            }
            if (value == null)
            {
                throw new ArgumentNullException(nameof(value));
            }

            var bucket = GetBucket(cache);
            bucket.Upsert(key, value, GetLifetime(cache, options));
        }

        /// <summary>
        /// Sets a cache item using its key asynchronously. If the key exists, it will not be updated.
        /// </summary>
        /// <param name="cache">The <see cref="CouchbaseCache"/> cache.</param>
        /// <param name="key">The key for the cache item.</param>
        /// <param name="value">An array of bytes representing the item.</param>
        /// <param name="options">The <see cref="DistributedCacheEntryOptions"/> for the item; note that only sliding expiration is currently supported.</param>
        public static async Task SetAsync<T>(this IDistributedCache cache, string key, T value, DistributedCacheEntryOptions options)
        {
            if (key == null)
            {
                throw new ArgumentNullException(nameof(key));
            }
            if (value == null)
            {
                throw new ArgumentNullException(nameof(value));
            }

            var bucket = GetBucket(cache);
            await bucket.UpsertAsync(key, value, GetLifetime(cache, options));
        }

        /// <summary>
        /// Gets a cache item by its key, returning null if the item does not exist within the Cache.
        /// </summary>
        /// <param name="cache">The <see cref="CouchbaseCache"/> cache.</param>
        /// <param name="key">The key to lookup the item.</param>
        /// <returns>The cache item if found, otherwise null.</returns>
        public static T Get<T>(this IDistributedCache cache, string key)
        {
            var bucket = GetBucket(cache);
            var result = bucket.Get<T>(key);
            return result.Value;
        }

        /// <summary>
        /// Gets a cache item by its key, returning null if the item does not exist within the Cache.
        /// </summary>
        /// <param name="cache">The <see cref="CouchbaseCache"/> cache.</param>
        /// <param name="key">The key to lookup the item.</param>
        /// <param name="options">The <see cref="DistributedCacheEntryOptions"/> for the item; note that only sliding expiration is currently supported.</param>
        /// <returns>The cache item if found, otherwise null.</returns>
        public static T Get<T>(this IDistributedCache cache, string key, DistributedCacheEntryOptions options)
        {
            if (key == null)
            {
                throw new ArgumentNullException(nameof(key));
            }

            var bucket = GetBucket(cache);

            return bucket.GetAndTouch<T>(key, GetLifetime(cache, options)).Value;
        }

        /// <summary>
        /// Gets a cache item by its key asynchronously, returning null if the item does not exist within the Cache.
        /// </summary>
        /// <param name="cache">The <see cref="CouchbaseCache"/> cache.</param>
        /// <param name="key">The key to lookup the item.</param>
        /// <param name="token"></param>
        /// <returns>The cache item if found, otherwise null.</returns>
        public static Task<T> GetAsync<T>(this IDistributedCache cache, string key, CancellationToken token)
        {
            return GetAsync<T>(cache, key, new DistributedCacheEntryOptions(), token);
        }

        /// <summary>
        /// Gets a cache item by its key asynchronously, returning null if the item does not exist within the Cache.
        /// </summary>
        /// <param name="cache">The <see cref="CouchbaseCache"/> cache.</param>
        /// <param name="key">The key to lookup the item.</param>
        /// <returns>The cache item if found, otherwise null.</returns>
        public static Task<T> GetAsync<T>(this IDistributedCache cache, string key)
        {
            return GetAsync<T>(cache, key, new DistributedCacheEntryOptions(), new CancellationToken());
        }

        /// <summary>
        /// Gets a cache item by its key asynchronously, returning null if the item does not exist within the Cache.
        /// </summary>
        /// <param name="cache">The <see cref="CouchbaseCache"/> cache.</param>
        /// <param name="key">The key to lookup the item.</param>
        /// <param name="options">The <see cref="DistributedCacheEntryOptions"/> for the item; note that only sliding expiration is currently supported.</param>
        /// <returns>The cache item if found, otherwise null.</returns>
        public static Task<T> GetAsync<T>(this IDistributedCache cache, string key, DistributedCacheEntryOptions options)
        {
            return GetAsync<T>(cache, key, options, new CancellationToken());
        }

        /// <summary>
        /// Gets a cache item by its key asynchronously, returning null if the item does not exist within the Cache.
        /// </summary>
        /// <param name="cache">The <see cref="CouchbaseCache"/> cache.</param>
        /// <param name="key">The key to lookup the item.</param>
        /// <param name="options">The <see cref="DistributedCacheEntryOptions"/> for the item; note that only sliding expiration is currently supported.</param>
        /// <param name="token"></param>
        /// <returns>The cache item if found, otherwise null.</returns>
        public static async Task<T> GetAsync<T>(this IDistributedCache cache, string key, DistributedCacheEntryOptions options, CancellationToken token)
        {
            token.ThrowIfCancellationRequested();
            if (key == null)
            {
                throw new ArgumentNullException(nameof(key));
            }

            var bucket = GetBucket(cache);
            return (await bucket.GetAndTouchAsync<T>(key, GetLifetime(cache, options))).Value;
        }

        /// <summary>
        /// Gets a cache item by its key, returning null if the item does not exist within the Cache.
        /// </summary>
        /// <param name="cache">The <see cref="CouchbaseCache"/> cache.</param>
        /// <param name="key">The key to lookup the item.</param>
        /// <param name="options">The <see cref="DistributedCacheEntryOptions"/> for the item; note that only sliding expiration is currently supported.</param>
        /// <returns>The cache item if found, otherwise null.</returns>
        public static byte[] Get(this IDistributedCache cache, string key, DistributedCacheEntryOptions options)
        {
            return Get<byte[]>(cache, key, options);
        }

        /// <summary>
        /// Gets a cache item by its key asynchronously, returning null if the item does not exist within the Cache.
        /// </summary>
        /// <param name="cache">The <see cref="CouchbaseCache"/> cache.</param>
        /// <param name="key">The key to lookup the item.</param>
        /// <param name="options">The <see cref="DistributedCacheEntryOptions"/> for the item; note that only sliding expiration is currently supported.</param>
        /// <returns>The cache item if found, otherwise null.</returns>
        public static Task<byte[]> GetAsync(this IDistributedCache cache, string key, DistributedCacheEntryOptions options)
        {
            return GetAsync<byte[]>(cache, key, options);
        }

        /// <summary>
        /// Gets the <see cref="IBucket"/> that the <see cref="CouchbaseCache"/> is using.
        /// </summary>
        /// <param name="cache"></param>
        /// <returns></returns>
        static IBucket GetBucket(IDistributedCache cache)
        {
            if (!(cache is ICouchbaseCache couchbaseCache))
            {
                throw new NotSupportedException("The IDistributedCache must be a CouchbaseCache.");
            }
            return couchbaseCache.Bucket;
        }

        /// <summary>
        /// Gets the lifetime of the cache iten as a <see cref="TimeSpan"/>.
        /// </summary>
        /// <param name="cache"></param>
        /// <param name="itemOptions"></param>
        /// <returns></returns>
        internal static TimeSpan GetLifetime(IDistributedCache cache, DistributedCacheEntryOptions itemOptions = null)
        {
            if (!(cache is ICouchbaseCache couchbaseCache))
            {
                throw new NotSupportedException("The IDistributedCache must be a CouchbaseCache.");
            }

            if (itemOptions?.SlidingExpiration != null)
            {
                return itemOptions.SlidingExpiration.Value;
            }

            if (itemOptions == null)
            {
                return CouchbaseCache.InfiniteLifetime;
            }

            return couchbaseCache.Options.Value.LifeSpan ?? CouchbaseCache.InfiniteLifetime;
        }
    }
}
