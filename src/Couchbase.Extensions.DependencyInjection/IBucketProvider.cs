using System;
using Couchbase.Configuration.Client;
using Couchbase.Core;

namespace Couchbase.Extensions.DependencyInjection
{
    /// <summary>
    /// Provides access to buckets for a Couchbase cluster.  Should maintain
    /// singleton instances of each bucket.  Consumers should not dispose the
    /// <see cref="IBucket"/> implementations.  Instead, this provider should be
    /// disposed during application shutdown using <see cref="ICouchbaseLifetimeService"/>.
    /// </summary>
    public interface IBucketProvider : IDisposable
    {
        /// <summary>
        /// Get a Couchbase bucket.
        /// </summary>
        /// <param name="bucketName">Name of the bucket.</param>
        /// <returns><see cref="IBucket"/> implementation for the given bucket name.</returns>
        /// <remarks>
        /// This implementation is for buckets without a password, or for cases where the bucket
        /// password is stored in the configuration.
        /// </remarks>
        IBucket GetBucket(string bucketName);

        /// <summary>
        /// Get a Couchbase bucket.
        /// </summary>
        /// <param name="bucketName">Name of the bucket.</param>
        /// <param name="password">Password to access the bucket.</param>
        /// <returns><see cref="IBucket"/> implementation for the given bucket name.</returns>
        /// <remarks>For Couchbase Server 5.0 Role Based Access Control (RBAC) do not use this overload;
        /// instead set the Username and Password at the root of the configuration in your JSON or App.Config
        /// or <see cref="ClientConfiguration"/> and call <see cref="IBucketProvider.GetBucket(string)"/>!</remarks>
        IBucket GetBucket(string bucketName, string password);
    }
}
