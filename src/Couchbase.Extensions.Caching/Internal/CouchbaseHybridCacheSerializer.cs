using System.Buffers;
using Couchbase.Core.IO.Serializers;
using Microsoft.Extensions.Caching.Hybrid;

namespace Couchbase.Extensions.Caching.Internal
{
    /// <summary>
    /// Wraps an <see cref="IBufferedTypeSerializer"/> in a <see cref="IHybridCacheSerializer{T}"/>.
    /// </summary>
    /// <typeparam name="T">Type to serializer or deserialize.</typeparam>
    /// <param name="serializer">The inner <see cref="IBufferedTypeSerializer"/>.</param>
    internal sealed class CouchbaseHybridCacheSerializer<T>(IBufferedTypeSerializer serializer) : IHybridCacheSerializer<T>
    {
        /// <inheritdoc />
        public T Deserialize(ReadOnlySequence<byte> source) => serializer.Deserialize<T>(source)!;

        /// <inheritdoc />
        public void Serialize(T value, IBufferWriter<byte> target) => serializer.Serialize(target, value);
    }
}
