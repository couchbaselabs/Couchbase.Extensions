using System;
using Couchbase.Extensions.Caching.Internal;
using Couchbase.Extensions.DependencyInjection;
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Options;

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
            => services.AddDistributedCouchbaseCacheCore(serviceKey: null, setupAction);

        /// <summary>
        /// Adds a <see cref="CouchbaseCache"/> as a service using a <see cref="Action{CouchbaseCacheOptions}"/>
        /// </summary>
        /// <param name="services">The <see cref="IServiceCollection"/> to add the <see cref="CouchbaseCache"/> service to.</param>
        /// <param name="setupAction">The setup delegate that will be fired when the service is created.</param>
        /// <returns>The <see cref="IServiceCollection"/> that was updated with the <see cref="Action{CouchbaseCacheOptions}"/></returns>
        public static IServiceCollection AddKeyedDistributedCouchbaseCache(this IServiceCollection services,
            object serviceKey,
            Action<CouchbaseCacheOptions>? setupAction = null)
        {
            ArgumentNullException.ThrowIfNull(serviceKey);

            return services.AddDistributedCouchbaseCacheCore(serviceKey, setupAction);
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

        private static IServiceCollection AddDistributedCouchbaseCacheCore(this IServiceCollection services,
            object? serviceKey,
            Action<CouchbaseCacheOptions>? setupAction = null)
        {
            ArgumentNullException.ThrowIfNull(services);

            var optionsName = serviceKey?.ToString() ?? Options.DefaultName;
            var optionsBuilder = services.AddOptions<CouchbaseCacheOptions>(optionsName);
            if (setupAction is not null)
            {
                optionsBuilder.Configure(setupAction);
            }

            if (serviceKey is null)
            {
                services.TryAddCouchbaseBucket<ICouchbaseCacheBucketProvider, DefaultCouchbaseCacheBucketProvider>(static bucketBuilder =>
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
            }
            else
            {
                services.TryAddKeyedSingleton<ICouchbaseCacheBucketProvider>(serviceKey,
                    (sp, serviceKey) =>
                    {
                        return new DefaultCouchbaseCacheBucketProvider(
                            sp.GetRequiredService<IBucketProvider>(),
                            sp.GetRequiredService<IOptionsMonitor<CouchbaseCacheOptions>>().Get(optionsName));
                    });
                services.TryAddKeyedSingleton<ICouchbaseCacheCollectionProvider>(serviceKey,
                    (sp, serviceKey) =>
                    {
                        return new DefaultCouchbaseCacheCollectionProvider(
                            sp.GetRequiredKeyedService<ICouchbaseCacheBucketProvider>(serviceKey),
                            sp.GetRequiredService<IOptionsMonitor<CouchbaseCacheOptions>>().Get(optionsName));
                    });

                // Replace any already registered IDistributedCache. Forward requests to the singleton
                // ICouchbaseCache instance. This is registered as transient in case the consumer has registered
                // a custom ICouchbaseCache implementation that is not a singleton.
                services.RemoveAllKeyed<IDistributedCache>(serviceKey);
                services.AddKeyedTransient<IDistributedCache>(serviceKey,
                    static (serviceProvider, serviceKey) => serviceProvider.GetRequiredKeyedService<ICouchbaseCache>(serviceKey));

                services.TryAddKeyedSingleton<ICouchbaseCache>(serviceKey,
                    (sp, serviceKey) => new CouchbaseCache(
                        sp.GetRequiredKeyedService<ICouchbaseCacheCollectionProvider>(serviceKey),
                        sp.GetRequiredService<IOptionsMonitor<CouchbaseCacheOptions>>().Get(optionsName)));
            }

            return services;
        }
    }
}
