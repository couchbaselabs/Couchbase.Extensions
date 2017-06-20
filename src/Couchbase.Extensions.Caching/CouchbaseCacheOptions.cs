using System;
using Couchbase.Configuration.Client;
using Couchbase.Core;
using Microsoft.Extensions.Options;

namespace Couchbase.Extensions.Caching
{
    /// <summary>
    /// Options for <see cref="CouchbaseCache"/>. Note that if <see cref="Bucket"/> is empty the <see cref="ClusterHelper.GetBucket(string)"/>
    /// will be used to create the bucket.
    /// </summary>
    public class CouchbaseCacheOptions : IOptions<CouchbaseCacheOptions>
    {
        /// <summary>
        /// The current <see cref="CouchbaseCacheOptions"/> instance.
        /// </summary>
        public CouchbaseCacheOptions Value => this;

        /// <summary>
        /// The global lifespan for cache items.
        /// </summary>
        public TimeSpan? LifeSpan { get; set; }
    }
}
