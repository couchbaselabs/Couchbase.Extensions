using System;
using System.Threading;
using System.Threading.Tasks;
using Couchbase.Extensions.Locks.Internal;
using Couchbase.KeyValue;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace Couchbase.Extensions.Locks
{
    /// <summary>
    /// Distributed mutex extensions for <see cref="ICouchbaseCollection"/>.
    /// </summary>
    public static class LocksCollectionExtensions
    {
        /// <summary>
        /// Request a distributed Couchbase Mutex that expires after <paramref name="expiration"/>.
        /// </summary>
        /// <remarks>
        /// Uses the default holder from <see cref="LockHolder"/>.
        ///
        /// The <see cref="ICouchbaseMutex"/> should be disposed once the lock is no longer needed.
        /// </remarks>
        /// <param name="collection">Couchbase collection.</param>
        /// <param name="name">Name of the lock.</param>
        /// <param name="expiration">Time until mutex expires, if not renewed.</param>
        /// <param name="cancellationToken">Cancellation token.</param>
        /// <returns>The <see cref="ICouchbaseMutex"/>.</returns>
        /// <exception cref="CouchbaseLockUnavailableException">Thrown if the lock could not be acquired.</exception>
        /// <exception cref="CouchbaseException">Thrown on general Couchbase communication errors.</exception>
        /// <exception cref="ArgumentNullException">Thrown if bucket is null.</exception>
        /// <exception cref="ArgumentException">Thrown for invalid name.</exception>
        /// <exception cref="ArgumentOutOfRangeException">Thrown for non-positive expiration.</exception>
        public static Task<ICouchbaseMutex> RequestMutexAsync(this ICouchbaseCollection collection, string name, TimeSpan expiration,
            CancellationToken cancellationToken = default)
        {
            return collection.RequestMutexAsync(name, LockHolder.Default, expiration, cancellationToken);
        }

        /// <summary>
        /// Request a distributed Couchbase Mutex that expires after <paramref name="expiration"/>.
        /// </summary>
        /// <remarks>
        /// The <see cref="ICouchbaseMutex"/> should be disposed once the lock is no longer needed.
        /// </remarks>
        /// <param name="collection">Couchbase collection.</param>
        /// <param name="name">Name of the lock.</param>
        /// <param name="holder">Useful identifier for who is holding the lock.</param>
        /// <param name="expiration">Time until mutex expires, if not renewed.</param>
        /// <param name="cancellationToken">Cancellation token.</param>
        /// <returns>The <see cref="ICouchbaseMutex"/>.</returns>
        /// <exception cref="CouchbaseLockUnavailableException">Thrown if the lock could not be acquired.</exception>
        /// <exception cref="CouchbaseException">Thrown on general Couchbase communication errors.</exception>
        /// <exception cref="ArgumentNullException">Thrown if bucket is null.</exception>
        /// <exception cref="ArgumentException">Thrown for invalid name or holder.</exception>
        /// <exception cref="ArgumentOutOfRangeException">Thrown for non-positive expiration.</exception>
        public static async Task<ICouchbaseMutex> RequestMutexAsync(this ICouchbaseCollection collection, string name, string holder, TimeSpan expiration,
            CancellationToken cancellationToken = default)
        {
            if (collection == null)
            {
                throw new ArgumentNullException(nameof(collection));
            }
            if (string.IsNullOrEmpty(name))
            {
                throw new ArgumentException("Value cannot be null or empty.", nameof(name));
            }
            if (string.IsNullOrEmpty(holder))
            {
                throw new ArgumentException("Value cannot be null or empty.", nameof(holder));
            }
            if (expiration <= TimeSpan.Zero)
            {
                throw new ArgumentOutOfRangeException(nameof(expiration), "Value must be positive.");
            }

            var logger = collection.Scope.Bucket.Cluster.ClusterServices.GetRequiredService<ILogger<CouchbaseMutex>>();

            var lockObj = new CouchbaseMutex(collection, name, holder, logger);

            // This will throw if it fails to create the initial lock
            await lockObj.Renew(expiration, cancellationToken).ConfigureAwait(false);

            return lockObj;
        }
    }
}
