using System;
using System.Threading.Tasks;
using Couchbase.Extensions.Caching.Internal;
using Microsoft.Extensions.Caching.Distributed;
using Moq;
using Xunit;
using Xunit.Abstractions;

namespace Couchbase.Extensions.Caching.UnitTests
{
    public class CouchbaseCacheExtensionTests
    {
        private readonly ITestOutputHelper output;

        public CouchbaseCacheExtensionTests(ITestOutputHelper output)
        {
            this.output = output;
        }

        [Fact]
        public void Set_WhenKeyIsNull_ThrowArgumentNullException()
        {
            var provider = new Mock<ICouchbaseCacheCollectionProvider>();

            var cache = new CouchbaseCache(provider.Object, new CouchbaseCacheOptions());

            Assert.Throws<ArgumentNullException>(() => cache.Set(null!, Array.Empty<byte>(), new DistributedCacheEntryOptions()));
        }

        [Fact]
        public async Task SetAsync_WhenKeyIsNull_ThrowArgumentNullException()
        {
            var provider = new Mock<ICouchbaseCacheCollectionProvider>();

            var cache = new CouchbaseCache(provider.Object, new CouchbaseCacheOptions());

            await Assert.ThrowsAsync<ArgumentNullException>(async () => await cache.SetAsync(null!, Array.Empty<byte>(), new DistributedCacheEntryOptions()));
        }

        [Fact]
        public void Get_WhenKeyIsNull_ThrowArgumentNullException()
        {
            var provider = new Mock<ICouchbaseCacheCollectionProvider>();

            var cache = new CouchbaseCache(provider.Object, new CouchbaseCacheOptions());

            Assert.Throws<ArgumentNullException>(() => cache.Get(null!));
        }

        [Fact]
        public async Task GetAsync_WhenKeyIsNull_ThrowArgumentNullException()
        {
            var provider = new Mock<ICouchbaseCacheCollectionProvider>();

            var cache = new CouchbaseCache(provider.Object, new CouchbaseCacheOptions());

            await Assert.ThrowsAsync<ArgumentNullException>(async () => await cache.GetAsync(null!));
        }

        [Fact]
        public async Task GetAsync_Generic_WhenKeyIsNull_ThrowArgumentNullException()
        {
            var provider = new Mock<ICouchbaseCacheCollectionProvider>();

            var cache = new CouchbaseCache(provider.Object, new CouchbaseCacheOptions());

            await Assert.ThrowsAsync<ArgumentNullException>(async () => await cache.GetAsync<Poco>(null!));
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
