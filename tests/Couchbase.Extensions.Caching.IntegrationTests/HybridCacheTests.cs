using System;
using System.Linq;
using System.Threading.Tasks;
using Couchbase.Extensions.Caching.Internal;
using Couchbase.KeyValue;
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Caching.Hybrid;
using Microsoft.Extensions.DependencyInjection;
using Moq;
using Xunit;

namespace Couchbase.Extensions.Caching.IntegrationTests
{
    public class HybridCacheTests : IClassFixture<ClusterFixture>
    {
        private readonly ClusterFixture _fixture;

        public HybridCacheTests(ClusterFixture fixture)
        {
            _fixture = fixture;
        }

        [Fact]
        public async Task Test_SetAndGetAsync()
        {
            var cache = GetCache();

            const string key = $"HybridCacheTests.{nameof(Test_SetAndGetAsync)}";
            var poco = new Poco {Name = "poco1", Age = 12};

            await cache.RemoveAsync(key);
            await cache.SetAsync(key, poco, new HybridCacheEntryOptions
            {
                Flags = HybridCacheEntryFlags.DisableLocalCache
            });
            var result = await cache.GetOrCreateAsync(key, _ => ValueTask.FromResult(new Poco() { Name = "foo" }), new HybridCacheEntryOptions
            {
                Flags = HybridCacheEntryFlags.DisableLocalCache
            });

            Assert.Equal(poco.Name, result.Name);
        }

        [Fact]
        public async Task Test_GetAsync_Missing()
        {
            var cache = GetCache();

            const string key = $"HybridCacheTests.{nameof(Test_GetAsync_Missing)}";

            var result = await cache.GetOrCreateAsync(key, _ => ValueTask.FromResult(new Poco() { Name = "foo" }), new HybridCacheEntryOptions
            {
                Flags = HybridCacheEntryFlags.DisableLocalCache,
                Expiration = TimeSpan.FromSeconds(1)
            });

            Assert.Equal("foo", result.Name);
        }

        [Fact]
        public async Task Test_RemoveAsync()
        {
            var cache = GetCache();

            const string key = $"HybridCacheTests.{nameof(Test_RemoveAsync)}";

            await cache.SetAsync(key, new Poco());
            await cache.RemoveAsync(key);
            var result = await cache.GetOrCreateAsync(key, _ => ValueTask.FromResult<Poco>(null), new HybridCacheEntryOptions()
            {
                Flags = HybridCacheEntryFlags.DisableLocalCache,
                Expiration = TimeSpan.FromSeconds(1)
            });

            Assert.Null(result);
        }

        [Fact]
        public async Task Test_RemoveAsync_Missing()
        {
            var cache = GetCache();

            const string key = $"HybridCacheTests.{nameof(Test_RemoveAsync)}";

            await cache.RemoveAsync(key);
        }

        private HybridCache GetCache()
        {
            var services = new ServiceCollection();
            services.AddSingleton<ICouchbaseCacheBucketProvider>(_fixture);
            services.AddSingleton<ICouchbaseCacheCollectionProvider>(_fixture);
            services.AddDistributedCouchbaseCache();

            services
                .AddHybridCache(options =>
                {
                    options.MaximumKeyLength = 250; // Maximum Couchbase key size
                    options.MaximumPayloadBytes = 20 * 1024 * 1024; // Maximum 20MB Couchbase document size
                    options.DisableCompression = true; // Prefer Snappy compression built into the Couchbase SDK
                })
                .AddSerializerFactory<CouchbaseCacheSerializerFactory>();

            return services.BuildServiceProvider(new ServiceProviderOptions()
            {
                ValidateOnBuild = true,
                ValidateScopes = true
            })
                .GetRequiredService<HybridCache>();
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
