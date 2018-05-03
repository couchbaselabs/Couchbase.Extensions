using System;
using Couchbase.Core.Snappy;
using Snappy;

namespace Couchbase.Extensions.Compression
{
    public class SnappyCompressor : ISnappyCompressor
    {
        public int MinSize { get; set; }
        public double MinRatio { get; set; }

        public SnappyCompressor(int minSize, double minRatio)
        {
            if (minSize <= 0) throw new ArgumentOutOfRangeException(nameof(minSize));
            if (minRatio <= 0) throw new ArgumentOutOfRangeException(nameof(minRatio));

            MinSize = minSize;
            MinRatio = minRatio;
        }

        public byte[] Compress(byte[] buffer, int offset, int length)
        {
            if (length > MinSize)
            {
                var compressed = new byte[SnappyCodec.GetMaxCompressedLength(length)];
                var compressedLength = SnappyCodec.Compress(buffer, offset, length, compressed, 0);

                if (MinRatio > (double) compressedLength / length)
                {
                    Array.Resize(ref compressed, compressedLength);
                    return compressed;
                }
            }

            return null;
        }

        public byte[] Uncompress(byte[] buffer, int offset, int length)
        {
            var uncompressed = new byte[SnappyCodec.GetUncompressedLength(buffer, offset, length)];
            var uncompressedLength = SnappyCodec.Uncompress(buffer, offset, length, uncompressed, 0);

            if (uncompressed.Length < uncompressedLength)
            {
                Array.Resize(ref uncompressed, uncompressedLength);
            }

            return uncompressed;
        }
    }
}
