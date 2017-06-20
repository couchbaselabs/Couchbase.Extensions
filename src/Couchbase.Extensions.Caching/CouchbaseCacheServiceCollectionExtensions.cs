using System;
using Couchbase.Extensions.DependencyInjection;
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;

namespace Couchbase.Extensions.Caching
{
    public static class CouchbaseCacheServiceCollectionExtensions
    {
        /// <summary>
        /// Adds a <see cref="CouchbaseCache"/> as a service using a <see cref="Action{CouchbaseCacheOptions}"/>
        /// </summary>
        /// <param name="services">The <see cref="IServiceCollection"/> to add the <see cref="CouchbaseCache"/> service to.</param>
        /// <param name="setupAction">The setup delegate that will be fired when the service is created.</param>
        /// <returns>The <see cref="IServiceCollection"/> that was updated with the <see cref="Action{CouchbaseCacheOptions}"/></returns>
        public static IServiceCollection AddDistributedCouchbaseCache(this IServiceCollection services, Action<CouchbaseCacheOptions> setupAction)
        {
            if (services == null)
            {
                throw new ArgumentNullException(nameof(services));
            }
            if (setupAction == null)
            {
                throw new ArgumentNullException(nameof(setupAction));
            }

            services.AddOptions();
            services.Configure(setupAction);
            services.TryAddSingleton<IDistributedCache, CouchbaseCache>();

            return services;
        }

        /// <summary>
        /// Adds a <see cref="CouchbaseCache"/> as a service using a <see cref="Action{CouchbaseCacheOptions}"/>
        /// </summary>
        /// <param name="services">The <see cref="IServiceCollection"/> to add the <see cref="CouchbaseCache"/> service to.</param>
        /// <param name="bucketName">The bucket name that the cache will use.</param>
        /// <param name="setupAction">The setup delegate that will be fired when the service is created.</param>
        /// <returns>The <see cref="IServiceCollection"/> that was updated with the <see cref="Action{CouchbaseCacheOptions}"/></returns>
        public static IServiceCollection AddDistributedCouchbaseCache(this IServiceCollection services, string bucketName, Action<CouchbaseCacheOptions> setupAction)
        {
            if (services == null)
            {
                throw new ArgumentNullException(nameof(services));
            }
            if (setupAction == null)
            {
                throw new ArgumentNullException(nameof(setupAction));
            }

            services.AddCouchbaseBucket<ICouchbaseCacheBucketProvider>(bucketName);
            services.AddOptions();
            services.Configure(setupAction);
            services.TryAddSingleton<IDistributedCache, CouchbaseCache>();

            return services;
        }

        /// <summary>
        /// Adds a <see cref="CouchbaseCache"/> as a service using a <see cref="Action{CouchbaseCacheOptions}"/>
        /// </summary>
        /// <param name="services">The <see cref="IServiceCollection"/> to add the <see cref="CouchbaseCache"/> service to.</param>
        /// <param name="password">The password for the bucket that the cache will use.</param>
        /// <param name="setupAction">The setup delegate that will be fired when the service is created.</param>
        /// <param name="bucketName">The bucket name that the cache will use.</param>
        /// <returns>The <see cref="IServiceCollection"/> that was updated with the <see cref="Action{CouchbaseCacheOptions}"/></returns>
        public static IServiceCollection AddDistributedCouchbaseCache(this IServiceCollection services, string bucketName, string password, Action<CouchbaseCacheOptions> setupAction)
        {
            if (services == null)
            {
                throw new ArgumentNullException(nameof(services));
            }
            if (setupAction == null)
            {
                throw new ArgumentNullException(nameof(setupAction));
            }

            services.AddCouchbaseBucket<ICouchbaseCacheBucketProvider>(bucketName, password);
            services.AddOptions();
            services.Configure(setupAction);
            services.TryAddSingleton<IDistributedCache, CouchbaseCache>();

            return services;
        }
    }
}
