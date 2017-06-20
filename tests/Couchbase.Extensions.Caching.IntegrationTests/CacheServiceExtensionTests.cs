using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Couchbase.Extensions.Caching.IntegrationTests.Infrastructure;
using Couchbase.Extensions.DependencyInjection;
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.DependencyInjection;
using Moq;
using NUnit.Framework;

namespace Couchbase.Extensions.Caching.IntegrationTests
{
    [TestFixture]
    public class CacheServiceExtensionTests
    {
        [Ignore("Temporarily skip.")]
        [Test]
        public void AddDistributedCouchbaseCache_ReplacesPreviouslyUserRegisteredServices()
        {
            // Arrange
            var services = new ServiceCollection();
            services.AddScoped(typeof(IDistributedCache), sp => new Mock<IDistributedCache>());

            var config = TestConfiguration.GetConfiguration();
            services.AddCouchbase(config);

            // Act
            services.AddDistributedCouchbaseCache("default", options => { });

            // Assert
            var serviceProvider = services.BuildServiceProvider();

            var distributedCache = services.FirstOrDefault(desc => desc.ServiceType == typeof(IDistributedCache));

            Assert.NotNull(distributedCache);
            Assert.AreEqual(ServiceLifetime.Scoped, distributedCache.Lifetime);
            Assert.IsInstanceOf<CouchbaseCache>(serviceProvider.GetRequiredService<IDistributedCache>());
        }
    }
}
