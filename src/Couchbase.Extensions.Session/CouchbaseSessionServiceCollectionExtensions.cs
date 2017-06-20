using System;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Session;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;

namespace Couchbase.Extensions.Session
{
    /// <summary>
    /// Extension methods for adding a CouchbaseSession to the DI container.
    /// </summary>
    public static class CouchbaseSessionServiceCollectionExtensions
    {
        /// <summary>
        /// Adds the <see cref="CouchbaseSession"></see> to the application.
        /// </summary>
        /// <param name="services">The services.</param>
        /// <returns></returns>
        /// <exception cref="System.ArgumentNullException"></exception>
        public static IServiceCollection AddCouchbaseSession(this IServiceCollection services)
        {
            if (services == null)
            {
                throw new ArgumentNullException(nameof(services));
            }

            services.TryAddTransient<ISessionStore, CouchbaseSessionStore>();
            services.AddDataProtection();

            return services;
        }

        /// <summary>
        /// Adds the <see cref="CouchbaseSession"></see> to the application with <see cref="SessionOptions"/>.
        /// </summary>
        /// <param name="services">The services.</param>
        /// <param name="options">The options.</param>
        /// <returns></returns>
        /// <exception cref="System.ArgumentNullException">
        /// </exception>
        public static IServiceCollection AddCouchbaseSession(this IServiceCollection services,
            Action<SessionOptions> options)
        {
            if (services == null)
            {
                throw new ArgumentNullException(nameof(services));
            }
            if (options == null)
            {
                throw new ArgumentNullException(nameof(options));
            }

            services.Configure(options);
            services.AddCouchbaseSession();

            return services;
        }
    }
}
