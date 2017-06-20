using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Security.Cryptography;
using System.Threading.Tasks;
using Couchbase.Extensions.Caching;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;

namespace Couchbase.Extensions.Session
{
    public class CouchbaseSession : ISession
    {
        private static readonly RandomNumberGenerator CryptoRandom = RandomNumberGenerator.Create();
        private const int IdByteCount = 16;
        private readonly IDistributedCache _cache;
        private readonly string _sessionKey;
        private readonly TimeSpan _idleTimeout;
        private readonly Func<bool> _tryEstablishSession;
        private readonly ILogger<CouchbaseSession> _logger;
        private readonly bool _isNewSessionKey;
        private bool _isModified;
        private bool _loaded;
        private bool _isAvailable;
        private string _sessionId;
        private byte[] _sessionIdBytes;
        private Dictionary<string, object> _store;

        public CouchbaseSession(
            IDistributedCache cache,
            string sessionKey,
            TimeSpan idleTimeout,
            Func<bool> tryEstablishSession,
            ILoggerFactory loggerFactory,
            bool isNewSessionKey)
        {
            if (cache == null)
            {
                throw new ArgumentNullException(nameof(cache));
            }

            if (string.IsNullOrEmpty(sessionKey))
            {
                throw new ArgumentException("Argument cannot be null or empty.", nameof(sessionKey));
            }

            if (tryEstablishSession == null)
            {
                throw new ArgumentNullException(nameof(tryEstablishSession));
            }

            if (loggerFactory == null)
            {
                throw new ArgumentNullException(nameof(loggerFactory));
            }

            _cache = cache;
            _sessionKey = sessionKey;
            _idleTimeout = idleTimeout;
            _tryEstablishSession = tryEstablishSession;
            _logger = loggerFactory.CreateLogger<CouchbaseSession>();
            _isNewSessionKey = isNewSessionKey;
            _store = new Dictionary<string, object>(StringComparer.OrdinalIgnoreCase);
        }

        private void Load()
        {
            if (!_loaded)
            {
                try
                {
                    var data = _cache.Get<Dictionary<string, object>>(_sessionKey);
                    if (!_isNewSessionKey && _store == null)
                    {
                        _logger.LogWarning(2, "Accessing expired session, Key:{0}", _sessionKey);
                    }

                    if(data != null)
                    {
                        _store = new Dictionary<string, object>(data, StringComparer.OrdinalIgnoreCase);
                    }
                    _isAvailable = true;
                }
                catch (Exception exception)
                {
                    _logger.LogError(6, exception, "Session cache read exception, Key:{0}", _sessionKey);
                    _isAvailable = false;
                    _sessionId = string.Empty;
                    _sessionIdBytes = null;
                }
                finally
                {
                    _loaded = true;
                }
            }
        }

        public async Task LoadAsync()
        {
            if (!_loaded)
            {
                try
                {
                    var data = await _cache.GetAsync<Dictionary<string, object>>(_sessionKey);
                    if (!_isNewSessionKey && _store == null)
                    {
                        _logger.LogWarning(2, "Accessing expired session, Key:{0}", _sessionKey);
                    }
                    if(data != null)
                    {
                        _store = new Dictionary<string, object>(data, StringComparer.OrdinalIgnoreCase);
                    }
                    _isAvailable = true;
                }
                catch (Exception exception)
                {
                    _logger.LogError(6, exception, "Session cache read exception, Key:{0}", _sessionKey);
                    _isAvailable = false;
                    _sessionId = string.Empty;
                    _sessionIdBytes = null;
                }
                finally
                {
                    _loaded = true;
                }
            }
        }

        public async Task CommitAsync()
        {
            if (_isModified)
            {
                if (_logger.IsEnabled(LogLevel.Information))
                {
                    try
                    {
                        var data = await _cache.GetAsync<Dictionary<string, object>>(_sessionKey);
                        if (data == null)
                        {
                            _logger.LogInformation(3, "Session started; Key:{sessionKey}, Id:{sessionId}", _sessionKey, Id);
                        }
                    }
                    catch (Exception exception)
                    {
                        _logger.LogError(6, "Session cache read exception, Key:{sessionKey}", _sessionKey, exception);
                    }
                }

                await _cache.SetAsync(
                    _sessionKey,
                    _store,
                    new DistributedCacheEntryOptions().SetSlidingExpiration(_idleTimeout));

                _isModified = false;
                _logger.LogDebug(5, "Session stored; Key:{sessionKey}, Id:{sessionId}, Count:{count}",_sessionKey, Id, _store.Count);
            }
            else
            {
                await _cache.RefreshAsync(_sessionKey);
            }
        }

        /// <summary>
        /// Retrieve the value of the given key, if present as a JSON byte array.
        /// </summary>
        /// <param name="key">The key for the item.</param>
        /// <param name="value">The value of the item if found as a JSON byte array.</param>
        /// <returns></returns>
        public bool TryGetValue(string key, out byte[] value)
        {
            Load();
            object item;
            var success = _store.TryGetValue(key, out item);
            if (success)
            {
                if (item.GetType() == typeof(byte[]))
                {
                    value = (byte[]) item;
                }
                else
                {
                    value = ConvertToBytes(item);
                }
            }
            else
            {
                value = null;
            }
            return success;
        }

        internal byte[] ConvertToBytes(object value)
        {
            using (var ms = new MemoryStream())
            {
                using (var sw = new StreamWriter(ms))
                {
                    using (var jr = new JsonTextWriter(sw))
                    {
                        var serializer = JsonSerializer.Create();
                        serializer.Serialize(jr, value);
                    }
                }
                return ms.ToArray();
            }
        }

        public bool TryGetValue<T>(string key, out T value)
        {
            Load();
            object item;
            var success = _store.TryGetValue(key, out item);
            value = (T) item;
            return success;
        }

        public void Set<T>(string key, T value)
        {
            if (value == null)
            {
                throw new ArgumentNullException(nameof(value));
            }
            if (IsAvailable)
            {
                if (!_tryEstablishSession())
                {
                    throw new InvalidOperationException("The session cannot be established after the response has started.");
                }
            }
            _isModified = true;
            _store[key] = value;
        }

        public void Set(string key, byte[] value)
        {
            if (value == null)
            {
                throw new ArgumentNullException(nameof(value));
            }
            if (IsAvailable)
            {
                if (!_tryEstablishSession())
                {
                    throw new InvalidOperationException("The session cannot be established after the response has started.");
                }
            }
            _isModified = true;
            _store[key] = value;
        }

        public void Remove(string key)
        {
            Load();
            _isModified |= _store.Remove(key);
        }

        public void Clear()
        {
            Load();
            _isModified |= _store.Count > 0;
            _store.Clear();
        }

        public bool IsAvailable
        {
            get
            {
                Load();
                return _isAvailable;
            }
        }

        private byte[] IdBytes
        {
            get
            {
                if (IsAvailable && _sessionIdBytes == null)
                {
                    _sessionIdBytes = new byte[IdByteCount];
                    CryptoRandom.GetBytes(_sessionIdBytes);
                }
                return _sessionIdBytes;
            }
        }

        public string Id
        {
            get
            {
                Load();
                return _sessionId ?? (_sessionId = new Guid(IdBytes).ToString());
            }
        }

        public IEnumerable Keys => ((ISession)this).Keys;

        IEnumerable<string> ISession.Keys
        {
            get
            {
                Load();
                return _store.Keys;
            }
        }
    }
}
