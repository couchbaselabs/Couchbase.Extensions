using System;
using Microsoft.Extensions.Caching.Distributed;
using Moq;
using NUnit.Framework;

namespace Couchbase.Extensions.Caching.UnitTests
{
    [TestFixture]
    public class CouchbaseCacheExtensionTests
    {
        [Test]
        public void Set_WhenKeyIsNull_ThrowArgumentNullException()
        {
            var provider = new Mock<ICouchbaseCacheBucketProvider>();

            var cache = new CouchbaseCache(provider.Object, new CouchbaseCacheOptions());

            Assert.Throws<ArgumentNullException>(() => cache.Set(null, new byte[0], new DistributedCacheEntryOptions()));
        }

        [Test]
        public void SetAsync_WhenKeyIsNull_ThrowArgumentNullException()
        {
            var provider = new Mock<ICouchbaseCacheBucketProvider>();

            var cache = new CouchbaseCache(provider.Object, new CouchbaseCacheOptions());

            Assert.ThrowsAsync<ArgumentNullException>(async () => await cache.SetAsync(null, new byte[0], new DistributedCacheEntryOptions()));
        }

        [Test]
        public void Get_WhenKeyIsNull_ThrowArgumentNullException()
        {
            var provider = new Mock<ICouchbaseCacheBucketProvider>();

            var cache = new CouchbaseCache(provider.Object, new CouchbaseCacheOptions());

            Assert.Throws<ArgumentNullException>(() => cache.Get(null, new DistributedCacheEntryOptions()));
        }

        [Test]
        public void GetAsync_WhenKeyIsNull_ThrowArgumentNullException()
        {
            var provider = new Mock<ICouchbaseCacheBucketProvider>();

            var cache = new CouchbaseCache(provider.Object, new CouchbaseCacheOptions());

            Assert.ThrowsAsync<ArgumentNullException>(async () => await cache.GetAsync(null, new DistributedCacheEntryOptions()));
        }

        [Test]
        public void Get_Generic_WhenKeyIsNull_ThrowArgumentNullException()
        {
            var provider = new Mock<ICouchbaseCacheBucketProvider>();

            var cache = new CouchbaseCache(provider.Object, new CouchbaseCacheOptions());

            Assert.Throws<ArgumentNullException>(() => cache.Get<Poco>(null, new DistributedCacheEntryOptions()));
        }

        [Test]
        public void GetAsynct_Generic_WhenKeyIsNull_ThrowArgumentNullException()
        {
            var provider = new Mock<ICouchbaseCacheBucketProvider>();

            var cache = new CouchbaseCache(provider.Object, new CouchbaseCacheOptions());

            Assert.ThrowsAsync<ArgumentNullException>(async () => await cache.GetAsync<Poco>(null, new DistributedCacheEntryOptions()));
        }

        public class Poco
        {
            public string Name { get; set; }

            public int Age { get; set; }

            public override string ToString()
            {
                return string.Concat(Name, "-", Age);
            }
        }
    }
}
