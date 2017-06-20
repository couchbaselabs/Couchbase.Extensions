using System;
using Microsoft.AspNetCore.Http;

namespace Couchbase.Extensions.Session
{
    /// <summary>
    /// Extensions for persisting POCO's to the sesson store.
    /// </summary>
    public static class CouchbaseSessionExtensions
    {
        /// <summary>
        /// Sets the object of Type T into the Couchbase session store.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="session">The session.</param>
        /// <param name="key">The key for the item.</param>
        /// <param name="value">The value to persist into the session store.</param>
        /// <exception cref="System.NotSupportedException">Only supported by CouchbaseDistributedSession!</exception>
        public static void SetObject<T>(this ISession session, string key, T value)
        {
            var couchbaseSession = session as CouchbaseSession;
            if (couchbaseSession == null)
            {
                throw new NotSupportedException("Only supported by CouchbaseDistributedSession!");
            }

            couchbaseSession.Set(key, value);
        }

        /// <summary>
        /// Gets the object of Type T from the Couchbase session store.
        /// </summary>
        /// <typeparam name="T">The <see cref="Type"/> to persist to the session store.</typeparam>
        /// <param name="session">The session.</param>
        /// <param name="key">The key for the item.</param>
        /// <returns>The item for the key as "T".</returns>
        /// <exception cref="System.NotSupportedException">Only supported by CouchbaseDistributedSession!</exception>
        public static T GetObject<T>(this ISession session, string key)
        {
            var couchbaseSession = session as CouchbaseSession;
            if (couchbaseSession == null)
            {
                throw new NotSupportedException("Only supported by CouchbaseDistributedSession!");
            }

            T value;
            couchbaseSession.TryGetValue(key, out value);
            return value;
        }
    }
}
