using System;
using System.Linq;
using System.Threading.Tasks;
using Couchbase.Extensions.Caching.Internal;
using Couchbase.KeyValue;
using Microsoft.Extensions.Caching.Distributed;
using Moq;
using Xunit;

namespace Couchbase.Extensions.Caching.IntegrationTests
{
    public class CouchbaseCacheTests : IClassFixture<ClusterFixture>
    {
        private readonly ClusterFixture _fixture;

        public CouchbaseCacheTests(ClusterFixture fixture)
        {
            _fixture = fixture;
        }

        [Fact]
        public void Test_SetAndGet_Bytes()
        {
            var cache = GetCache();

            const string key = $"CouchbaseCacheTests.{nameof(Test_SetAndGet_Bytes)}";
            var bytes = Enumerable.Range(1, 64).Select(p => (byte) p).ToArray();

            cache.Remove(key);
            cache.Set(key, bytes, new DistributedCacheEntryOptions
            {
                AbsoluteExpirationRelativeToNow = TimeSpan.FromSeconds(15)
            });
            var result = cache.Get(key);

            Assert.Equal(bytes, result);
        }

        [Fact]
        public async Task Test_SetAndGetAsync_Bytes()
        {
            var cache = GetCache();

            const string key = $"CouchbaseCacheTests.{nameof(Test_SetAndGetAsync_Bytes)}";
            var bytes = Enumerable.Range(1, 64).Select(p => (byte) p).ToArray();

            await cache.RemoveAsync(key);
            await cache.SetAsync(key, bytes, new DistributedCacheEntryOptions
            {
                AbsoluteExpirationRelativeToNow = TimeSpan.FromSeconds(15)
            });
            var result = await cache.GetAsync(key);

            Assert.Equal(bytes, result);
        }

        [Fact]
        public async Task Test_SetAndGetAsync_Poco()
        {
            var cache = GetCache();

            const string key = $"CouchbaseCacheTests.{nameof(Test_SetAndGetAsync_Poco)}";
            var poco = new Poco {Name = "poco1", Age = 12};

            await cache.RemoveAsync(key);
            await cache.SetAsync(key, poco, new DistributedCacheEntryOptions
            {
                AbsoluteExpirationRelativeToNow = TimeSpan.FromSeconds(15)
            });
            var result = await cache.GetAsync<Poco>(key);

            Assert.Equal(poco.Name, result.Name);
        }

        [Fact]
        public void Test_Get_MissingBytes()
        {
            var cache = GetCache();

            const string key = $"CouchbaseCacheTests.{nameof(Test_Get_MissingBytes)}";

            var bytes = cache.Get(key);

            Assert.Null(bytes);
        }

        [Fact]
        public async Task Test_GetAsync_MissingBytes()
        {
            var cache = GetCache();

            const string key = $"CouchbaseCacheTests.{nameof(Test_GetAsync_MissingBytes)}";

            var bytes = await cache.GetAsync(key);

            Assert.Null(bytes);
        }

        [Fact]
        public async Task Test_GetAsync_MissingPoco()
        {
            var cache = GetCache();

            const string key = $"CouchbaseCacheTests.{nameof(Test_GetAsync_MissingPoco)}";

            var bytes = await cache.GetAsync<Poco>(key);

            Assert.Null(bytes);
        }

        [Fact]
        public void Test_Remove()
        {
            var cache = GetCache();

            const string key = $"CouchbaseCacheTests.{nameof(Test_Remove)}";
            var bytes = Enumerable.Range(1, 64).Select(p => (byte) p).ToArray();

            cache.Set(key, bytes);
            cache.Remove(key);
            var result = cache.Get(key);

            Assert.Null(result);
        }

        [Fact]
        public async Task Test_RemoveAsync()
        {
            var cache = GetCache();

            const string key = $"CouchbaseCacheTests.{nameof(Test_RemoveAsync)}";
            var bytes = Enumerable.Range(1, 64).Select(p => (byte) p).ToArray();

            await cache.SetAsync(key, bytes);
            await cache.RemoveAsync(key);
            var result = await cache.GetAsync(key);

            Assert.Null(result);
        }

        [Fact]
        public void Test_Remove_Missing()
        {
            var cache = GetCache();

            const string key = $"CouchbaseCacheTests.{nameof(Test_RemoveAsync)}";

            cache.Remove(key);
        }

        [Fact]
        public async Task Test_RemoveAsync_Missing()
        {
            var cache = GetCache();

            const string key = $"CouchbaseCacheTests.{nameof(Test_RemoveAsync)}";

            await cache.RemoveAsync(key);
        }

        [Fact]
        public void Test_Refresh_Missing()
        {
            var cache = GetCache();

            const string key = $"CouchbaseCacheTests.{nameof(Test_Refresh_Missing)}";

            cache.Refresh(key);
        }

        [Fact]
        public async Task Test_RefreshAsync_Missing()
        {
            var cache = GetCache();

            const string key = $"CouchbaseCacheTests.{nameof(Test_RefreshAsync_Missing)}";

            await cache.RefreshAsync(key);
        }

        [Fact]
        public async Task Test_RefreshAsync_SlidesExpiration()
        {
            var cache = GetCache();

            const string key = $"CouchbaseCacheTests.{nameof(Test_RefreshAsync_SlidesExpiration)}";
            var bytes = Enumerable.Range(1, 64).Select(p => (byte) p).ToArray();

            await cache.SetAsync(key, bytes, new DistributedCacheEntryOptions {
                SlidingExpiration = TimeSpan.FromSeconds(2)
            });

            await Task.Delay(1000);

            await cache.RefreshAsync(key);

            await Task.Delay(1000);

            var result = await cache.GetAsync(key);

            Assert.Equal(bytes, result);
        }

        [Fact]
        public async Task Test_RefreshAsync_DoesNotSlidePastAbsoluteExpiration()
        {
            var cache = GetCache();

            const string key = $"CouchbaseCacheTests.{nameof(Test_RefreshAsync_SlidesExpiration)}";
            var bytes = Enumerable.Range(1, 64).Select(p => (byte) p).ToArray();

            await cache.SetAsync(key, bytes, new DistributedCacheEntryOptions {
                AbsoluteExpirationRelativeToNow = TimeSpan.FromSeconds(2),
                SlidingExpiration = TimeSpan.FromSeconds(5)
            });

            await Task.Delay(1000);

            await cache.RefreshAsync(key);

            await Task.Delay(2000);

            var result = await cache.GetAsync(key);

            Assert.Null(result);
        }

        [Fact]
        public async Task Test_SetAsync_With_AbsoluteExpirationRelativeToNow_Expires()
        {
            var cache = GetCache();
            const string key = $"CouchbaseCacheTests.{nameof(Test_SetAsync_With_AbsoluteExpirationRelativeToNow_Expires)}";
            await cache.SetAsync(key, "some cache value", new DistributedCacheEntryOptions
            {
                AbsoluteExpirationRelativeToNow = TimeSpan.FromSeconds(2)
            });
            await Task.Delay(3000);

            var result = await cache.GetAsync(key);

            Assert.Null(result);
        }

        [Fact]
        public async Task Test_SetAsync_With_AbsoluteExpiration_Expires()
        {
            var cache = GetCache();
            const string key = $"CouchbaseCacheTests.{nameof(Test_SetAsync_With_AbsoluteExpiration_Expires)}";
            await cache.SetAsync(key, "some cache value", new DistributedCacheEntryOptions
            {
                AbsoluteExpiration = DateTimeOffset.UtcNow.AddSeconds(2)
            });
            await Task.Delay(3000);

            var result = await cache.GetAsync(key);

            Assert.Null(result);
        }

        private CouchbaseCache GetCache()
        {
            var provider = new Mock<ICouchbaseCacheCollectionProvider>();
            provider
                .Setup(x => x.GetCollectionAsync())
                .Returns(() => new ValueTask<ICouchbaseCollection>(_fixture.GetDefaultCollectionAsync()));

            return new CouchbaseCache(provider.Object, new CouchbaseCacheOptions());
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
