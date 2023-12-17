using Couchbase.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;

namespace Couchbase.Extensions.Caching.Internal
{
    internal class DefaultCouchbaseCacheCollectionProvider(
        ICouchbaseCacheBucketProvider bucketProvider,
        IOptions<CouchbaseCacheOptions> options)
        : NamedCollectionProvider(bucketProvider, options.Value.ScopeName, options.Value.CollectionName), ICouchbaseCacheCollectionProvider
    {
    }
}
