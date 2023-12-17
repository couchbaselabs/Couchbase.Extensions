using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Couchbase.Core.Exceptions.KeyValue;
using Couchbase.Core.IO.Transcoders;
using Couchbase.KeyValue;
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;

namespace Couchbase.Extensions.Caching.Internal
{
    /// <inheritdoc />
    internal class CouchbaseCache : ICouchbaseCache
    {
        private const string CacheMetadataKey = "cache_config";

        private readonly ICouchbaseCacheCollectionProvider _collectionProvider;
        private readonly TimeProvider _timeProvider;
        private readonly TimeSpan _defaultSlidingExpiration;
        private ITypeTranscoder? _transcoder;

        // For unit testing allow injecting a time provider
        internal CouchbaseCache(ICouchbaseCacheCollectionProvider collectionProvider, IOptions<CouchbaseCacheOptions> options,
            TimeProvider? timeProvider)
        {
            ArgumentNullException.ThrowIfNull(collectionProvider);
            ArgumentNullException.ThrowIfNull(options);

            var cacheOptions = options.Value;

            if (cacheOptions.DefaultSlidingExpiration <= TimeSpan.Zero)
            {
                throw new ArgumentOutOfRangeException(
                    nameof(cacheOptions.DefaultSlidingExpiration),
                    cacheOptions.DefaultSlidingExpiration,
                    "The sliding expiration value must be positive.");
            }

            _collectionProvider = collectionProvider;
            _timeProvider = timeProvider ?? TimeProvider.System;
            _defaultSlidingExpiration = cacheOptions.DefaultSlidingExpiration;
        }

        public CouchbaseCache(ICouchbaseCacheCollectionProvider collectionProvider, IOptions<CouchbaseCacheOptions> options)
            : this(collectionProvider, options, timeProvider: null)
        {
        }

        /// <inheritdoc />
        public async Task<T?> GetAsync<T>(string key, CancellationToken token = default)
        {
            ArgumentNullException.ThrowIfNull(key);
            token.ThrowIfCancellationRequested();

            try
            {
                var collection = await _collectionProvider.GetCollectionAsync().ConfigureAwait(false);
                using var result = await collection
                    .LookupInAsync(key,
                        builder =>
                        {
                            builder.GetFull();
                            builder.Get(CacheMetadataKey, true);
                        },
                        new LookupInOptions().Transcoder(GetOrCreateTranscoder(collection)).CancellationToken(token))
                    .ConfigureAwait(false);

                if (result.Exists(1))
                {
                    var cacheMetadata = result.ContentAs<CacheMetadata>(1);

                    // Push out the sliding expiration
                    // Touch in the background, don't wait for completion and don't pass cancellation token
                    _ = TouchAsync(collection, key, cacheMetadata, token: default);
                }

                return result.ContentAs<T>(0);
            }
            catch (DocumentNotFoundException)
            {
                return default;
            }
        }

        /// <inheritdoc />
        public byte[]? Get(string key) =>
            GetAsync<byte[]>(key).GetAwaiter().GetResult();

        /// <inheritdoc />
        public Task<byte[]?> GetAsync(string key, CancellationToken cancellationToken = default) =>
            GetAsync<byte[]>(key, cancellationToken);

        /// <inheritdoc />
        public async Task SetAsync<T>(string key, T value, DistributedCacheEntryOptions options,
            CancellationToken token = default)
        {
            ArgumentNullException.ThrowIfNull(key);
            ArgumentNullException.ThrowIfNull(value);
            ArgumentNullException.ThrowIfNull(options);
            token.ThrowIfCancellationRequested();

            if (options is { AbsoluteExpiration: null, AbsoluteExpirationRelativeToNow: null, SlidingExpiration: null })
            {
                // Apply the default sliding expiration. We explicity throw out other options in this scenario.
                options = new DistributedCacheEntryOptions()
                {
                    SlidingExpiration = _defaultSlidingExpiration
                };
            }

            var collection = await _collectionProvider.GetCollectionAsync().ConfigureAwait(false);

            var utcNow = _timeProvider.GetUtcNow();
            var cacheMetadata = CacheMetadata.Create(options, utcNow);

            List<MutateInSpec> specs =
            [
                MutateInSpec.SetDoc(value),
                MutateInSpec.Upsert(CacheMetadataKey, cacheMetadata, isXattr: true),
            ];

            var mutateInOptions = new MutateInOptions()
                .CancellationToken(token)
                .Transcoder(GetOrCreateTranscoder(collection))
                .StoreSemantics(StoreSemantics.Upsert)
                .Expiry(cacheMetadata.GetRelativeExpiration(utcNow));

            using var _ = await collection.MutateInAsync(key, specs, mutateInOptions)
                .ConfigureAwait(false);
        }

        /// <inheritdoc />
        public void Set(string key, byte[] value, DistributedCacheEntryOptions options) =>
            SetAsync<byte[]>(key, value, options).GetAwaiter().GetResult();

        /// <inheritdoc />
        public Task SetAsync(string key, byte[] value, DistributedCacheEntryOptions options,
            CancellationToken cancellationToken = default) =>
            SetAsync<byte[]>(key, value, options, cancellationToken);

        /// <inheritdoc />
        public void Refresh(string key) =>
            RefreshAsync(key).GetAwaiter().GetResult();

        /// <inheritdoc />
        public async Task RefreshAsync(string key, CancellationToken token = default)
        {
            ArgumentNullException.ThrowIfNull(key);
            token.ThrowIfCancellationRequested();

            try
            {
                var collection = await _collectionProvider.GetCollectionAsync().ConfigureAwait(false);

                List<LookupInSpec> specs =
                [
                    LookupInSpec.Get(CacheMetadataKey, isXattr: true),
                ];

                using var result = await collection
                    .LookupInAsync(key, specs,
                        new LookupInOptions()
                            .Transcoder(GetOrCreateTranscoder(collection))
                            .CancellationToken(token))
                    .ConfigureAwait(false);

                if (result.Exists(0))
                {
                    var cacheMetadata = result.ContentAs<CacheMetadata>(0);

                    // Push out the sliding expiration
                    await TouchAsync(collection, key, cacheMetadata, token)
                        .ConfigureAwait(false);
                }
            }
            catch (DocumentNotFoundException)
            {
                // Ignore
            }
        }

        /// <inheritdoc />
        public void Remove(string key) =>
            RemoveAsync(key).GetAwaiter().GetResult();

        /// <inheritdoc />
        public async Task RemoveAsync(string key, CancellationToken token = default)
        {
            ArgumentNullException.ThrowIfNull(key);
            token.ThrowIfCancellationRequested();

            try
            {
                var collection = await _collectionProvider.GetCollectionAsync().ConfigureAwait(false);

                // Avoid the heap allocation if we can't be canceled
                var removeOptions = token.CanBeCanceled
                    ? new RemoveOptions().CancellationToken(token)
                    : null;

                await collection.RemoveAsync(key, removeOptions).ConfigureAwait(false);
            }
            catch (DocumentNotFoundException)
            {
                // Ignore
            }
        }

        private async Task TouchAsync(ICouchbaseCollection collection, string key, CacheMetadata? cacheMetadata,
            CancellationToken token = default)
        {
            // If sliding expiration is set, touch the item on get to push out the expiration

            var expiration = cacheMetadata?.GetRelativeExpiration(_timeProvider.GetUtcNow());
            if (expiration is null)
            {
                return;
            }

            if (expiration.GetValueOrDefault() <= TimeSpan.Zero)
            {
                // We've expired per the XATTR, remove the document instead of pushing expiration
                await RemoveAsync(key, token).ConfigureAwait(false);
                return;
            }

            // Avoid the heap allocation if we can't be canceled
            var touchOptions = token.CanBeCanceled
                ? new TouchOptions().CancellationToken(token)
                : null;

            try
            {
                await collection.TouchAsync(key, expiration.GetValueOrDefault(), touchOptions).ConfigureAwait(false);
            }
            catch (DocumentNotFoundException)
            {
                // Ignore
            }
        }

        private ITypeTranscoder GetOrCreateTranscoder(ICouchbaseCollection collection)
        {
            var transcoder = _transcoder;
            if (transcoder is not null)
            {
                // Fast path once initialized
                return transcoder;
            }

            // Make a new transcoder and swap it in, but don't overwrite and return the current value if another
            // thread has set it already.
            transcoder = new CacheTranscoder(
                collection.Scope.Bucket.Cluster.ClusterServices.GetRequiredService<ITypeTranscoder>());
            return Interlocked.CompareExchange(ref _transcoder, transcoder, null) ?? transcoder;
        }
    }
}
