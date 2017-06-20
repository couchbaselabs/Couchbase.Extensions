using System;
using Couchbase.Core;
using Moq;
using NUnit.Framework;

namespace Couchbase.Extensions.Caching.UnitTests
{
    [TestFixture]
    public class CouchbaseCacheTests
    {
        [Test]
        public void Set_WhenKeyIsNull_ThrowArgumentNullException()
        {
            var provider = new Mock<ICouchbaseCacheBucketProvider>();

            var cache = new CouchbaseCache(provider.Object, new CouchbaseCacheOptions());

            Assert.Throws<ArgumentNullException>(() => cache.Set(null, new byte[0], null));
        }

        [Test]
        public void SetAsync_WhenKeyIsNull_ThrowArgumentNullException()
        {
            var provider = new Mock<ICouchbaseCacheBucketProvider>();

            var cache = new CouchbaseCache(provider.Object, new CouchbaseCacheOptions());

            Assert.ThrowsAsync<ArgumentNullException>(async () => await cache.SetAsync(null, new byte[0], null));
        }

        [Test]
        public void Get_WhenKeyIsNull_ThrowArgumentNullException()
        {
            var provider = new Mock<ICouchbaseCacheBucketProvider>();

            var cache = new CouchbaseCache(provider.Object, new CouchbaseCacheOptions());

            Assert.Throws<ArgumentNullException>(() => cache.Get(null));
        }

        [Test]
        public void GetAsync_WhenKeyIsNull_ThrowArgumentNullException()
        {
            var provider = new Mock<ICouchbaseCacheBucketProvider>();

            var cache = new CouchbaseCache(provider.Object, new CouchbaseCacheOptions());

            Assert.ThrowsAsync<ArgumentNullException>(async () => await cache.GetAsync(null));
        }

        [Test]
        public void Refresh_WhenKeyIsNull_ThrowArgumentNullException()
        {
            var provider = new Mock<ICouchbaseCacheBucketProvider>();

            var cache = new CouchbaseCache(provider.Object, new CouchbaseCacheOptions());

            Assert.Throws<ArgumentNullException>(() => cache.Refresh(null));
        }

        [Test]
        public void RefreshAsync_WhenKeyIsNull_ThrowArgumentNullException()
        {
            var provider = new Mock<ICouchbaseCacheBucketProvider>();

            var cache = new CouchbaseCache(provider.Object, new CouchbaseCacheOptions());

            Assert.ThrowsAsync<ArgumentNullException>(async () => await cache.RefreshAsync(null));
        }

        [Test]
        public void Remove_WhenKeyIsNull_ThrowArgumentNullException()
        {
            var provider = new Mock<ICouchbaseCacheBucketProvider>();

            var cache = new CouchbaseCache(provider.Object, new CouchbaseCacheOptions());

            Assert.Throws<ArgumentNullException>(() => cache.Remove(null));
        }

        [Test]
        public void RemoveAsync_WhenKeyIsNull_ThrowArgumentNullException()
        {
            var provider = new Mock<ICouchbaseCacheBucketProvider>();

            var cache = new CouchbaseCache(provider.Object, new CouchbaseCacheOptions());

            Assert.ThrowsAsync< ArgumentNullException>(async () => await cache.RemoveAsync(null));
        }

        [Test] public void Set_WhenValueIsNull_ThrowArgumentNullException()
        {
            var provider = new Mock<ICouchbaseCacheBucketProvider>();

            var cache = new CouchbaseCache(provider.Object, new CouchbaseCacheOptions());

            Assert.Throws<ArgumentNullException>(() => cache.Set(null, new byte[0], null));
        }

        [Test]
        public void SetAsync_WhenValueIsNull_ThrowArgumentNullException()
        {
            var provider = new Mock<ICouchbaseCacheBucketProvider>();

            var cache = new CouchbaseCache(provider.Object, new CouchbaseCacheOptions());

            Assert.ThrowsAsync<ArgumentNullException>(async () => await cache.SetAsync(null, new byte[0], null));
        }
    }
}
