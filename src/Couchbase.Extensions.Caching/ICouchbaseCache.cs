using System.Threading.Tasks;
using System.Threading;
using Microsoft.Extensions.Caching.Distributed;

namespace Couchbase.Extensions.Caching
{
    /// <summary>
    /// Provides and interface for implementing a <see cref="IDistributedCache"/> using Couchbase server.
    /// </summary>
    /// <seealso cref="IDistributedCache" />
    public interface ICouchbaseCache : IDistributedCache
    {
        /// <summary>
        /// Gets a value with the given key.
        /// </summary>
        /// <typeparam name="T">Type of value.</typeparam>
        /// <param name="key">A string identifying the requested value.</param>
        /// <param name="token">The <see cref="CancellationToken"/> used to propagate notifications that the operation should be canceled.</param>
        /// <returns>The <see cref="Task"/> that represents the asynchronous operation, containing the located value or null/default.</returns>
        Task<T?> GetAsync<T>(string key, CancellationToken token = default);

        /// <summary>
        /// Sets a value with the given key.
        /// </summary>
        /// <typeparam name="T">Type of value.</typeparam>
        /// <param name="key">A string identifying the requested value.</param>
        /// <param name="value">The value to set in the cache.</param>
        /// <param name="options">The cache options for the value.</param>
        /// <param name="token">The <see cref="CancellationToken"/> used to propagate notifications that the operation should be canceled.</param>
        /// <returns>The <see cref="Task"/> that represents the asynchronous operation.</returns>
        Task SetAsync<T>(string key, T value, DistributedCacheEntryOptions options,
            CancellationToken token = default);
    }
}
