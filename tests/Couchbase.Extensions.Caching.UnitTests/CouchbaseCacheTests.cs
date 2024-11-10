using System;
using System.Buffers;
using System.Collections.Generic;
using System.Threading.Tasks;
using Couchbase.Core.Exceptions.KeyValue;
using Couchbase.Core.IO.Transcoders;
using Couchbase.Extensions.Caching.Internal;
using Couchbase.KeyValue;
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Time.Testing;
using Moq;
using Xunit;

namespace Couchbase.Extensions.Caching.UnitTests
{
    public class CouchbaseCacheTests
    {
        #region Set

        [Fact]
        public void Set_WhenKeyIsNull_ThrowArgumentNullException()
        {
            var provider = new Mock<ICouchbaseCacheCollectionProvider>();

            var cache = new CouchbaseCache(provider.Object, new CouchbaseCacheOptions());

            Assert.Throws<ArgumentNullException>(() => cache.Set(null, Array.Empty<byte>(), null));
        }

        [Fact]
        public async Task SetAsync_WhenKeyIsNull_ThrowArgumentNullException()
        {
            var provider = new Mock<ICouchbaseCacheCollectionProvider>();

            var cache = new CouchbaseCache(provider.Object, new CouchbaseCacheOptions());

            await Assert.ThrowsAsync<ArgumentNullException>(async () => await cache.SetAsync(null, Array.Empty<byte>(), null));
        }

        [Fact]
        public async Task SetAsync_AlreadyExpired_InvalidOperationException()
        {
            var collection = new Mock<ICouchbaseCollection>();
            collection
                .Setup(m => m.Scope.Bucket.Cluster.ClusterServices.GetService(typeof(ITypeTranscoder)))
                .Returns(new JsonTranscoder());

            var provider = new Mock<ICouchbaseCacheCollectionProvider>();
            provider
                .Setup(m => m.GetCollectionAsync())
                .ReturnsAsync(collection.Object);

            var timeProvider = new FakeTimeProvider()
            {
                AutoAdvanceAmount = TimeSpan.FromSeconds(1)
            };

            var cache = new CouchbaseCache(provider.Object, new CouchbaseCacheOptions(), timeProvider);

            await Assert.ThrowsAsync<InvalidOperationException>(() => cache.SetAsync("thekey", Array.Empty<byte>(), new DistributedCacheEntryOptions()
            {
                AbsoluteExpiration = timeProvider.GetUtcNow()
            }));
        }

        #endregion

        [Fact]
        public void Get_WhenKeyIsNull_ThrowArgumentNullException()
        {
            var provider = new Mock<ICouchbaseCacheCollectionProvider>();

            var cache = new CouchbaseCache(provider.Object, new CouchbaseCacheOptions());

            Assert.Throws<ArgumentNullException>(() => cache.Get(null));
        }

        [Fact]
        public async Task GetAsync_WhenKeyIsNull_ThrowArgumentNullException()
        {
            var provider = new Mock<ICouchbaseCacheCollectionProvider>();

            var cache = new CouchbaseCache(provider.Object, new CouchbaseCacheOptions());

            await Assert.ThrowsAsync<ArgumentNullException>(async () => await cache.GetAsync(null));
        }

        [Fact]
        public async Task GetAsync_DocumentNotFound_ReturnsNull()
        {
            var collection = new Mock<ICouchbaseCollection>();
            collection
                .Setup(m => m.LookupInAsync(It.IsAny<string>(), It.IsAny<IEnumerable<LookupInSpec>>(), It.IsAny<LookupInOptions>()))
                .ThrowsAsync(new DocumentNotFoundException());
            collection
                .Setup(m => m.Scope.Bucket.Cluster.ClusterServices.GetService(typeof(ITypeTranscoder)))
                .Returns(new JsonTranscoder());

            var provider = new Mock<ICouchbaseCacheCollectionProvider>();
            provider
                .Setup(m => m.GetCollectionAsync())
                .ReturnsAsync(collection.Object);

            var cache = new CouchbaseCache(provider.Object, new CouchbaseCacheOptions());

            var result = await cache.GetAsync("key");

            Assert.Null(result);
        }

        [Fact]
        public void Refresh_WhenKeyIsNull_ThrowArgumentNullException()
        {
            var provider = new Mock<ICouchbaseCacheCollectionProvider>();

            var cache = new CouchbaseCache(provider.Object, new CouchbaseCacheOptions());

            Assert.Throws<ArgumentNullException>(() => cache.Refresh(null));
        }

        [Fact]
        public async Task RefreshAsync_WhenKeyIsNull_ThrowArgumentNullException()
        {
            var provider = new Mock<ICouchbaseCacheCollectionProvider>();

            var cache = new CouchbaseCache(provider.Object, new CouchbaseCacheOptions());

            await Assert.ThrowsAsync<ArgumentNullException>(async () => await cache.RefreshAsync(null));
        }

        [Fact]
        public void Remove_WhenKeyIsNull_ThrowArgumentNullException()
        {
            var provider = new Mock<ICouchbaseCacheCollectionProvider>();

            var cache = new CouchbaseCache(provider.Object, new CouchbaseCacheOptions());

            Assert.Throws<ArgumentNullException>(() => cache.Remove(null));
        }

        [Fact]
        public async Task RemoveAsync_WhenKeyIsNull_ThrowArgumentNullException()
        {
            var provider = new Mock<ICouchbaseCacheCollectionProvider>();

            var cache = new CouchbaseCache(provider.Object, new CouchbaseCacheOptions());

            await Assert.ThrowsAsync< ArgumentNullException>(async () => await cache.RemoveAsync(null));
        }

        [Fact]
        public async Task RemoveAsync_DocumentNotFound_ReturnsNull()
        {
            var collection = new Mock<ICouchbaseCollection>();
            collection
                .Setup(m => m.RemoveAsync(It.IsAny<string>(), It.IsAny<RemoveOptions>()))
                .ThrowsAsync(new DocumentNotFoundException());

            var provider = new Mock<ICouchbaseCacheCollectionProvider>();
            provider
                .Setup(m => m.GetCollectionAsync())
                .ReturnsAsync(collection.Object);

            var cache = new CouchbaseCache(provider.Object, new CouchbaseCacheOptions());

            await cache.RemoveAsync("key");
        }

        [Fact]
        public void Set_WhenValueIsNull_ThrowArgumentNullException()
        {
            var provider = new Mock<ICouchbaseCacheCollectionProvider>();

            var cache = new CouchbaseCache(provider.Object, new CouchbaseCacheOptions());

            Assert.Throws<ArgumentNullException>(() => cache.Set(null, Array.Empty<byte>(), null));
        }

        [Fact]
        public async Task SetAsync_WhenValueIsNull_ThrowArgumentNullException()
        {
            var provider = new Mock<ICouchbaseCacheCollectionProvider>();

            var cache = new CouchbaseCache(provider.Object, new CouchbaseCacheOptions());

            await Assert.ThrowsAsync<ArgumentNullException>(async () => await cache.SetAsync(null, Array.Empty<byte>(), null));
        }

        #region TryGet

        [Fact]
        public async Task TryGetAsync_WhenKeyIsNull_ThrowArgumentNullException()
        {
            var provider = new Mock<ICouchbaseCacheCollectionProvider>();

            var cache = new CouchbaseCache(provider.Object, new CouchbaseCacheOptions());

            var writer = new ArrayBufferWriter<byte>();

            await Assert.ThrowsAsync<ArgumentNullException>(async () => await cache.TryGetAsync(null, writer));
        }

        [Fact]
        public async Task TryGetAsync_DocumentNotFound_ReturnsFalse()
        {
            var collection = new Mock<ICouchbaseCollection>();
            collection
                .Setup(m => m.LookupInAsync(It.IsAny<string>(), It.IsAny<IEnumerable<LookupInSpec>>(), It.IsAny<LookupInOptions>()))
                .ThrowsAsync(new DocumentNotFoundException());
            collection
                .Setup(m => m.Scope.Bucket.Cluster.ClusterServices.GetService(typeof(ITypeTranscoder)))
                .Returns(new JsonTranscoder());

            var provider = new Mock<ICouchbaseCacheCollectionProvider>();
            provider
                .Setup(m => m.GetCollectionAsync())
                .ReturnsAsync(collection.Object);

            var cache = new CouchbaseCache(provider.Object, new CouchbaseCacheOptions());

            var writer = new ArrayBufferWriter<byte>();

            var result = await cache.TryGetAsync("key", writer);

            Assert.False(result);
        }

        [Fact]
        public async Task TryGetAsync_DocumentFound_ReturnsTrueAndWrites()
        {
            var lookupResult = new Mock<ILookupInResult>();
            lookupResult
                .Setup(m => m.Exists(0))
                .Returns(true);
            lookupResult
                .Setup(m => m.Exists(1))
                .Returns(true);
            lookupResult
                .Setup(m => m.ContentAs<CacheBuffer>(0))
                .Returns((int _) => new CacheBuffer(TestBytes.ToArray()));
            lookupResult
                .Setup(m => m.ContentAs<CacheMetadata>(1))
                .Returns((int _) => new CacheMetadata { AbsoluteExpiration = DateTime.UtcNow.AddHours(1) });

            var collection = new Mock<ICouchbaseCollection>();
            collection
                .Setup(m => m.LookupInAsync(It.IsAny<string>(), It.IsAny<IEnumerable<LookupInSpec>>(), It.IsAny<LookupInOptions>()))
                .ReturnsAsync(lookupResult.Object);
            collection
                .Setup(m => m.Scope.Bucket.Cluster.ClusterServices.GetService(typeof(ITypeTranscoder)))
                .Returns(new JsonTranscoder());

            var provider = new Mock<ICouchbaseCacheCollectionProvider>();
            provider
                .Setup(m => m.GetCollectionAsync())
                .ReturnsAsync(collection.Object);

            var cache = new CouchbaseCache(provider.Object, new CouchbaseCacheOptions());

            var writer = new ArrayBufferWriter<byte>();

            var result = await cache.TryGetAsync("key", writer);

            Assert.True(result);
            Assert.True(writer.WrittenSpan.SequenceEqual(TestBytes), "Written data does not match.");
        }

        [Fact]
        public async Task TryGetAsync_DocumentFoundWithSlidingExpiration_Touches()
        {
            var lookupResult = new Mock<ILookupInResult>();
            lookupResult
                .Setup(m => m.Exists(0))
                .Returns(true);
            lookupResult
                .Setup(m => m.Exists(1))
                .Returns(true);
            lookupResult
                .Setup(m => m.ContentAs<CacheBuffer>(0))
                .Returns((int _) => new CacheBuffer(TestBytes.ToArray()));
            lookupResult
                .Setup(m => m.ContentAs<CacheMetadata>(1))
                .Returns((int _) => new CacheMetadata { SlidingExpiration = TimeSpan.FromMinutes(1) });

            var collection = new Mock<ICouchbaseCollection>();
            collection
                .Setup(m => m.LookupInAsync(It.IsAny<string>(), It.IsAny<IEnumerable<LookupInSpec>>(), It.IsAny<LookupInOptions>()))
                .ReturnsAsync(lookupResult.Object);
            collection
                .Setup(m => m.TouchAsync(It.IsAny<string>(), It.IsAny<TimeSpan>(), It.IsAny<TouchOptions>()))
                .Returns(Task.CompletedTask);
            collection
                .Setup(m => m.Scope.Bucket.Cluster.ClusterServices.GetService(typeof(ITypeTranscoder)))
                .Returns(new JsonTranscoder());

            var provider = new Mock<ICouchbaseCacheCollectionProvider>();
            provider
                .Setup(m => m.GetCollectionAsync())
                .ReturnsAsync(collection.Object);

            var cache = new CouchbaseCache(provider.Object, new CouchbaseCacheOptions());

            var writer = new ArrayBufferWriter<byte>();

            var result = await cache.TryGetAsync("key", writer);

            Assert.True(result);
            collection.Verify(m => m.TouchAsync("key", TimeSpan.FromMinutes(1), It.IsAny<TouchOptions>()), Times.Once);
        }

        #endregion

        #region Helpers

        private static ReadOnlySpan<byte> TestBytes => [0, 1, 2, 3, 4, 5, 6];

        #endregion
    }
}
