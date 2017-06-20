using System.Collections.Specialized;
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.DependencyInjection;
using Moq;
using NUnit.Framework;
using System.Linq;
using Couchbase.Configuration.Client;
using Couchbase.Core;
using Couchbase.Extensions.DependencyInjection;

namespace Couchbase.Extensions.Caching.UnitTests
{
    [TestFixture]
    public class CacheServiceExtensionTests
    {
        [Test]
        public void AddDistributedCouchbaseCache_RegistersDistributedCacheAsSingleton()
        {
            // Arrange
            var services = new ServiceCollection();

            // Act
            services.AddDistributedCouchbaseCache("bucketName", options => { });

            // Assert
            var distributedCache = services.FirstOrDefault(desc => desc.ServiceType == typeof(IDistributedCache));

            Assert.NotNull(distributedCache);
            Assert.AreEqual(ServiceLifetime.Singleton, distributedCache.Lifetime);
        }

        [Test]
        public void AddDistributedCouchbaseCache_Allows_Chaining()
        {
            var services = new ServiceCollection();

            Assert.AreSame(services, services.AddDistributedCouchbaseCache("bucketName", _ => { }));
        }
    }
}
