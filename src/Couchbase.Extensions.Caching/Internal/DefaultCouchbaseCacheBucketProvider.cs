using Couchbase.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;

namespace Couchbase.Extensions.Caching.Internal
{
    internal class DefaultCouchbaseCacheBucketProvider(
        IBucketProvider bucketProvider,
        IOptions<CouchbaseCacheOptions> options)
        : NamedBucketProvider(bucketProvider, options.Value.BucketName), ICouchbaseCacheBucketProvider
    {
    }
}
