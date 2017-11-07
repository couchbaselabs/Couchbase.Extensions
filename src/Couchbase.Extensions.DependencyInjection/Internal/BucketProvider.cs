using System;
using System.Collections.Concurrent;
using System.Linq;
using Couchbase.Core;

namespace Couchbase.Extensions.DependencyInjection.Internal
{
    internal class BucketProvider : IBucketProvider
    {
        private readonly IClusterProvider _clusterProvider;
        private readonly ConcurrentDictionary<string, IBucket> _buckets = new ConcurrentDictionary<string, IBucket>();
        private bool _disposed = false;

        public BucketProvider(IClusterProvider clusterProvider)
        {
            _clusterProvider = clusterProvider ?? throw new ArgumentNullException(nameof(clusterProvider));
        }

        /// <inheritdoc />
        public IBucket GetBucket(string bucketName)
        {
            return GetBucket(bucketName, null);
        }

        /// <inheritdoc />
        public IBucket GetBucket(string bucketName, string password)
        {
            if (_disposed)
            {
                throw new ObjectDisposedException(nameof(BucketProvider));
            }
            if (bucketName == null)
            {
                throw new ArgumentNullException(nameof(bucketName));
            }

            return _buckets.GetOrAdd(bucketName, name =>
            {
                var cluster = _clusterProvider.GetCluster();

                if (string.IsNullOrWhiteSpace(password))
                {
                    //try to find a password in configuration
                    if (cluster.Configuration.BucketConfigs.TryGetValue(name, out var bucketConfig)
                        && bucketConfig.Password != null)
                    {
                        return cluster.OpenBucket(name, bucketConfig.Password);
                    }

                    return cluster.OpenBucket(name);
                }

                return cluster.OpenBucket(name, password);
            });
        }

        public void Dispose()
        {
            if (!_disposed)
            {
                _disposed = true;

                var bucketCache = _buckets.Values.ToList();
                _buckets.Clear();

                foreach (var bucket in bucketCache)
                {
                    bucket.Dispose();
                }
            }
        }
    }
}
