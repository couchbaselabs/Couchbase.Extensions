using System;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Session;
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Logging;

namespace Couchbase.Extensions.Session
{
    /// <summary>
    /// The factory for creating sessions when requested.
    /// </summary>
    /// <seealso cref="Microsoft.AspNetCore.Session.ISessionStore" />
    public class CouchbaseSessionStore : ISessionStore
    {
        private readonly IDistributedCache _cache;
        private readonly ILoggerFactory _loggerFactory;

        /// <summary>
        /// Initializes a new instance of the <see cref="CouchbaseSessionStore"/> class.
        /// </summary>
        /// <param name="cache">The cache.</param>
        /// <param name="loggerFactory">The logger factory.</param>
        /// <exception cref="System.ArgumentNullException">
        /// </exception>
        public CouchbaseSessionStore(IDistributedCache cache, ILoggerFactory loggerFactory)
        {
            if (cache == null)
            {
                throw new ArgumentNullException(nameof(cache));
            }
            if (loggerFactory == null)
            {
                throw new ArgumentNullException(nameof(loggerFactory));
            }

            _cache = cache;
            _loggerFactory = loggerFactory;
        }

        /// <summary>
        /// Creates a <see cref="CouchbaseSession"/> for a given key.
        /// </summary>
        /// <param name="sessionKey">The session key.</param>
        /// <param name="idleTimeout">The idle timeout.</param>
        /// <param name="tryEstablishSession">The try establish session.</param>
        /// <param name="isNewSessionKey">if set to <c>true</c> [is new session key].</param>
        /// <returns></returns>
        /// <exception cref="System.ArgumentNullException">
        /// </exception>
        public ISession Create(string sessionKey, TimeSpan idleTimeout, Func<bool> tryEstablishSession, bool isNewSessionKey)
        {
            if (string.IsNullOrWhiteSpace(sessionKey))
            {
                throw new ArgumentNullException(nameof(sessionKey));
            }
            if (tryEstablishSession == null)
            {
                throw new ArgumentNullException(nameof(tryEstablishSession));
            }

            return new CouchbaseSession(_cache, sessionKey, idleTimeout, tryEstablishSession, _loggerFactory, isNewSessionKey);
        }
    }
}
