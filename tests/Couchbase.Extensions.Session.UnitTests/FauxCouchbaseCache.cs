using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Couchbase.Core;
using Couchbase.Extensions.Caching;
using Microsoft.Extensions.Caching.Distributed;

namespace Couchbase.Extensions.Session.UnitTests
{
    public class FauxCouchbaseCache : ICouchbaseCache
    {
        public FauxCouchbaseCache()
        {
            Options = new CouchbaseCacheOptions();
            Bucket = new FauxBucket();
        }

        public bool DisableGet
        {
            get => ((FauxBucket) Bucket).DisableGet;
            set => ((FauxBucket) Bucket).DisableGet = value;
        }

        public bool DisableSetAsync
        {
            get => ((FauxBucket)Bucket).DisableSetAsync;
            set => ((FauxBucket)Bucket).DisableSetAsync = value;
        }

        public bool DisableRefreshAsync
        {
            get => ((FauxBucket)Bucket).DisableRefreshAsync;
            set => ((FauxBucket)Bucket).DisableRefreshAsync = value;
        }

        internal static readonly TimeSpan InfiniteLifetime = TimeSpan.Zero;

        public Dictionary<string, byte[]> _store = new Dictionary<string, byte[]>();

        public byte[] Get(string key)
        {
            return GetAsync(key, CancellationToken.None).GetAwaiter().GetResult();
        }

        public async Task<byte[]> GetAsync(string key, CancellationToken token = new CancellationToken())
        {
            if (DisableGet)
            {
                throw new NotSupportedException();
            }
            return (await Bucket.GetAsync<byte[]>(key)).Value;
        }

        public void Set(string key, byte[] value, DistributedCacheEntryOptions options)
        {
            SetAsync(key, value, options).GetAwaiter().GetResult();
        }

        public async Task SetAsync(string key, byte[] value, DistributedCacheEntryOptions options,
            CancellationToken token = new CancellationToken())
        {
            if (DisableSetAsync)
            {
                throw new InvalidOperationException();
            }
            await Bucket.UpsertAsync(key, value);
        }

        public void Refresh(string key)
        {
            RefreshAsync(key).GetAwaiter().GetResult();
        }

        public async Task RefreshAsync(string key, CancellationToken token = new CancellationToken())
        {
            if (DisableRefreshAsync)
            {
                throw new InvalidOperationException();
            }
            await Bucket.TouchAsync(key, TimeSpan.Zero);
        }

        public void Remove(string key)
        {
            RemoveAsync(key).GetAwaiter().GetResult();
        }

        public Task RemoveAsync(string key, CancellationToken token = new CancellationToken())
        {
            var result = Bucket.RemoveAsync(key);
            return Task.CompletedTask;
        }

        public IBucket Bucket { get; }
        public CouchbaseCacheOptions Options { get; }
        public TimeSpan GetLifetime(DistributedCacheEntryOptions options = null)
        {
            if (options?.SlidingExpiration != null)
            {
                return options.SlidingExpiration.Value;
            }

            return Options.Value.LifeSpan ?? InfiniteLifetime;
        }
    }
}

#region [ License information          ]
/* ************************************************************
 * 
 *    @author Couchbase <info@couchbase.com>
 *    @copyright 2017 Couchbase, Inc.
 *    
 *    Licensed under the Apache License, Version 2.0 (the "License");
 *    you may not use this file except in compliance with the License.
 *    You may obtain a copy of the License at
 *    
 *        http://www.apache.org/licenses/LICENSE-2.0
 *    
 *    Unless required by applicable law or agreed to in writing, software
 *    distributed under the License is distributed on an "AS IS" BASIS,
 *    WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
 *    See the License for the specific language governing permissions and
 *    limitations under the License.
 *    
 * ************************************************************/
#endregion
