using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Threading;
using System.Threading.Tasks;
using Couchbase.Core.IO.Serializers;
using Couchbase.Extensions.Caching.Internal;
using Microsoft.Extensions.Caching.Hybrid;
using Microsoft.Extensions.DependencyInjection;

namespace Couchbase.Extensions.Caching
{
    /// <summary>
    /// Constructs <see cref="IHybridCacheSerializer{T}"/> instances based upon the configured type serializer
    /// from a Couchbase collection. The type serializer must implement <see cref="IBufferedTypeSerializer"/>.
    /// Only constructs serializers for types that the inner serializer can handle.
    /// </summary>
    /// <remarks>
    /// Most Couchbase serializers will at least try to serialize all types, so when this factory is registered
    /// any factories registered after it will not be used. However, some serializers, such as the System.Text.Json
    /// serializer built from a JsonSerializerContext, are smarter and will only serialize types that they know about.
    /// </remarks>
    public sealed class CouchbaseCacheSerializerFactory : IHybridCacheSerializerFactory
    {
        // Optionally used to acquire to inner serializer after bootstrapping.
        private readonly ICouchbaseCacheCollectionProvider? _collectionProvider;

        private int _initialized;
        private IBufferedTypeSerializer? _innerSerializer;

        /// <summary>
        /// Constructs a new <see cref="CouchbaseCacheSerializerFactory"/> that acquires the inner serializer
        /// from a Couchbase collection's configuration.
        /// </summary>
        /// <param name="collectionProvider">The Couchbase collection.</param>
        public CouchbaseCacheSerializerFactory(
            ICouchbaseCacheCollectionProvider collectionProvider)
        {
            _collectionProvider = collectionProvider;
        }

        /// <summary>
        /// Constructs a new <see cref="CouchbaseCacheSerializerFactory"/>.
        /// </summary>
        /// <param name="innerSerializer">The inner Couchbase serializer.</param>
        public CouchbaseCacheSerializerFactory(IBufferedTypeSerializer innerSerializer)
        {
            _innerSerializer = innerSerializer;
            _initialized = 1;
        }

        /// <inheritdoc />
        public bool TryCreateSerializer<T>([NotNullWhen(true)] out IHybridCacheSerializer<T>? serializer)
        {
            var innerSerializer = GetInnerSerializer();

            if (innerSerializer is null || !innerSerializer.CanSerialize(typeof(T)))
            {
                serializer = null;
                return false;
            }

            serializer = new CouchbaseHybridCacheSerializer<T>(innerSerializer);
            return true;
        }

        private IBufferedTypeSerializer? GetInnerSerializer()
        {
            if (Volatile.Read(ref _initialized) > 0)
            {
                return Volatile.Read(ref _innerSerializer);
            }

            // The need for GetAwaiter().GetResult() is unfortunate but necessary because Couchbase.Extensions.DI
            // doesn't currently have a method to get to the ITypeSerializer until after aysnc bootstrapping.
            var result = Interlocked.CompareExchange(ref _innerSerializer, GetFromCollectionProvider().GetAwaiter().GetResult(), null)
                ?? _innerSerializer;
            Volatile.Write(ref _initialized, 1);

            return result;

            async Task<IBufferedTypeSerializer?> GetFromCollectionProvider()
            {
                Debug.Assert(_collectionProvider is not null);

                var collection = await _collectionProvider.GetCollectionAsync().ConfigureAwait(false);
                var serializer = collection.Scope.Bucket.Cluster.ClusterServices.GetRequiredService<ITypeSerializer>();
                return serializer as IBufferedTypeSerializer;
            }
        }
    }
}
