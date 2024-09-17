using Couchbase.Extensions.Caching;
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Caching.Memory;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

#nullable enable

namespace Couchbase.Extensions.Session.UnitTests
{
    /// <summary>
    /// A "fake" cache for unit testing Couchbase caching.
    /// </summary>
    internal class CouchbaseInMemoryCache(TimeProvider timeProvider) : ICouchbaseCache
    {
        private readonly Dictionary<string, CacheEntry> _cache = new();

        public bool DisableGet { get; set; }
        public bool DisableSet { get; set; }
        public bool DisableRefresh { get; set; }
        public bool DisableRemove { get; set; }

        public bool DelayGetAsync { get; set; }
        public bool DelaySetAsync { get; set; }
        public bool DelayRefreshAsync { get; set; }
        public bool DelayRemoveAsync { get; set; }

        public byte[]? Get(string key) => GetCacheEntry<byte[]>(key)?.Value;

        public async Task<T?> GetAsync<T>(string key, CancellationToken token = default)
        {
            if (DelayGetAsync)
            {
                await Task.Delay(TimeSpan.FromSeconds(10), timeProvider, token);
            }

            token.ThrowIfCancellationRequested();

            var entry = GetCacheEntry<T>(key);
            return entry is not null ? entry.Value : default;
        }

        public Task<byte[]?> GetAsync(string key, CancellationToken token = default) => GetAsync<byte[]>(key, token);

        public void Refresh(string key) => GetCacheEntry(key, forRefresh: true);

        public async Task RefreshAsync(string key, CancellationToken token = default)
        {
            if (DelayRefreshAsync)
            {
                await Task.Delay(TimeSpan.FromSeconds(10), timeProvider, token);
            }

            GetCacheEntry(key, forRefresh: true);
        }

        public void Remove(string key) => RemoveEntry(key);

        public async Task RemoveAsync(string key, CancellationToken token = default)
        {
            if (DelayRemoveAsync)
            {
                await Task.Delay(TimeSpan.FromSeconds(10), timeProvider, token);
            }

            token.ThrowIfCancellationRequested();

            RemoveEntry(key);
        }

        public void Set(string key, byte[] value, DistributedCacheEntryOptions options)
        {
            SetEntry(key, value, options);
        }

        public async Task SetAsync<T>(string key, T value, DistributedCacheEntryOptions options, CancellationToken token = default)
        {
            if (DelaySetAsync)
            {
                await Task.Delay(TimeSpan.FromSeconds(10), timeProvider, token);
            }

            token.ThrowIfCancellationRequested();

            SetEntry(key, value, options);
        }

        public Task SetAsync(string key, byte[] value, DistributedCacheEntryOptions options, CancellationToken token = default) =>
            SetAsync<byte[]>(key, value, options, token);

        #region Get/Set Helpers

        private CacheEntry? GetCacheEntry(string key, bool forRefresh = false)
        {
            if (forRefresh)
            {
                if (DisableRefresh)
                {
                    throw new InvalidOperationException();
                }
            }
            else if (DisableGet)
            {
                throw new InvalidOperationException();
            }

            if (!_cache.TryGetValue(key, out var entry))
            {
                return null;
            }

            var now = timeProvider.GetUtcNow();
            if (entry.AbsoluteExpiration != null && entry.AbsoluteExpiration.Value < now)
            {
                _cache.Remove(key);
                return null;
            }

            if (entry.SlidingExpiration != null)
            {
                var newExpiration = now.Add(entry.SlidingExpiration.Value);
                if (entry.AbsoluteExpiration is null || newExpiration < entry.AbsoluteExpiration)
                {
                    entry.AbsoluteExpiration = newExpiration;
                }
            }

            return entry;
        }

        private CacheEntry<T>? GetCacheEntry<T>(string key)
        {
            var entry = GetCacheEntry(key);

            return (CacheEntry<T>?)entry;
        }

        private void SetEntry<T>(string key, T value, DistributedCacheEntryOptions options)
        {
            if (DisableSet)
            {
                throw new InvalidOperationException();
            }

            var entry = new CacheEntry<T>(value)
            {
                AbsoluteExpiration = options.AbsoluteExpiration,
                SlidingExpiration = options.SlidingExpiration,
            };

            var now = timeProvider.GetUtcNow();
            if (options.AbsoluteExpirationRelativeToNow is not null)
            {
                var newAbsoluteExpiration = now.Add(options.AbsoluteExpirationRelativeToNow.Value);
                if (entry.AbsoluteExpiration is null || entry.AbsoluteExpiration > newAbsoluteExpiration)
                {
                    entry.AbsoluteExpiration = newAbsoluteExpiration;
                }
            }

            if (entry.SlidingExpiration is not null)
            {
                var newSlidingExpiration = now.Add(entry.SlidingExpiration.Value);
                if (entry.AbsoluteExpiration is null || entry.AbsoluteExpiration > newSlidingExpiration)
                {
                    entry.AbsoluteExpiration = newSlidingExpiration;
                }
            }

            if (entry.AbsoluteExpiration is not null && entry.AbsoluteExpiration <= now)
            {
                // Don't cache if already expired
                return;
            }

            _cache[key] = entry;
        }

        private void RemoveEntry(string key)
        {
            if (DisableRemove)
            {
                throw new InvalidOperationException();
            }

            _cache.Remove(key);
        }

        #endregion

        #region CacheEntry

        private abstract class CacheEntry
        {
            public DateTimeOffset? AbsoluteExpiration { get; set; }
            public TimeSpan? SlidingExpiration { get; init; }
        }

        private sealed class CacheEntry<T>(T value) : CacheEntry
        {
            public T Value { get; } = value;
        }

        #endregion
    }
}
