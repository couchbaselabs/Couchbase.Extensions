using System;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using Couchbase.Core.IO.Serializers;

namespace Couchbase.Extensions.Caching.Internal
{
    /// <summary>
    /// Wraps the consumer's <see cref="ITypeSerializer"/> with a serializer for <see cref="CacheMetadata"/> objects.
    /// <see cref="CacheMetadata"/> is serialized using <see cref="SystemTextJsonSerializer"/> while all other types
    /// are forwarded to the inner serializer.
    /// </summary>
    /// <param name="innerSerializer"></param>
    internal sealed class CacheSerializer(ITypeSerializer innerSerializer) : ITypeSerializer
    {
        public static SystemTextJsonSerializer XAttrSerializer { get; } =
            SystemTextJsonSerializer.Create(CacheSerializerContext.Default);

        public T? Deserialize<T>(ReadOnlyMemory<byte> buffer)
        {
            if (typeof(T) == typeof(CacheMetadata))
            {
                return (T?)(object?) XAttrSerializer.Deserialize<CacheMetadata>(buffer);
            }

            return innerSerializer.Deserialize<T>(buffer);
        }

        public T? Deserialize<T>(Stream stream)
        {
            if (typeof(T) == typeof(CacheMetadata))
            {
                return (T?)(object?) XAttrSerializer.Deserialize<CacheMetadata>(stream);
            }

            return innerSerializer.Deserialize<T>(stream);
        }

        public async ValueTask<T?> DeserializeAsync<T>(Stream stream, CancellationToken cancellationToken = default)
        {
            if (typeof(T) == typeof(CacheMetadata))
            {
                return (T?)(object?) await XAttrSerializer.DeserializeAsync<CacheMetadata>(stream, cancellationToken);
            }

            return await innerSerializer.DeserializeAsync<T>(stream, cancellationToken);
        }

        public void Serialize(Stream stream, object? obj)
        {
            if (obj is CacheMetadata cacheInfo)
            {
                XAttrSerializer.Serialize(stream, cacheInfo);
                return;
            }

            innerSerializer.Serialize(stream, obj);
        }

        public ValueTask SerializeAsync(Stream stream, object? obj, CancellationToken cancellationToken = default)
        {
            if (obj is CacheMetadata cacheInfo)
            {
                return XAttrSerializer.SerializeAsync(stream, cacheInfo, cancellationToken);
            }

            return innerSerializer.SerializeAsync(stream, obj, cancellationToken);
        }
    }
}
