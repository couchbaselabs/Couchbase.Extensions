using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using Xunit;

namespace Couchbase.Extensions.Session.UnitTests
{
    public class CouchbaseSessionServiceCollectionExtensionTests
    {
        [Fact]
        public void AddCouchbaseSession_Registers_CouchbaseDistributedSession_As_Transient()
        {
            var services = new ServiceCollection();

            services.AddCouchbaseSession();

            var session = services.FirstOrDefault(x => x.ImplementationType == typeof(CouchbaseSessionStore));

            Assert.Equal(ServiceLifetime.Transient, session.Lifetime);
        }

        [Fact]
        public void AddCouchbaseSession2_Registers_CouchbaseDistributedSession_As_Transient()
        {
            var services = new ServiceCollection();

            services.AddCouchbaseSession(opt=> {});

            var session = services.FirstOrDefault(x => x.ImplementationType == typeof(CouchbaseSessionStore));

            Assert.Equal(ServiceLifetime.Transient, session.Lifetime);
        }
    }
}
