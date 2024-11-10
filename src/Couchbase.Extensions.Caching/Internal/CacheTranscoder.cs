using System;
using System.Buffers;
using System.IO;
using Couchbase.Core.IO.Operations;
using Couchbase.Core.IO.Serializers;
using Couchbase.Core.IO.Transcoders;

namespace Couchbase.Extensions.Caching.Internal
{
    /// <summary>
    /// Wraps the consumer's <see cref="ITypeTranscoder"/> with a transcoder for <see cref="CacheMetadata"/> objects
    /// and byte arrays. All other types are forwarded to the inner transcoder.
    /// </summary>
    internal class CacheTranscoder(
        ITypeTranscoder innerTranscoder)
        : ITypeTranscoder
    {
        // If the inner transcoder has a serializer, wrap it in a CacheSerializer
        private readonly ITypeSerializer _serializer = innerTranscoder.Serializer is not null
            ? new CacheSerializer(innerTranscoder.Serializer)
            : CacheSerializer.XAttrSerializer;

        public ITypeSerializer? Serializer
        {
            get => _serializer;
            set => throw new NotSupportedException();
        }

        public Flags GetFormat<T>(T value)
        {
            if (typeof(T) == typeof(byte[]))
            {
                return new Flags
                {
                    DataFormat = DataFormat.Binary,
                    TypeCode = TypeCode.Object
                };
            }

            if (typeof(T) == typeof(CacheMetadata))
            {
                return new Flags
                {
                    DataFormat = DataFormat.Json,
                    TypeCode = TypeCode.Object
                };
            }

            return innerTranscoder.GetFormat(value);
        }

        public void Encode<T>(Stream stream, T value, Flags flags, OpCode opcode)
        {
            // This syntax supports JIT eliding for value types
            if (typeof(T) == typeof(ReadOnlySequence<byte>))
            {
                var sequence = (ReadOnlySequence<byte>)(object)value!;
                foreach (var memory in sequence)
                {
                    stream.Write(memory.Span);
                }
            }

            if (value is byte[] bytes)
            {
                stream.Write(bytes, 0, bytes.Length);
            }
            else if (value is CacheMetadata cacheInfo)
            {
                Serializer!.Serialize(stream, cacheInfo);
            }
            else
            {
                innerTranscoder.Encode(stream, value, flags, opcode);
            }
        }

        public T? Decode<T>(ReadOnlyMemory<byte> buffer, Flags flags, OpCode opcode)
        {
            // This syntax supports JIT eliding for value types
            if (typeof(T) == typeof(CacheBuffer))
            {
                // Special handling for IBufferDistributedCache, the returned sequence
                // will only be valid until the returned ILookupInResponse is disposed.
                return (T)(object)new CacheBuffer(buffer);
            }

            if (typeof(T) == typeof(byte[]))
            {
                return (T)(object)buffer.ToArray();
            }

            if (typeof(T) == typeof(CacheMetadata))
            {
                return Serializer!.Deserialize<T>(buffer);
            }

            return innerTranscoder.Decode<T>(buffer, flags, opcode);
        }
    }
}
