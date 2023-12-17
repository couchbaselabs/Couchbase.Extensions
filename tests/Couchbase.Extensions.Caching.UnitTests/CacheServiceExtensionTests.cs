using System.Linq;
using Microsoft.Extensions.DependencyInjection;
using Xunit;

namespace Couchbase.Extensions.Caching.UnitTests
{
    public class CacheServiceExtensionTests
    {
        [Fact]
        public void AddDistributedCouchbaseCache_RegistersDistributedCacheAsSingleton()
        {
            // Arrange
            var services = new ServiceCollection();

            // Act
            services.AddDistributedCouchbaseCache();

            // Assert
            var distributedCache = services.FirstOrDefault(desc => desc.ServiceType == typeof(ICouchbaseCache));

            Assert.NotNull(distributedCache);
            Assert.Equal(ServiceLifetime.Singleton, distributedCache.Lifetime);
        }

        [Fact]
        public void AddDistributedCouchbaseCache_Allows_Chaining()
        {
            var services = new ServiceCollection();

            Assert.Same(services, services.AddDistributedCouchbaseCache());
        }
    }
}
