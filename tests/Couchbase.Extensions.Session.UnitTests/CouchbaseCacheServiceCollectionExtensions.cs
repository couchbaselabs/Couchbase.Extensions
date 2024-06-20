using Couchbase.Extensions.Caching;
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Couchbase.Extensions.Session.UnitTests
{
    internal static class CouchbaseCacheServiceCollectionExtensions
    {
        public static IServiceCollection AddDistributedCouchbaseCache(this IServiceCollection services)
        {
            if (services == null)
            {
                throw new ArgumentNullException(nameof(services));
            }
            services.AddOptions();

            var descriptor = services.FirstOrDefault(x => x.ServiceType == typeof(ICouchbaseCache));
            if (descriptor != null) services.Remove(descriptor);
            services.AddSingleton<IDistributedCache, MemoryDistributedCache>();
            services.TryAddSingleton<ICouchbaseCache, CouchbaseInMemoryCache>();
            services.TryAddSingleton(TimeProvider.System);

            return services;
        }
    }
}
