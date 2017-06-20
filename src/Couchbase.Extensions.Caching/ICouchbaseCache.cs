using System;
using Couchbase.Core;
using Microsoft.Extensions.Caching.Distributed;

namespace Couchbase.Extensions.Caching
{
    /// <summary>
    /// Provides and interface for implementing a <see cref="IDistributedCache"/> using Couchbase server.
    /// </summary>
    /// <seealso cref="Microsoft.Extensions.Caching.Distributed.IDistributedCache" />
    public interface ICouchbaseCache : IDistributedCache
    {
        /// <summary>
        /// Gets the Couchbase bucket as the backing store.
        /// </summary>
        /// <value>
        /// The bucket.
        /// </value>
        IBucket Bucket { get; }

        /// <summary>
        /// Gets the options used by the Cache.
        /// </summary>
        /// <value>
        /// The options.
        /// </value>
        CouchbaseCacheOptions Options { get; }

        /// <summary>
        /// Gets the lifetime or expiration from the <see cref="DistributedCacheEntryOptions"/>. Only
        /// sliding expiration is currently supported. If <see cref="DistributedCacheEntryOptions.SlidingExpiration"/>
        /// if not set, then the <see cref="CouchbaseCacheOptions.LifeSpan"/> will be used. If it is empty then the
        /// default lifespan of zero (0) will be used which is infinite expiration.
        /// </summary>
        /// <param name="options"></param>
        /// <returns></returns>
        TimeSpan GetLifetime(DistributedCacheEntryOptions options = null);
    }
}
