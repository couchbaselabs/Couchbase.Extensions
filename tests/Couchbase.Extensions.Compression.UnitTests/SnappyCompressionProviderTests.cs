using System.IO;
using System.Reflection;
using System.Text;
using Xunit;

namespace Couchbase.Extensions.Compression.UnitTests
{
    public class SnappyCompressionProviderTests
    {
        [Fact]
        public void Can_compress_and_decompress()
        {
            string value;
            using (var stream = typeof(SnappyCompressionProviderTests).GetTypeInfo().Assembly.GetManifestResourceStream("Couchbase.Extensions.Compression.UnitTests.Data.txt"))
            using (var reader = new StreamReader(stream))
            {
                value = reader.ReadToEnd();
            }

            var bytes = Encoding.UTF8.GetBytes(value);

            var provider = new SnappyCompressor(32, 0.83);

            var compressed = provider.Compress(bytes, 0, bytes.Length);
            Assert.NotEqual(bytes, compressed);

            var uncompressed = provider.Uncompress(compressed, 0, compressed.Length);
            Assert.Equal(bytes, uncompressed);
        }

        [Fact]
        public void Compress_returns_null_if_under_min_size()
        {
            const string value = "hello world";
            var bytes = Encoding.UTF8.GetBytes(value);

            var compressor = new SnappyCompressor(32, 0.83);
            Assert.Null(compressor.Compress(bytes, 0, bytes.Length));
        }

        [Fact]
        public void Compress_returns_null_if_under_min_ratio()
        {
            const string value = "hello world";
            var bytes = Encoding.UTF8.GetBytes(value);

            var compressor = new SnappyCompressor(5, 0.83);
            Assert.Null(compressor.Compress(bytes, 0, bytes.Length));
        }
    }
}
