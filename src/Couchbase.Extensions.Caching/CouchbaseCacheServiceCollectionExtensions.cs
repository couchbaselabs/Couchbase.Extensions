using System;
using Couchbase.Extensions.Caching.Internal;
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
        public static IServiceCollection AddDistributedCouchbaseCache(this IServiceCollection services,
            Action<CouchbaseCacheOptions>? setupAction = null)
        {
            ArgumentNullException.ThrowIfNull(services);

            services.AddOptions();
            if (setupAction is not null)
            {
                services.Configure(setupAction);
            }

            services.TryAddCouchbaseBucket<ICouchbaseCacheBucketProvider, DefaultCouchbaseCacheBucketProvider>(bucketBuilder =>
            {
                bucketBuilder.AddCollection<ICouchbaseCacheCollectionProvider, DefaultCouchbaseCacheCollectionProvider>();
            });

            // Replace any already registered IDistributedCache. Forward requests to the singleton
            // ICouchbaseCache instance. This is registered as transient in case the consumer has registered
            // a custom ICouchbaseCache implementation that is not a singleton.
            services.RemoveAll<IDistributedCache>();
            services.AddTransient<IDistributedCache>(
                static serviceProvider => serviceProvider.GetRequiredService<ICouchbaseCache>());

            services.TryAddSingleton<ICouchbaseCache, CouchbaseCache>();

            return services;
        }

        /// <summary>
        /// Adds a <see cref="CouchbaseCache"/> as a service using a <see cref="Action{CouchbaseCacheOptions}"/>
        /// </summary>
        /// <param name="services">The <see cref="IServiceCollection"/> to add the <see cref="CouchbaseCache"/> service to.</param>
        /// <param name="bucketName">The bucket name that the cache will use.</param>
        /// <param name="setupAction">The setup delegate that will be fired when the service is created.</param>
        /// <returns>The <see cref="IServiceCollection"/> that was updated with the <see cref="Action{CouchbaseCacheOptions}"/></returns>
        [Obsolete("Use the overload that accepts an Action<CouchbaseCacheOptions> instead, setting the bucket name in the callback.")]
        public static IServiceCollection AddDistributedCouchbaseCache(this IServiceCollection services, string bucketName, Action<CouchbaseCacheOptions>? setupAction = null)
        {
            return services.AddDistributedCouchbaseCache(options =>
            {
                options.BucketName = bucketName;
                setupAction?.Invoke(options);
            });
        }
    }
}
