using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Security.Cryptography;
using System.Threading;
using System.Threading.Tasks;
using Couchbase.Core.Transcoders;
using Couchbase.Extensions.Caching;
using Couchbase.IO.Operations;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;

namespace Couchbase.Extensions.Session
{
    public class CouchbaseSession : ISession
    {
        readonly DefaultTranscoder _transcoder = new DefaultTranscoder();
        private static readonly RandomNumberGenerator CryptoRandom = RandomNumberGenerator.Create();
        private const int IdByteCount = 16;
        private readonly IDistributedCache _cache;
        private readonly string _sessionKey;
        private readonly TimeSpan _idleTimeout;
        private readonly TimeSpan _ioTimeout;
        private readonly Func<bool> _tryEstablishSession;
        private readonly ILogger<CouchbaseSession> _logger;
        private readonly bool _isNewSessionKey;
        private bool _isModified;
        private bool _loaded;
        private bool _isAvailable;
        private string _sessionId;
        private byte[] _sessionIdBytes;
        private Dictionary<string, byte[]> _store;

        public CouchbaseSession(
            IDistributedCache cache,
            string sessionKey,
            TimeSpan idleTimeout,
            TimeSpan ioTimeout,
            Func<bool> tryEstablishSession,
            ILoggerFactory loggerFactory,
            bool isNewSessionKey)
        {
            if (string.IsNullOrEmpty(sessionKey))
            {
                throw new ArgumentException("Argument cannot be null or empty.", nameof(sessionKey));
            }

            if (loggerFactory == null)
            {
                throw new ArgumentNullException(nameof(loggerFactory));
            }

            _cache = cache ?? throw new ArgumentNullException(nameof(cache));
            _sessionKey = sessionKey;
            _idleTimeout = idleTimeout;
            _ioTimeout = ioTimeout == TimeSpan.Zero ? TimeSpan.FromSeconds(10) : ioTimeout;
            _tryEstablishSession = tryEstablishSession ?? throw new ArgumentNullException(nameof(tryEstablishSession));
            _logger = loggerFactory.CreateLogger<CouchbaseSession>();
            _isNewSessionKey = isNewSessionKey;
            _store = new Dictionary<string, byte[]>();
        }

        private void Load()
        {
           LoadAsync().GetAwaiter().GetResult();
        }

        public async Task LoadAsync(CancellationToken cancellationToken = default(CancellationToken))
        {
            cancellationToken.ThrowIfCancellationRequested();
            if (!_loaded)
            {
                try
                {
                    using (var timeout = new CancellationTokenSource(_ioTimeout))
                    {
                        var cts = CancellationTokenSource.CreateLinkedTokenSource(timeout.Token, cancellationToken);
                        var data = await _cache.GetAsync<Dictionary<string, byte[]>>(_sessionKey, null, cts.Token);
                        if (data != null)
                        {
                            _store = new Dictionary<string, byte[]>(data, StringComparer.OrdinalIgnoreCase);
                        }
                        else if (!_isNewSessionKey)
                        {
                            _logger.LogWarning(2, "Accessing expired session, Key:{0}", _sessionKey);
                        }
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

        public async Task CommitAsync(CancellationToken token)
        {
            token.ThrowIfCancellationRequested();
            if (_isModified)
            {
                if (_logger.IsEnabled(LogLevel.Information))
                {
                    try
                    {
                        using (var timeout = new CancellationTokenSource(_ioTimeout))
                        {
                            var cts = CancellationTokenSource.CreateLinkedTokenSource(timeout.Token, token);
                            var data = await _cache.GetAsync<Dictionary<string, byte[]>>(_sessionKey, cts.Token);
                            if (data == null)
                            {
                                _logger.LogInformation(3, "Session started; Key:{sessionKey}, Id:{sessionId}",
                                    _sessionKey, Id);
                            }
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
                await _cache.RefreshAsync(_sessionKey, token);
            }
        }

        /// <inheritdoc />
        public bool TryGetValue(string key, out byte[] value)
        {
            Load();
            var success = _store.TryGetValue(key, out var item);
            if (success)
            {
                value = item.GetType() == typeof(byte[]) ? item : ConvertToBytes(item);
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
            value = default(T);
            var success = _store.TryGetValue(key, out var item);
            value = _transcoder.Decode<T>(item, 0, item.Length, new Flags(), OperationCode.NoOp);
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
            _store[key] = ConvertToBytes(value);
        }

        public void Set(string key, byte[] value)
        {
            if (IsAvailable)
            {
                if (!_tryEstablishSession())
                {
                    throw new InvalidOperationException("The session cannot be established after the response has started.");
                }
            }
            _isModified = true;
            _store[key] = value ?? throw new ArgumentNullException(nameof(value));
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
