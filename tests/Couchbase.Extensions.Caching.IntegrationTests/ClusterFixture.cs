using System.Threading.Tasks;
using Couchbase.Compression.Snappier;
using Couchbase.Extensions.DependencyInjection;
using Couchbase.KeyValue;
using Microsoft.Extensions.Configuration;
using Xunit;

namespace Couchbase.Extensions.Caching.IntegrationTests
{
    public class ClusterFixture : IAsyncLifetime, ICouchbaseCacheBucketProvider, ICouchbaseCacheCollectionProvider
    {
        private bool _bucketOpened;

        public ClusterOptions ClusterOptions { get; }

        public ICluster Cluster { get; private set; }

        public string ScopeName => throw new System.NotImplementedException();

        public string CollectionName => throw new System.NotImplementedException();

        public ClusterFixture()
        {
            ClusterOptions = GetClusterOptions();
        }

        public async ValueTask<ICluster> GetClusterAsync()
        {
            if (_bucketOpened)
            {
                return Cluster;
            }

            await GetDefaultBucketAsync().ConfigureAwait(false);
            return Cluster;
        }

        public async Task<IBucket> GetDefaultBucketAsync()
        {
            var bucket = await Cluster.BucketAsync("default").ConfigureAwait(false);

            _bucketOpened = true;

            return bucket;
        }

        public async Task<ICouchbaseCollection> GetDefaultCollectionAsync()
        {
            var bucket = await GetDefaultBucketAsync().ConfigureAwait(false);
            return bucket.DefaultCollection();
        }

        public static ClusterOptions GetClusterOptions()
        {
            return new ConfigurationBuilder()
                .AddJsonFile("config.json")
                .Build()
                .GetSection("couchbase")
                .Get<ClusterOptions>()
                .WithSnappyCompression();
        }

        public async Task InitializeAsync()
        {
            Cluster = await Couchbase.Cluster.ConnectAsync(GetClusterOptions())
                .ConfigureAwait(false);
        }

        public Task DisposeAsync()
        {
            Cluster?.Dispose();

            return Task.CompletedTask;
        }

        string INamedBucketProvider.BucketName => "default";

        ValueTask<IBucket> INamedBucketProvider.GetBucketAsync() =>
            new(GetDefaultBucketAsync());

        ValueTask<ICouchbaseCollection> INamedCollectionProvider.GetCollectionAsync() =>
            new(GetDefaultCollectionAsync());
    }
}