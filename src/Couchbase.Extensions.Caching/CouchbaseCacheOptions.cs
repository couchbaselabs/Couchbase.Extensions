using System;
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Options;

namespace Couchbase.Extensions.Caching
{
    /// <summary>
    /// Options related to the Couchbase implementation of <see cref="IDistributedCache"/>.
    /// </summary>
    public class CouchbaseCacheOptions : IOptions<CouchbaseCacheOptions>
    {
        /// <summary>
        /// Name of the bucket to use for the cache. Defaults to "cache".
        /// </summary>
        public string BucketName { get; set; } = "cache";

        /// <summary>
        /// Name of the scope to use for the cache. Defaults to the default scope.
        /// </summary>
        public string ScopeName { get; set; } = "_default";

        /// <summary>
        /// Name of the collection to use for the cache. Defaults to the default collection.
        /// </summary>
        public string CollectionName { get; set; } = "_default";

        /// <summary>
        /// The default sliding expiration set for a cache entry if neither Absolute or SlidingExpiration has been set explicitly.
        /// By default, its 20 minutes.
        /// </summary>
        public TimeSpan DefaultSlidingExpiration { get; set; } = TimeSpan.FromMinutes(20);

        // Helper method to simply pass in a raw CouchbaseCacheOptions.
        CouchbaseCacheOptions IOptions<CouchbaseCacheOptions>.Value => this;
    }
}
