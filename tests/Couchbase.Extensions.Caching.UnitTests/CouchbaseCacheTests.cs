using System;
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
    }
}
