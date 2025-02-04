using System;

namespace Couchbase.Extensions.Caching.Internal
{
    /// <summary>
    /// Wrapper used for swapping buffers through the <see cref="CacheTranscoder"/>. It is used
    /// instead of directly using <see cref="ReadOnlyMemory{T}"/> to ensure there can be no confusion
    /// with behaviors in the transcoder being wrapped by the <see cref="CacheTranscoder"/>.
    /// </summary>
    internal readonly struct CacheBuffer(ReadOnlyMemory<byte> data)
    {
        /// <summary>
        /// Data buffer from the operation. Only valid until the operation response is disposed.
        /// </summary>
        public ReadOnlyMemory<byte> Data { get; } = data;
    }
}
