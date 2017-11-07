using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Couchbase.Analytics;
using Couchbase.Configuration.Client;
using Couchbase.Core;
using Couchbase.Core.Buckets;
using Couchbase.Core.Serialization;
using Couchbase.Core.Version;
using Couchbase.IO.Operations;
using Couchbase.Management;
using Couchbase.N1QL;
using Couchbase.Search;
using Couchbase.Views;

namespace Couchbase.Extensions.Session.UnitTests
{
    public class FauxBucket : IBucket
    {
        private Dictionary<string, BucketItem> _store = new Dictionary<string, BucketItem>();
        private DefaultSerializer _serializer = new DefaultSerializer();

        public bool DisableGet { get; set; }

        public bool DisableSetAsync { get; set; }

        public bool DisableRefreshAsync { get; set; }

        public void Dispose()
        {
            throw new NotImplementedException();
        }

        public bool Exists(string key)
        {
            throw new NotImplementedException();
        }

        public bool Exists(string key, TimeSpan timeout)
        {
            throw new NotImplementedException();
        }

        public Task<bool> ExistsAsync(string key)
        {
            throw new NotImplementedException();
        }

        public Task<bool> ExistsAsync(string key, TimeSpan timeout)
        {
            throw new NotImplementedException();
        }

        public Task<ObserveResponse> ObserveAsync(string key, ulong cas, bool deletion, ReplicateTo replicateTo, PersistTo persistTo)
        {
            throw new NotImplementedException();
        }

        public Task<ObserveResponse> ObserveAsync(string key, ulong cas, bool deletion, ReplicateTo replicateTo, PersistTo persistTo, TimeSpan timeout)
        {
            throw new NotImplementedException();
        }

        public ObserveResponse Observe(string key, ulong cas, bool deletion, ReplicateTo replicateTo, PersistTo persistTo)
        {
            throw new NotImplementedException();
        }

        public ObserveResponse Observe(string key, ulong cas, bool deletion, ReplicateTo replicateTo, PersistTo persistTo,
            TimeSpan timeout)
        {
            throw new NotImplementedException();
        }

        public IDocumentResult<T> Upsert<T>(IDocument<T> document)
        {
            throw new NotImplementedException();
        }

        public IDocumentResult<T> Upsert<T>(IDocument<T> document, TimeSpan timeout)
        {
            throw new NotImplementedException();
        }

        public Task<IDocumentResult<T>> UpsertAsync<T>(IDocument<T> document)
        {
            throw new NotImplementedException();
        }

        public Task<IDocumentResult<T>> UpsertAsync<T>(IDocument<T> document, TimeSpan timeout)
        {
            throw new NotImplementedException();
        }

        public IDocumentResult<T> Upsert<T>(IDocument<T> document, ReplicateTo replicateTo)
        {
            throw new NotImplementedException();
        }

        public IDocumentResult<T> Upsert<T>(IDocument<T> document, ReplicateTo replicateTo, TimeSpan timeout)
        {
            throw new NotImplementedException();
        }

        public Task<IDocumentResult<T>> UpsertAsync<T>(IDocument<T> document, ReplicateTo replicateTo)
        {
            throw new NotImplementedException();
        }

        public Task<IDocumentResult<T>> UpsertAsync<T>(IDocument<T> document, ReplicateTo replicateTo, TimeSpan timeout)
        {
            throw new NotImplementedException();
        }

        public IDocumentResult<T> Upsert<T>(IDocument<T> document, ReplicateTo replicateTo, PersistTo persistTo)
        {
            throw new NotImplementedException();
        }

        public IDocumentResult<T> Upsert<T>(IDocument<T> document, ReplicateTo replicateTo, PersistTo persistTo, TimeSpan timeout)
        {
            throw new NotImplementedException();
        }

        public Task<IDocumentResult<T>> UpsertAsync<T>(IDocument<T> document, ReplicateTo replicateTo, PersistTo persistTo)
        {
            throw new NotImplementedException();
        }

        public Task<IDocumentResult<T>> UpsertAsync<T>(IDocument<T> document, ReplicateTo replicateTo, PersistTo persistTo, TimeSpan timeout)
        {
            throw new NotImplementedException();
        }

        public Task<IDocumentResult<T>[]> UpsertAsync<T>(List<IDocument<T>> documents)
        {
            throw new NotImplementedException();
        }

        public Task<IDocumentResult<T>[]> UpsertAsync<T>(List<IDocument<T>> documents, TimeSpan timeout)
        {
            throw new NotImplementedException();
        }

        public Task<IDocumentResult<T>[]> UpsertAsync<T>(List<IDocument<T>> documents, ReplicateTo replicateTo)
        {
            throw new NotImplementedException();
        }

        public Task<IDocumentResult<T>[]> UpsertAsync<T>(List<IDocument<T>> documents, ReplicateTo replicateTo, TimeSpan timeout)
        {
            throw new NotImplementedException();
        }

        public Task<IDocumentResult<T>[]> UpsertAsync<T>(List<IDocument<T>> documents, ReplicateTo replicateTo, PersistTo persistTo)
        {
            throw new NotImplementedException();
        }

        public Task<IDocumentResult<T>[]> UpsertAsync<T>(List<IDocument<T>> documents, ReplicateTo replicateTo, PersistTo persistTo, TimeSpan timeout)
        {
            throw new NotImplementedException();
        }

        public IOperationResult<T> Upsert<T>(string key, T value)
        {
            return UpsertAsync(key, value, TimeSpan.Zero).GetAwaiter().GetResult();
        }

        public Task<IOperationResult<T>> UpsertAsync<T>(string key, T value)
        {
            return UpsertAsync(key, value, TimeSpan.Zero);
        }

        public IOperationResult<T> Upsert<T>(string key, T value, uint expiration)
        {
            throw new NotImplementedException();
        }

        public IOperationResult<T> Upsert<T>(string key, T value, uint expiration, TimeSpan timeout)
        {
            throw new NotImplementedException();
        }

        public Task<IOperationResult<T>> UpsertAsync<T>(string key, T value, uint expiration)
        {
            throw new NotImplementedException();
        }

        public Task<IOperationResult<T>> UpsertAsync<T>(string key, T value, uint expiration, TimeSpan timeout)
        {
            throw new NotImplementedException();
        }

        public IOperationResult<T> Upsert<T>(string key, T value, TimeSpan expiration)
        {
            return UpsertAsync(key, value, expiration).GetAwaiter().GetResult();
        }

        public IOperationResult<T> Upsert<T>(string key, T value, TimeSpan expiration, TimeSpan timeout)
        {
            throw new NotImplementedException();
        }

        public Task<IOperationResult<T>> UpsertAsync<T>(string key, T value, TimeSpan expiration)
        {
            if (DisableSetAsync)
            {
                throw new NotSupportedException();
            }
            _store[key] = new BucketItem(_store, key, _serializer.Serialize(value), expiration);
            return Task.FromResult((IOperationResult<T>)new FauxOperationResult<T>
            {
                Success = true
            });
        }

        public Task<IOperationResult<T>> UpsertAsync<T>(string key, T value, TimeSpan expiration, TimeSpan timeout)
        {
            throw new NotImplementedException();
        }

        public IOperationResult<T> Upsert<T>(string key, T value, ulong cas)
        {
            throw new NotImplementedException();
        }

        public Task<IOperationResult<T>> UpsertAsync<T>(string key, T value, ulong cas)
        {
            throw new NotImplementedException();
        }

        public IOperationResult<T> Upsert<T>(string key, T value, ulong cas, uint expiration)
        {
            throw new NotImplementedException();
        }

        public IOperationResult<T> Upsert<T>(string key, T value, ulong cas, uint expiration, TimeSpan timeout)
        {
            throw new NotImplementedException();
        }

        public Task<IOperationResult<T>> UpsertAsync<T>(string key, T value, ulong cas, uint expiration)
        {
            throw new NotImplementedException();
        }

        public Task<IOperationResult<T>> UpsertAsync<T>(string key, T value, ulong cas, uint expiration, TimeSpan timeout)
        {
            throw new NotImplementedException();
        }

        public IOperationResult<T> Upsert<T>(string key, T value, ulong cas, TimeSpan expiration)
        {
            throw new NotImplementedException();
        }

        public IOperationResult<T> Upsert<T>(string key, T value, ulong cas, TimeSpan expiration, TimeSpan timeout)
        {
            throw new NotImplementedException();
        }

        public Task<IOperationResult<T>> UpsertAsync<T>(string key, T value, ulong cas, TimeSpan expiration)
        {
            throw new NotImplementedException();
        }

        public Task<IOperationResult<T>> UpsertAsync<T>(string key, T value, ulong cas, TimeSpan expiration, TimeSpan timeout)
        {
            throw new NotImplementedException();
        }

        public IOperationResult<T> Upsert<T>(string key, T value, ReplicateTo replicateTo)
        {
            throw new NotImplementedException();
        }

        public IOperationResult<T> Upsert<T>(string key, T value, ReplicateTo replicateTo, TimeSpan timeout)
        {
            throw new NotImplementedException();
        }

        public Task<IOperationResult<T>> UpsertAsync<T>(string key, T value, ReplicateTo replicateTo)
        {
            throw new NotImplementedException();
        }

        public Task<IOperationResult<T>> UpsertAsync<T>(string key, T value, ReplicateTo replicateTo, TimeSpan timeout)
        {
            throw new NotImplementedException();
        }

        public IOperationResult<T> Upsert<T>(string key, T value, ReplicateTo replicateTo, PersistTo persistTo)
        {
            throw new NotImplementedException();
        }

        public IOperationResult<T> Upsert<T>(string key, T value, ReplicateTo replicateTo, PersistTo persistTo, TimeSpan timeout)
        {
            throw new NotImplementedException();
        }

        public Task<IOperationResult<T>> UpsertAsync<T>(string key, T value, ReplicateTo replicateTo, PersistTo persistTo)
        {
            throw new NotImplementedException();
        }

        public Task<IOperationResult<T>> UpsertAsync<T>(string key, T value, ReplicateTo replicateTo, PersistTo persistTo, TimeSpan timeout)
        {
            throw new NotImplementedException();
        }

        public IOperationResult<T> Upsert<T>(string key, T value, uint expiration, ReplicateTo replicateTo, PersistTo persistTo,
            TimeSpan timeout)
        {
            throw new NotImplementedException();
        }

        public Task<IOperationResult<T>> UpsertAsync<T>(string key, T value, uint expiration, ReplicateTo replicateTo, PersistTo persistTo)
        {
            throw new NotImplementedException();
        }

        public Task<IOperationResult<T>> UpsertAsync<T>(string key, T value, uint expiration, ReplicateTo replicateTo, PersistTo persistTo,
            TimeSpan timeout)
        {
            throw new NotImplementedException();
        }

        public IOperationResult<T> Upsert<T>(string key, T value, ulong cas, uint expiration, ReplicateTo replicateTo,
            PersistTo persistTo)
        {
            throw new NotImplementedException();
        }

        public IOperationResult<T> Upsert<T>(string key, T value, ulong cas, uint expiration, ReplicateTo replicateTo,
            PersistTo persistTo, TimeSpan timeout)
        {
            throw new NotImplementedException();
        }

        public Task<IOperationResult<T>> UpsertAsync<T>(string key, T value, ulong cas, uint expiration, ReplicateTo replicateTo, PersistTo persistTo)
        {
            throw new NotImplementedException();
        }

        public Task<IOperationResult<T>> UpsertAsync<T>(string key, T value, ulong cas, uint expiration, ReplicateTo replicateTo, PersistTo persistTo,
            TimeSpan timeout)
        {
            throw new NotImplementedException();
        }

        public IOperationResult<T> Upsert<T>(string key, T value, TimeSpan expiration, ReplicateTo replicateTo, PersistTo persistTo)
        {
            throw new NotImplementedException();
        }

        public IOperationResult<T> Upsert<T>(string key, T value, TimeSpan expiration, ReplicateTo replicateTo, PersistTo persistTo,
            TimeSpan timeout)
        {
            throw new NotImplementedException();
        }

        public Task<IOperationResult<T>> UpsertAsync<T>(string key, T value, TimeSpan expiration, ReplicateTo replicateTo, PersistTo persistTo)
        {
            throw new NotImplementedException();
        }

        public Task<IOperationResult<T>> UpsertAsync<T>(string key, T value, TimeSpan expiration, ReplicateTo replicateTo, PersistTo persistTo,
            TimeSpan timeout)
        {
            throw new NotImplementedException();
        }

        public IOperationResult<T> Upsert<T>(string key, T value, ulong cas, TimeSpan expiration, ReplicateTo replicateTo,
            PersistTo persistTo)
        {
            throw new NotImplementedException();
        }

        public IOperationResult<T> Upsert<T>(string key, T value, ulong cas, TimeSpan expiration, ReplicateTo replicateTo,
            PersistTo persistTo, TimeSpan timeout)
        {
            throw new NotImplementedException();
        }

        public Task<IOperationResult<T>> UpsertAsync<T>(string key, T value, ulong cas, TimeSpan expiration, ReplicateTo replicateTo, PersistTo persistTo)
        {
            throw new NotImplementedException();
        }

        public Task<IOperationResult<T>> UpsertAsync<T>(string key, T value, ulong cas, TimeSpan expiration, ReplicateTo replicateTo, PersistTo persistTo,
            TimeSpan timeout)
        {
            throw new NotImplementedException();
        }

        public IDictionary<string, IOperationResult<T>> Upsert<T>(IDictionary<string, T> items)
        {
            throw new NotImplementedException();
        }

        public IDictionary<string, IOperationResult<T>> Upsert<T>(IDictionary<string, T> items, TimeSpan timeout)
        {
            throw new NotImplementedException();
        }

        public IDictionary<string, IOperationResult<T>> Upsert<T>(IDictionary<string, T> items, ParallelOptions options)
        {
            throw new NotImplementedException();
        }

        public IDictionary<string, IOperationResult<T>> Upsert<T>(IDictionary<string, T> items, ParallelOptions options, TimeSpan timeout)
        {
            throw new NotImplementedException();
        }

        public IDictionary<string, IOperationResult<T>> Upsert<T>(IDictionary<string, T> items, ParallelOptions options, int rangeSize)
        {
            throw new NotImplementedException();
        }

        public IDictionary<string, IOperationResult<T>> Upsert<T>(IDictionary<string, T> items, ParallelOptions options, int rangeSize, TimeSpan timeout)
        {
            throw new NotImplementedException();
        }

        public IDocumentResult<T> Replace<T>(IDocument<T> document)
        {
            throw new NotImplementedException();
        }

        public IDocumentResult<T> Replace<T>(IDocument<T> document, TimeSpan timeout)
        {
            throw new NotImplementedException();
        }

        public Task<IDocumentResult<T>> ReplaceAsync<T>(IDocument<T> document)
        {
            throw new NotImplementedException();
        }

        public Task<IDocumentResult<T>> ReplaceAsync<T>(IDocument<T> document, TimeSpan timeout)
        {
            throw new NotImplementedException();
        }

        public IDocumentResult<T> Replace<T>(IDocument<T> document, ReplicateTo replicateTo)
        {
            throw new NotImplementedException();
        }

        public IDocumentResult<T> Replace<T>(IDocument<T> document, ReplicateTo replicateTo, TimeSpan timeout)
        {
            throw new NotImplementedException();
        }

        public Task<IDocumentResult<T>> ReplaceAsync<T>(IDocument<T> document, ReplicateTo replicateTo)
        {
            throw new NotImplementedException();
        }

        public Task<IDocumentResult<T>> ReplaceAsync<T>(IDocument<T> document, ReplicateTo replicateTo, TimeSpan timeout)
        {
            throw new NotImplementedException();
        }

        public Task<IDocumentResult<T>[]> ReplaceAsync<T>(List<IDocument<T>> documents)
        {
            throw new NotImplementedException();
        }

        public Task<IDocumentResult<T>[]> ReplaceAsync<T>(List<IDocument<T>> documents, TimeSpan timeout)
        {
            throw new NotImplementedException();
        }

        public Task<IDocumentResult<T>[]> ReplaceAsync<T>(List<IDocument<T>> documents, ReplicateTo replicateTo)
        {
            throw new NotImplementedException();
        }

        public Task<IDocumentResult<T>[]> ReplaceAsync<T>(List<IDocument<T>> documents, ReplicateTo replicateTo, TimeSpan timeout)
        {
            throw new NotImplementedException();
        }

        public Task<IDocumentResult<T>[]> ReplaceAsync<T>(List<IDocument<T>> documents, ReplicateTo replicateTo, PersistTo persistTo)
        {
            throw new NotImplementedException();
        }

        public IDocumentResult<T> Replace<T>(IDocument<T> document, ReplicateTo replicateTo, PersistTo persistTo)
        {
            throw new NotImplementedException();
        }

        public IDocumentResult<T> Replace<T>(IDocument<T> document, ReplicateTo replicateTo, PersistTo persistTo, TimeSpan timeout)
        {
            throw new NotImplementedException();
        }

        public Task<IDocumentResult<T>> ReplaceAsync<T>(IDocument<T> document, ReplicateTo replicateTo, PersistTo persistTo)
        {
            throw new NotImplementedException();
        }

        public Task<IDocumentResult<T>> ReplaceAsync<T>(IDocument<T> document, ReplicateTo replicateTo, PersistTo persistTo, TimeSpan timeout)
        {
            throw new NotImplementedException();
        }

        public IOperationResult<T> Replace<T>(string key, T value)
        {
            throw new NotImplementedException();
        }

        public Task<IOperationResult<T>> ReplaceAsync<T>(string key, T value)
        {
            throw new NotImplementedException();
        }

        public IOperationResult<T> Replace<T>(string key, T value, uint expiration)
        {
            throw new NotImplementedException();
        }

        public IOperationResult<T> Replace<T>(string key, T value, uint expiration, TimeSpan timeout)
        {
            throw new NotImplementedException();
        }

        public Task<IOperationResult<T>> ReplaceAsync<T>(string key, T value, uint expiration)
        {
            throw new NotImplementedException();
        }

        public Task<IOperationResult<T>> ReplaceAsync<T>(string key, T value, uint expiration, TimeSpan timeout)
        {
            throw new NotImplementedException();
        }

        public IOperationResult<T> Replace<T>(string key, T value, TimeSpan expiration)
        {
            throw new NotImplementedException();
        }

        public IOperationResult<T> Replace<T>(string key, T value, TimeSpan expiration, TimeSpan timeout)
        {
            throw new NotImplementedException();
        }

        public Task<IOperationResult<T>> ReplaceAsync<T>(string key, T value, TimeSpan expiration)
        {
            throw new NotImplementedException();
        }

        public Task<IOperationResult<T>> ReplaceAsync<T>(string key, T value, TimeSpan expiration, TimeSpan timeout)
        {
            throw new NotImplementedException();
        }

        public IOperationResult<T> Replace<T>(string key, T value, ulong cas)
        {
            throw new NotImplementedException();
        }

        public Task<IOperationResult<T>> ReplaceAsync<T>(string key, T value, ulong cas)
        {
            throw new NotImplementedException();
        }

        public IOperationResult<T> Replace<T>(string key, T value, ulong cas, uint expiration)
        {
            throw new NotImplementedException();
        }

        public IOperationResult<T> Replace<T>(string key, T value, ulong cas, uint expiration, TimeSpan timeout)
        {
            throw new NotImplementedException();
        }

        public Task<IOperationResult<T>> ReplaceAsync<T>(string key, T value, ulong cas, uint expiration)
        {
            throw new NotImplementedException();
        }

        public Task<IOperationResult<T>> ReplaceAsync<T>(string key, T value, ulong cas, uint expiration, TimeSpan timeout)
        {
            throw new NotImplementedException();
        }

        public IOperationResult<T> Replace<T>(string key, T value, ulong cas, TimeSpan expiration)
        {
            throw new NotImplementedException();
        }

        public IOperationResult<T> Replace<T>(string key, T value, ulong cas, TimeSpan expiration, TimeSpan timeout)
        {
            throw new NotImplementedException();
        }

        public Task<IOperationResult<T>> ReplaceAsync<T>(string key, T value, ulong cas, TimeSpan expiration)
        {
            throw new NotImplementedException();
        }

        public Task<IOperationResult<T>> ReplaceAsync<T>(string key, T value, ulong cas, TimeSpan expiration, TimeSpan timeout)
        {
            throw new NotImplementedException();
        }

        public IOperationResult<T> Replace<T>(string key, T value, ReplicateTo replicateTo)
        {
            throw new NotImplementedException();
        }

        public IOperationResult<T> Replace<T>(string key, T value, ReplicateTo replicateTo, TimeSpan timeout)
        {
            throw new NotImplementedException();
        }

        public Task<IOperationResult<T>> ReplaceAsync<T>(string key, T value, ReplicateTo replicateTo)
        {
            throw new NotImplementedException();
        }

        public Task<IOperationResult<T>> ReplaceAsync<T>(string key, T value, ReplicateTo replicateTo, TimeSpan timeout)
        {
            throw new NotImplementedException();
        }

        public IOperationResult<T> Replace<T>(string key, T value, ulong cas, ReplicateTo replicateTo)
        {
            throw new NotImplementedException();
        }

        public IOperationResult<T> Replace<T>(string key, T value, ulong cas, ReplicateTo replicateTo, TimeSpan timeout)
        {
            throw new NotImplementedException();
        }

        public Task<IOperationResult<T>> ReplaceAsync<T>(string key, T value, ulong cas, ReplicateTo replicateTo)
        {
            throw new NotImplementedException();
        }

        public Task<IOperationResult<T>> ReplaceAsync<T>(string key, T value, ulong cas, ReplicateTo replicateTo, TimeSpan timeout)
        {
            throw new NotImplementedException();
        }

        public IOperationResult<T> Replace<T>(string key, T value, ReplicateTo replicateTo, PersistTo persistTo)
        {
            throw new NotImplementedException();
        }

        public IOperationResult<T> Replace<T>(string key, T value, ReplicateTo replicateTo, PersistTo persistTo, TimeSpan timeout)
        {
            throw new NotImplementedException();
        }

        public Task<IOperationResult<T>> ReplaceAsync<T>(string key, T value, ReplicateTo replicateTo, PersistTo persistTo)
        {
            throw new NotImplementedException();
        }

        public Task<IOperationResult<T>> ReplaceAsync<T>(string key, T value, ReplicateTo replicateTo, PersistTo persistTo, TimeSpan timeout)
        {
            throw new NotImplementedException();
        }

        public IOperationResult<T> Replace<T>(string key, T value, ulong cas, ReplicateTo replicateTo, PersistTo persistTo)
        {
            throw new NotImplementedException();
        }

        public IOperationResult<T> Replace<T>(string key, T value, ulong cas, ReplicateTo replicateTo, PersistTo persistTo,
            TimeSpan timeout)
        {
            throw new NotImplementedException();
        }

        public Task<IOperationResult<T>> ReplaceAsync<T>(string key, T value, ulong cas, ReplicateTo replicateTo, PersistTo persistTo)
        {
            throw new NotImplementedException();
        }

        public Task<IOperationResult<T>> ReplaceAsync<T>(string key, T value, ulong cas, ReplicateTo replicateTo, PersistTo persistTo, TimeSpan timeout)
        {
            throw new NotImplementedException();
        }

        public IOperationResult<T> Replace<T>(string key, T value, ulong cas, uint expiration, ReplicateTo replicateTo,
            PersistTo persistTo)
        {
            throw new NotImplementedException();
        }

        public IOperationResult<T> Replace<T>(string key, T value, ulong cas, uint expiration, ReplicateTo replicateTo,
            PersistTo persistTo, TimeSpan timeout)
        {
            throw new NotImplementedException();
        }

        public Task<IOperationResult<T>> ReplaceAsync<T>(string key, T value, ulong cas, uint expiration, ReplicateTo replicateTo, PersistTo persistTo)
        {
            throw new NotImplementedException();
        }

        public Task<IOperationResult<T>> ReplaceAsync<T>(string key, T value, ulong cas, uint expiration, ReplicateTo replicateTo, PersistTo persistTo,
            TimeSpan timeout)
        {
            throw new NotImplementedException();
        }

        public IOperationResult<T> Replace<T>(string key, T value, ulong cas, TimeSpan expiration, ReplicateTo replicateTo,
            PersistTo persistTo)
        {
            throw new NotImplementedException();
        }

        public IOperationResult<T> Replace<T>(string key, T value, ulong cas, TimeSpan expiration, ReplicateTo replicateTo,
            PersistTo persistTo, TimeSpan timeout)
        {
            throw new NotImplementedException();
        }

        public Task<IOperationResult<T>> ReplaceAsync<T>(string key, T value, ulong cas, TimeSpan expiration, ReplicateTo replicateTo, PersistTo persistTo)
        {
            throw new NotImplementedException();
        }

        public Task<IOperationResult<T>> ReplaceAsync<T>(string key, T value, ulong cas, TimeSpan expiration, ReplicateTo replicateTo, PersistTo persistTo,
            TimeSpan timeout)
        {
            throw new NotImplementedException();
        }

        public IDocumentResult<T> Insert<T>(IDocument<T> document)
        {
            throw new NotImplementedException();
        }

        public IDocumentResult<T> Insert<T>(IDocument<T> document, TimeSpan timeout)
        {
            throw new NotImplementedException();
        }

        public Task<IDocumentResult<T>[]> InsertAsync<T>(List<IDocument<T>> documents)
        {
            throw new NotImplementedException();
        }

        public Task<IDocumentResult<T>[]> InsertAsync<T>(List<IDocument<T>> documents, TimeSpan timeout)
        {
            throw new NotImplementedException();
        }

        public Task<IDocumentResult<T>[]> InsertAsync<T>(List<IDocument<T>> documents, ReplicateTo replicateTo)
        {
            throw new NotImplementedException();
        }

        public Task<IDocumentResult<T>[]> InsertAsync<T>(List<IDocument<T>> documents, ReplicateTo replicateTo, TimeSpan timeout)
        {
            throw new NotImplementedException();
        }

        public Task<IDocumentResult<T>[]> InsertAsync<T>(List<IDocument<T>> documents, ReplicateTo replicateTo, PersistTo persistTo)
        {
            throw new NotImplementedException();
        }

        public Task<IDocumentResult<T>[]> InsertAsync<T>(List<IDocument<T>> documents, ReplicateTo replicateTo, PersistTo persistTo, TimeSpan timeout)
        {
            throw new NotImplementedException();
        }

        public Task<IDocumentResult<T>> InsertAsync<T>(IDocument<T> document)
        {
            throw new NotImplementedException();
        }

        public Task<IDocumentResult<T>> InsertAsync<T>(IDocument<T> document, TimeSpan timeout)
        {
            throw new NotImplementedException();
        }

        public IDocumentResult<T> Insert<T>(IDocument<T> document, ReplicateTo replicateTo)
        {
            throw new NotImplementedException();
        }

        public IDocumentResult<T> Insert<T>(IDocument<T> document, ReplicateTo replicateTo, TimeSpan timeout)
        {
            throw new NotImplementedException();
        }

        public Task<IDocumentResult<T>> InsertAsync<T>(IDocument<T> document, ReplicateTo replicateTo)
        {
            throw new NotImplementedException();
        }

        public Task<IDocumentResult<T>> InsertAsync<T>(IDocument<T> document, ReplicateTo replicateTo, TimeSpan timeout)
        {
            throw new NotImplementedException();
        }

        public IDocumentResult<T> Insert<T>(IDocument<T> document, ReplicateTo replicateTo, PersistTo persistTo)
        {
            throw new NotImplementedException();
        }

        public IDocumentResult<T> Insert<T>(IDocument<T> document, ReplicateTo replicateTo, PersistTo persistTo, TimeSpan timeout)
        {
            throw new NotImplementedException();
        }

        public Task<IDocumentResult<T>> InsertAsync<T>(IDocument<T> document, ReplicateTo replicateTo, PersistTo persistTo)
        {
            throw new NotImplementedException();
        }

        public Task<IDocumentResult<T>> InsertAsync<T>(IDocument<T> document, ReplicateTo replicateTo, PersistTo persistTo, TimeSpan timeout)
        {
            throw new NotImplementedException();
        }

        public IOperationResult<T> Insert<T>(string key, T value)
        {
            return InsertAsync(key, value).GetAwaiter().GetResult();
        }

        public Task<IOperationResult<T>> InsertAsync<T>(string key, T value)
        {
            return InsertAsync(key, value, TimeSpan.Zero);
        }

        public IOperationResult<T> Insert<T>(string key, T value, uint expiration)
        {
            throw new NotImplementedException();
        }

        public IOperationResult<T> Insert<T>(string key, T value, uint expiration, TimeSpan timeout)
        {
            throw new NotImplementedException();
        }

        public Task<IOperationResult<T>> InsertAsync<T>(string key, T value, uint expiration)
        {
            if (_store.TryAdd(key, new BucketItem(_store, key, _serializer.Serialize(value), TimeSpan.FromSeconds(expiration))))
            {
                return Task.FromResult((IOperationResult<T>)new FauxOperationResult<T>
                {
                    Success = true
                });
            }
            return Task.FromResult((IOperationResult<T>)new FauxOperationResult<T>
            {
                Success = false
            });
        }

        public Task<IOperationResult<T>> InsertAsync<T>(string key, T value, uint expiration, TimeSpan timeout)
        {
            throw new NotImplementedException();
        }

        public IOperationResult<T> Insert<T>(string key, T value, TimeSpan expiration)
        {
            throw new NotImplementedException();
        }

        public IOperationResult<T> Insert<T>(string key, T value, TimeSpan expiration, TimeSpan timeout)
        {
            throw new NotImplementedException();
        }

        public Task<IOperationResult<T>> InsertAsync<T>(string key, T value, TimeSpan expiration)
        {
            return InsertAsync(key, value, (uint)expiration.TotalSeconds);
        }

        public Task<IOperationResult<T>> InsertAsync<T>(string key, T value, TimeSpan expiration, TimeSpan timeout)
        {
            throw new NotImplementedException();
        }

        public IOperationResult<T> Insert<T>(string key, T value, ReplicateTo replicateTo)
        {
            throw new NotImplementedException();
        }

        public IOperationResult<T> Insert<T>(string key, T value, ReplicateTo replicateTo, TimeSpan timeout)
        {
            throw new NotImplementedException();
        }

        public Task<IOperationResult<T>> InsertAsync<T>(string key, T value, ReplicateTo replicateTo)
        {
            throw new NotImplementedException();
        }

        public Task<IOperationResult<T>> InsertAsync<T>(string key, T value, ReplicateTo replicateTo, TimeSpan timeout)
        {
            throw new NotImplementedException();
        }

        public IOperationResult<T> Insert<T>(string key, T value, ReplicateTo replicateTo, PersistTo persistTo)
        {
            throw new NotImplementedException();
        }

        public IOperationResult<T> Insert<T>(string key, T value, ReplicateTo replicateTo, PersistTo persistTo, TimeSpan timeout)
        {
            throw new NotImplementedException();
        }

        public Task<IOperationResult<T>> InsertAsync<T>(string key, T value, ReplicateTo replicateTo, PersistTo persistTo)
        {
            throw new NotImplementedException();
        }

        public Task<IOperationResult<T>> InsertAsync<T>(string key, T value, ReplicateTo replicateTo, PersistTo persistTo, TimeSpan timeout)
        {
            throw new NotImplementedException();
        }

        public IOperationResult<T> Insert<T>(string key, T value, uint expiration, ReplicateTo replicateTo, PersistTo persistTo)
        {
            throw new NotImplementedException();
        }

        public IOperationResult<T> Insert<T>(string key, T value, uint expiration, ReplicateTo replicateTo, PersistTo persistTo,
            TimeSpan timeout)
        {
            throw new NotImplementedException();
        }

        public Task<IOperationResult<T>> InsertAsync<T>(string key, T value, uint expiration, ReplicateTo replicateTo, PersistTo persistTo)
        {
            throw new NotImplementedException();
        }

        public Task<IOperationResult<T>> InsertAsync<T>(string key, T value, uint expiration, ReplicateTo replicateTo, PersistTo persistTo,
            TimeSpan timeout)
        {
            throw new NotImplementedException();
        }

        public IOperationResult<T> Insert<T>(string key, T value, TimeSpan expiration, ReplicateTo replicateTo, PersistTo persistTo)
        {
            throw new NotImplementedException();
        }

        public IOperationResult<T> Insert<T>(string key, T value, TimeSpan expiration, ReplicateTo replicateTo, PersistTo persistTo,
            TimeSpan timeout)
        {
            throw new NotImplementedException();
        }

        public Task<IOperationResult<T>> InsertAsync<T>(string key, T value, TimeSpan expiration, ReplicateTo replicateTo, PersistTo persistTo)
        {
            throw new NotImplementedException();
        }

        public Task<IOperationResult<T>> InsertAsync<T>(string key, T value, TimeSpan expiration, ReplicateTo replicateTo, PersistTo persistTo,
            TimeSpan timeout)
        {
            throw new NotImplementedException();
        }

        public IOperationResult Remove<T>(IDocument<T> document)
        {
            throw new NotImplementedException();
        }

        public IOperationResult Remove<T>(IDocument<T> document, TimeSpan timeout)
        {
            throw new NotImplementedException();
        }

        public Task<IOperationResult> RemoveAsync<T>(IDocument<T> document)
        {
            throw new NotImplementedException();
        }

        public Task<IOperationResult> RemoveAsync<T>(IDocument<T> document, TimeSpan timeout)
        {
            throw new NotImplementedException();
        }

        public IOperationResult Remove<T>(IDocument<T> document, ReplicateTo replicateTo)
        {
            throw new NotImplementedException();
        }

        public IOperationResult Remove<T>(IDocument<T> document, ReplicateTo replicateTo, TimeSpan timeout)
        {
            throw new NotImplementedException();
        }

        public Task<IOperationResult> RemoveAsync<T>(IDocument<T> document, ReplicateTo replicateTo)
        {
            throw new NotImplementedException();
        }

        public Task<IOperationResult> RemoveAsync<T>(IDocument<T> document, ReplicateTo replicateTo, TimeSpan timeout)
        {
            throw new NotImplementedException();
        }

        public IOperationResult Remove<T>(IDocument<T> document, ReplicateTo replicateTo, PersistTo persistTo)
        {
            throw new NotImplementedException();
        }

        public IOperationResult Remove<T>(IDocument<T> document, ReplicateTo replicateTo, PersistTo persistTo, TimeSpan timeout)
        {
            throw new NotImplementedException();
        }

        public Task<IOperationResult> RemoveAsync<T>(IDocument<T> document, ReplicateTo replicateTo, PersistTo persistTo)
        {
            throw new NotImplementedException();
        }

        public Task<IOperationResult> RemoveAsync<T>(IDocument<T> document, ReplicateTo replicateTo, PersistTo persistTo, TimeSpan timeout)
        {
            throw new NotImplementedException();
        }

        public Task<IOperationResult[]> RemoveAsync<T>(List<IDocument<T>> documents)
        {
            throw new NotImplementedException();
        }

        public Task<IOperationResult[]> RemoveAsync<T>(List<IDocument<T>> documents, TimeSpan timeout)
        {
            throw new NotImplementedException();
        }

        public Task<IOperationResult[]> RemoveAsync<T>(List<IDocument<T>> documents, ReplicateTo replicateTo)
        {
            throw new NotImplementedException();
        }

        public Task<IOperationResult[]> RemoveAsync<T>(List<IDocument<T>> documents, ReplicateTo replicateTo, TimeSpan timeout)
        {
            throw new NotImplementedException();
        }

        public Task<IOperationResult[]> RemoveAsync<T>(List<IDocument<T>> documents, ReplicateTo replicateTo, PersistTo persistTo)
        {
            throw new NotImplementedException();
        }

        public Task<IOperationResult[]> RemoveAsync<T>(List<IDocument<T>> documents, ReplicateTo replicateTo, PersistTo persistTo, TimeSpan timeout)
        {
            throw new NotImplementedException();
        }

        public IOperationResult Remove(string key)
        {
            throw new NotImplementedException();
        }

        public IOperationResult Remove(string key, TimeSpan timeout)
        {
            throw new NotImplementedException();
        }

        public Task<IOperationResult> RemoveAsync(string key)
        {
            throw new NotImplementedException();
        }

        public Task<IOperationResult> RemoveAsync(string key, TimeSpan timeout)
        {
            throw new NotImplementedException();
        }

        public IOperationResult Remove(string key, ulong cas)
        {
            throw new NotImplementedException();
        }

        public IOperationResult Remove(string key, ulong cas, TimeSpan timeout)
        {
            throw new NotImplementedException();
        }

        public Task<IOperationResult> RemoveAsync(string key, ulong cas)
        {
            throw new NotImplementedException();
        }

        public Task<IOperationResult> RemoveAsync(string key, ulong cas, TimeSpan timeout)
        {
            throw new NotImplementedException();
        }

        public IOperationResult Remove(string key, ReplicateTo replicateTo)
        {
            throw new NotImplementedException();
        }

        public IOperationResult Remove(string key, ReplicateTo replicateTo, TimeSpan timeout)
        {
            throw new NotImplementedException();
        }

        public Task<IOperationResult> RemoveAsync(string key, ReplicateTo replicateTo)
        {
            throw new NotImplementedException();
        }

        public IOperationResult Remove(string key, ulong cas, ReplicateTo replicateTo)
        {
            throw new NotImplementedException();
        }

        public IOperationResult Remove(string key, ulong cas, ReplicateTo replicateTo, TimeSpan timeout)
        {
            throw new NotImplementedException();
        }

        public Task<IOperationResult> RemoveAsync(string key, ulong cas, ReplicateTo replicateTo)
        {
            throw new NotImplementedException();
        }

        public Task<IOperationResult> RemoveAsync(string key, ulong cas, ReplicateTo replicateTo, TimeSpan timeout)
        {
            throw new NotImplementedException();
        }

        public IOperationResult Remove(string key, ReplicateTo replicateTo, PersistTo persistTo)
        {
            throw new NotImplementedException();
        }

        public IOperationResult Remove(string key, ReplicateTo replicateTo, PersistTo persistTo, TimeSpan timeout)
        {
            throw new NotImplementedException();
        }

        public Task<IOperationResult> RemoveAsync(string key, ReplicateTo replicateTo, PersistTo persistTo)
        {
            throw new NotImplementedException();
        }

        public Task<IOperationResult> RemoveAsync(string key, ReplicateTo replicateTo, PersistTo persistTo, TimeSpan timeout)
        {
            throw new NotImplementedException();
        }

        public IOperationResult Remove(string key, ulong cas, ReplicateTo replicateTo, PersistTo persistTo)
        {
            throw new NotImplementedException();
        }

        public IOperationResult Remove(string key, ulong cas, ReplicateTo replicateTo, PersistTo persistTo, TimeSpan timeout)
        {
            throw new NotImplementedException();
        }

        public Task<IOperationResult> RemoveAsync(string key, ulong cas, ReplicateTo replicateTo, PersistTo persistTo)
        {
            throw new NotImplementedException();
        }

        public Task<IOperationResult> RemoveAsync(string key, ulong cas, ReplicateTo replicateTo, PersistTo persistTo, TimeSpan timeout)
        {
            throw new NotImplementedException();
        }

        public IDictionary<string, IOperationResult> Remove(IList<string> keys)
        {
            throw new NotImplementedException();
        }

        public IDictionary<string, IOperationResult> Remove(IList<string> keys, TimeSpan timeout)
        {
            throw new NotImplementedException();
        }

        public IDictionary<string, IOperationResult> Remove(IList<string> keys, ParallelOptions options)
        {
            throw new NotImplementedException();
        }

        public IDictionary<string, IOperationResult> Remove(IList<string> keys, ParallelOptions options, TimeSpan timeout)
        {
            throw new NotImplementedException();
        }

        public IDictionary<string, IOperationResult> Remove(IList<string> keys, ParallelOptions options, int rangeSize)
        {
            throw new NotImplementedException();
        }

        public IDictionary<string, IOperationResult> Remove(IList<string> keys, ParallelOptions options, int rangeSize, TimeSpan timeout)
        {
            throw new NotImplementedException();
        }

        public IOperationResult Touch(string key, TimeSpan expiration)
        {
            throw new NotImplementedException();
        }

        public Task<IOperationResult> TouchAsync(string key, TimeSpan expiration)
        {
            if (_store.TryGetValue(key, out BucketItem value))
            {
                value.Reset(expiration);
                return Task.FromResult((IOperationResult)new FauxOperationResult<object> { Success = true});
            }

            return Task.FromResult((IOperationResult) new FauxOperationResult<object>{ Success = false});
        }

        public Task<IOperationResult> TouchAsync(string key, TimeSpan expiration, TimeSpan timeout)
        {
            throw new NotImplementedException();
        }

        public IOperationResult<T> GetAndTouch<T>(string key, TimeSpan expiration)
        {
            throw new NotImplementedException();
        }

        public IOperationResult<T> GetAndTouch<T>(string key, TimeSpan expiration, TimeSpan timeout)
        {
            throw new NotImplementedException();
        }

        public Task<IOperationResult<T>> GetAndTouchAsync<T>(string key, TimeSpan expiration)
        {
            if (DisableGet)
            {
                throw new NotSupportedException();
            }
            if (_store.TryGetValue(key, out BucketItem item))
            {
                return Task.FromResult((IOperationResult<T>)new FauxOperationResult<T>
                {
                    Success = true,
                    Value = _serializer.Deserialize<T>(item.Value, 0, item.Value.Length)
                });
            }
            return Task.FromResult((IOperationResult<T>)new FauxOperationResult<T>
            {
                Success = false
            });
        }

        public Task<IOperationResult<T>> GetAndTouchAsync<T>(string key, TimeSpan expiration, TimeSpan timeout)
        {
            throw new NotImplementedException();
        }

        public IDocumentResult<T> GetAndTouchDocument<T>(string key, TimeSpan expiration)
        {
            throw new NotImplementedException();
        }

        public IDocumentResult<T> GetAndTouchDocument<T>(string key, TimeSpan expiration, TimeSpan timeout)
        {
            throw new NotImplementedException();
        }

        public Task<IDocumentResult<T>> GetAndTouchDocumentAsync<T>(string key, TimeSpan expiration)
        {
            throw new NotImplementedException();
        }

        public Task<IDocumentResult<T>> GetAndTouchDocumentAsync<T>(string key, TimeSpan expiration, TimeSpan timeout)
        {
            throw new NotImplementedException();
        }

        public IDocumentResult<T> GetDocument<T>(string id)
        {
            throw new NotImplementedException();
        }

        public IDocumentResult<T> GetDocument<T>(string id, TimeSpan timeout)
        {
            throw new NotImplementedException();
        }

        public Task<IDocumentResult<T>> GetDocumentAsync<T>(string id)
        {
            throw new NotImplementedException();
        }

        public Task<IDocumentResult<T>> GetDocumentAsync<T>(string id, TimeSpan timeout)
        {
            throw new NotImplementedException();
        }

        public Task<IDocumentResult<T>[]> GetDocumentsAsync<T>(IEnumerable<string> ids)
        {
            throw new NotImplementedException();
        }

        public Task<IDocumentResult<T>[]> GetDocumentsAsync<T>(IEnumerable<string> ids, TimeSpan timeout)
        {
            throw new NotImplementedException();
        }

        public IDocumentResult<T> GetDocumentFromReplica<T>(string id)
        {
            throw new NotImplementedException();
        }

        public IDocumentResult<T> GetDocumentFromReplica<T>(string id, TimeSpan timeout)
        {
            throw new NotImplementedException();
        }

        public Task<IDocumentResult<T>> GetDocumentFromReplicaAsync<T>(string id)
        {
            throw new NotImplementedException();
        }

        public Task<IDocumentResult<T>> GetDocumentFromReplicaAsync<T>(string id, TimeSpan timeout)
        {
            throw new NotImplementedException();
        }

        public IOperationResult<T> Get<T>(string key)
        {
            return GetAsync<T>(key).GetAwaiter().GetResult();
        }

        public IOperationResult<T> Get<T>(string key, TimeSpan timeout)
        {
            throw new NotImplementedException();
        }

        public Task<IOperationResult<T>> GetAsync<T>(string key)
        {
            if (DisableGet)
            {
                throw new NotSupportedException();
            }
            if (_store.TryGetValue(key, out BucketItem item))
            {
                return Task.FromResult((IOperationResult<T>) new FauxOperationResult<T>
                {
                    Success = true,
                    Value = _serializer.Deserialize<T>(item.Value, 0, item.Value.Length)
                });
            }
            return Task.FromResult((IOperationResult<T>)new FauxOperationResult<T>
            {
                Success = false
            });
        }

        public Task<IOperationResult<T>> GetAsync<T>(string key, TimeSpan timeout)
        {
            throw new NotImplementedException();
        }

        public IOperationResult<T> GetFromReplica<T>(string key)
        {
            throw new NotImplementedException();
        }

        public IOperationResult<T> GetFromReplica<T>(string key, TimeSpan timeout)
        {
            throw new NotImplementedException();
        }

        public Task<IOperationResult<T>> GetFromReplicaAsync<T>(string key)
        {
            throw new NotImplementedException();
        }

        public Task<IOperationResult<T>> GetFromReplicaAsync<T>(string key, TimeSpan timeout)
        {
            throw new NotImplementedException();
        }

        public IDictionary<string, IOperationResult<T>> Get<T>(IList<string> keys)
        {
            throw new NotImplementedException();
        }

        public IDictionary<string, IOperationResult<T>> Get<T>(IList<string> keys, TimeSpan timeout)
        {
            throw new NotImplementedException();
        }

        public IDictionary<string, IOperationResult<T>> Get<T>(IList<string> keys, ParallelOptions options)
        {
            throw new NotImplementedException();
        }

        public IDictionary<string, IOperationResult<T>> Get<T>(IList<string> keys, ParallelOptions options, TimeSpan timeout)
        {
            throw new NotImplementedException();
        }

        public IDictionary<string, IOperationResult<T>> Get<T>(IList<string> keys, ParallelOptions options, int rangeSize)
        {
            throw new NotImplementedException();
        }

        public IDictionary<string, IOperationResult<T>> Get<T>(IList<string> keys, ParallelOptions options, int rangeSize, TimeSpan timeout)
        {
            throw new NotImplementedException();
        }

        public IOperationResult<T> GetWithLock<T>(string key, uint expiration)
        {
            throw new NotImplementedException();
        }

        public IOperationResult<T> GetAndLock<T>(string key, uint expiration)
        {
            throw new NotImplementedException();
        }

        public IOperationResult<T> GetAndLock<T>(string key, uint expiration, TimeSpan timeout)
        {
            throw new NotImplementedException();
        }

        public Task<IOperationResult<T>> GetWithLockAsync<T>(string key, uint expiration)
        {
            throw new NotImplementedException();
        }

        public Task<IOperationResult<T>> GetAndLockAsync<T>(string key, uint expiration)
        {
            throw new NotImplementedException();
        }

        public Task<IOperationResult<T>> GetAndLockAsync<T>(string key, uint expiration, TimeSpan timeout)
        {
            throw new NotImplementedException();
        }

        public IOperationResult<T> GetWithLock<T>(string key, TimeSpan expiration)
        {
            throw new NotImplementedException();
        }

        public IOperationResult<T> GetAndLock<T>(string key, TimeSpan expiration)
        {
            throw new NotImplementedException();
        }

        public IOperationResult<T> GetAndLock<T>(string key, TimeSpan expiration, TimeSpan timeout)
        {
            throw new NotImplementedException();
        }

        public Task<IOperationResult<T>> GetWithLockAsync<T>(string key, TimeSpan expiration)
        {
            throw new NotImplementedException();
        }

        public Task<IOperationResult<T>> GetAndLockAsync<T>(string key, TimeSpan expiration)
        {
            throw new NotImplementedException();
        }

        public Task<IOperationResult<T>> GetAndLockAsync<T>(string key, TimeSpan expiration, TimeSpan timeout)
        {
            throw new NotImplementedException();
        }

        public IOperationResult Unlock(string key, ulong cas)
        {
            throw new NotImplementedException();
        }

        public IOperationResult Unlock(string key, ulong cas, TimeSpan timeout)
        {
            throw new NotImplementedException();
        }

        public Task<IOperationResult> UnlockAsync(string key, ulong cas)
        {
            throw new NotImplementedException();
        }

        public Task<IOperationResult> UnlockAsync(string key, ulong cas, TimeSpan timeout)
        {
            throw new NotImplementedException();
        }

        public IOperationResult<ulong> Increment(string key)
        {
            throw new NotImplementedException();
        }

        public IOperationResult<ulong> Increment(string key, TimeSpan timeout)
        {
            throw new NotImplementedException();
        }

        public Task<IOperationResult<ulong>> IncrementAsync(string key)
        {
            throw new NotImplementedException();
        }

        public Task<IOperationResult<ulong>> IncrementAsync(string key, TimeSpan timeout)
        {
            throw new NotImplementedException();
        }

        public IOperationResult<ulong> Increment(string key, ulong delta)
        {
            throw new NotImplementedException();
        }

        public IOperationResult<ulong> Increment(string key, ulong delta, TimeSpan timeout)
        {
            throw new NotImplementedException();
        }

        public Task<IOperationResult<ulong>> IncrementAsync(string key, ulong delta)
        {
            throw new NotImplementedException();
        }

        public Task<IOperationResult<ulong>> IncrementAsync(string key, ulong delta, TimeSpan timeout)
        {
            throw new NotImplementedException();
        }

        public IOperationResult<ulong> Increment(string key, ulong delta, ulong initial)
        {
            throw new NotImplementedException();
        }

        public Task<IOperationResult<ulong>> IncrementAsync(string key, ulong delta, ulong initial)
        {
            throw new NotImplementedException();
        }

        public IOperationResult<ulong> Increment(string key, ulong delta, ulong initial, uint expiration)
        {
            throw new NotImplementedException();
        }

        public IOperationResult<ulong> Increment(string key, ulong delta, ulong initial, uint expiration, TimeSpan timeout)
        {
            throw new NotImplementedException();
        }

        public Task<IOperationResult<ulong>> IncrementAsync(string key, ulong delta, ulong initial, uint expiration)
        {
            throw new NotImplementedException();
        }

        public Task<IOperationResult<ulong>> IncrementAsync(string key, ulong delta, ulong initial, uint expiration, TimeSpan timeout)
        {
            throw new NotImplementedException();
        }

        public IOperationResult<ulong> Increment(string key, ulong delta, ulong initial, TimeSpan expiration)
        {
            throw new NotImplementedException();
        }

        public IOperationResult<ulong> Increment(string key, ulong delta, ulong initial, TimeSpan expiration, TimeSpan timeout)
        {
            throw new NotImplementedException();
        }

        public Task<IOperationResult<ulong>> IncrementAsync(string key, ulong delta, ulong initial, TimeSpan expiration)
        {
            throw new NotImplementedException();
        }

        public Task<IOperationResult<ulong>> IncrementAsync(string key, ulong delta, ulong initial, TimeSpan expiration, TimeSpan timeout)
        {
            throw new NotImplementedException();
        }

        public IOperationResult<ulong> Decrement(string key)
        {
            throw new NotImplementedException();
        }

        public IOperationResult<ulong> Decrement(string key, TimeSpan timeout)
        {
            throw new NotImplementedException();
        }

        public Task<IOperationResult<ulong>> DecrementAsync(string key)
        {
            throw new NotImplementedException();
        }

        public Task<IOperationResult<ulong>> DecrementAsync(string key, TimeSpan timeout)
        {
            throw new NotImplementedException();
        }

        public IOperationResult<ulong> Decrement(string key, ulong delta)
        {
            throw new NotImplementedException();
        }

        public IOperationResult<ulong> Decrement(string key, ulong delta, TimeSpan timeout)
        {
            throw new NotImplementedException();
        }

        public Task<IOperationResult<ulong>> DecrementAsync(string key, ulong delta)
        {
            throw new NotImplementedException();
        }

        public Task<IOperationResult<ulong>> DecrementAsync(string key, ulong delta, TimeSpan timeout)
        {
            throw new NotImplementedException();
        }

        public IOperationResult<ulong> Decrement(string key, ulong delta, ulong initial)
        {
            throw new NotImplementedException();
        }

        public Task<IOperationResult<ulong>> DecrementAsync(string key, ulong delta, ulong initial)
        {
            throw new NotImplementedException();
        }

        public IOperationResult<ulong> Decrement(string key, ulong delta, ulong initial, uint expiration)
        {
            throw new NotImplementedException();
        }

        public IOperationResult<ulong> Decrement(string key, ulong delta, ulong initial, uint expiration, TimeSpan timeout)
        {
            throw new NotImplementedException();
        }

        public Task<IOperationResult<ulong>> DecrementAsync(string key, ulong delta, ulong initial, uint expiration)
        {
            throw new NotImplementedException();
        }

        public Task<IOperationResult<ulong>> DecrementAsync(string key, ulong delta, ulong initial, uint expiration, TimeSpan timeout)
        {
            throw new NotImplementedException();
        }

        public IOperationResult<ulong> Decrement(string key, ulong delta, ulong initial, TimeSpan expiration)
        {
            throw new NotImplementedException();
        }

        public IOperationResult<ulong> Decrement(string key, ulong delta, ulong initial, TimeSpan expiration, TimeSpan timeout)
        {
            throw new NotImplementedException();
        }

        public Task<IOperationResult<ulong>> DecrementAsync(string key, ulong delta, ulong initial, TimeSpan expiration)
        {
            throw new NotImplementedException();
        }

        public Task<IOperationResult<ulong>> DecrementAsync(string key, ulong delta, ulong initial, TimeSpan expiration, TimeSpan timeout)
        {
            throw new NotImplementedException();
        }

        public IOperationResult<string> Append(string key, string value)
        {
            throw new NotImplementedException();
        }

        public IOperationResult<string> Append(string key, string value, TimeSpan timeout)
        {
            throw new NotImplementedException();
        }

        public Task<IOperationResult<string>> AppendAsync(string key, string value)
        {
            throw new NotImplementedException();
        }

        public Task<IOperationResult<string>> AppendAsync(string key, string value, TimeSpan timeout)
        {
            throw new NotImplementedException();
        }

        public IOperationResult<byte[]> Append(string key, byte[] value)
        {
            throw new NotImplementedException();
        }

        public IOperationResult<byte[]> Append(string key, byte[] value, TimeSpan timeout)
        {
            throw new NotImplementedException();
        }

        public Task<IOperationResult<byte[]>> AppendAsync(string key, byte[] value)
        {
            throw new NotImplementedException();
        }

        public Task<IOperationResult<byte[]>> AppendAsync(string key, byte[] value, TimeSpan timeout)
        {
            throw new NotImplementedException();
        }

        public IOperationResult<string> Prepend(string key, string value)
        {
            throw new NotImplementedException();
        }

        public IOperationResult<string> Prepend(string key, string value, TimeSpan timeout)
        {
            throw new NotImplementedException();
        }

        public Task<IOperationResult<string>> PrependAsync(string key, string value)
        {
            throw new NotImplementedException();
        }

        public Task<IOperationResult<string>> PrependAsync(string key, string value, TimeSpan timeout)
        {
            throw new NotImplementedException();
        }

        public IOperationResult<byte[]> Prepend(string key, byte[] value)
        {
            throw new NotImplementedException();
        }

        public IOperationResult<byte[]> Prepend(string key, byte[] value, TimeSpan timeout)
        {
            throw new NotImplementedException();
        }

        public Task<IOperationResult<byte[]>> PrependAsync(string key, byte[] value)
        {
            throw new NotImplementedException();
        }

        public Task<IOperationResult<byte[]>> PrependAsync(string key, byte[] value, TimeSpan timeout)
        {
            throw new NotImplementedException();
        }

        public IViewResult<T> Query<T>(IViewQueryable query)
        {
            throw new NotImplementedException();
        }

        public Task<IViewResult<T>> QueryAsync<T>(IViewQueryable query)
        {
            throw new NotImplementedException();
        }

        public IQueryResult<T> Query<T>(string query)
        {
            throw new NotImplementedException();
        }

        public Task<IQueryResult<T>> QueryAsync<T>(string query)
        {
            throw new NotImplementedException();
        }

        public IQueryResult<T> Query<T>(IQueryRequest queryRequest)
        {
            throw new NotImplementedException();
        }

        public Task<IQueryResult<T>> QueryAsync<T>(IQueryRequest queryRequest)
        {
            throw new NotImplementedException();
        }

        public Task<IQueryResult<T>> QueryAsync<T>(IQueryRequest queryRequest, CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }

        public IAnalyticsResult<T> Query<T>(IAnalyticsRequest analyticsRequest)
        {
            throw new NotImplementedException();
        }

        public Task<IAnalyticsResult<T>> QueryAsync<T>(IAnalyticsRequest analyticsRequest)
        {
            throw new NotImplementedException();
        }

        public Task<IAnalyticsResult<T>> QueryAsync<T>(IAnalyticsRequest analyticsRequest, CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }

        public IViewQuery CreateQuery(string designDoc, string view)
        {
            throw new NotImplementedException();
        }

        public IViewQuery CreateQuery(string designdoc, string view, bool development)
        {
            throw new NotImplementedException();
        }

        public IBucketManager CreateManager(string username, string password)
        {
            throw new NotImplementedException();
        }

        public IBucketManager CreateManager()
        {
            throw new NotImplementedException();
        }

        public IMutateInBuilder<TDocument> MutateIn<TDocument>(string key)
        {
            throw new NotImplementedException();
        }

        public IMutateInBuilder<TDocument> MutateIn<TDocument>(string key, TimeSpan timeout)
        {
            throw new NotImplementedException();
        }

        public ILookupInBuilder<TDocument> LookupIn<TDocument>(string key)
        {
            throw new NotImplementedException();
        }

        public ILookupInBuilder<TDocument> LookupIn<TDocument>(string key, TimeSpan timeout)
        {
            throw new NotImplementedException();
        }

        public ISearchQueryResult Query(SearchQuery searchQuery)
        {
            throw new NotImplementedException();
        }

        public Task<ISearchQueryResult> QueryAsync(SearchQuery searchQuery)
        {
            throw new NotImplementedException();
        }

        public IResult<TContent> MapGet<TContent>(string key, string mapkey)
        {
            throw new NotImplementedException();
        }

        public IResult<TContent> MapGet<TContent>(string key, string mapkey, TimeSpan timeout)
        {
            throw new NotImplementedException();
        }

        public IResult MapRemove(string key, string mapkey)
        {
            throw new NotImplementedException();
        }

        public IResult MapRemove(string key, string mapkey, TimeSpan timeout)
        {
            throw new NotImplementedException();
        }

        public IResult<int> MapSize(string key)
        {
            throw new NotImplementedException();
        }

        public IResult<int> MapSize(string key, TimeSpan timeout)
        {
            throw new NotImplementedException();
        }

        public IResult MapAdd(string key, string mapkey, string value, bool createMap)
        {
            throw new NotImplementedException();
        }

        public IResult MapAdd(string key, string mapkey, string value, bool createMap, TimeSpan timeout)
        {
            throw new NotImplementedException();
        }

        public IResult<TContent> ListGet<TContent>(string key, int index)
        {
            throw new NotImplementedException();
        }

        public IResult<TContent> ListGet<TContent>(string key, int index, TimeSpan timeout)
        {
            throw new NotImplementedException();
        }

        public IResult ListAppend(string key, object value, bool createList)
        {
            throw new NotImplementedException();
        }

        public IResult ListAppend(string key, object value, bool createList, TimeSpan timeout)
        {
            throw new NotImplementedException();
        }

        public IResult ListPrepend(string key, object value, bool createList)
        {
            throw new NotImplementedException();
        }

        public IResult ListPrepend(string key, object value, bool createList, TimeSpan timeout)
        {
            throw new NotImplementedException();
        }

        public IResult ListRemove(string key, int index)
        {
            throw new NotImplementedException();
        }

        public IResult ListRemove(string key, int index, TimeSpan timeout)
        {
            throw new NotImplementedException();
        }

        public IResult ListSet(string key, int index, string value)
        {
            throw new NotImplementedException();
        }

        public IResult ListSet(string key, int index, string value, TimeSpan timeout)
        {
            throw new NotImplementedException();
        }

        public IResult<int> ListSize(string key)
        {
            throw new NotImplementedException();
        }

        public IResult<int> ListSize(string key, TimeSpan timeout)
        {
            throw new NotImplementedException();
        }

        public IResult SetAdd(string key, string value, bool createSet)
        {
            throw new NotImplementedException();
        }

        public IResult SetAdd(string key, string value, bool createSet, TimeSpan timeout)
        {
            throw new NotImplementedException();
        }

        public IResult<bool> SetContains(string key, string value)
        {
            throw new NotImplementedException();
        }

        public IResult<bool> SetContains(string key, string value, TimeSpan timeout)
        {
            throw new NotImplementedException();
        }

        public IResult<int> SetSize(string key)
        {
            throw new NotImplementedException();
        }

        public IResult<int> SetSize(string key, TimeSpan timeout)
        {
            throw new NotImplementedException();
        }

        public IResult SetRemove<T>(string key, T value)
        {
            throw new NotImplementedException();
        }

        public IResult SetRemove<T>(string key, T value, TimeSpan timeout)
        {
            throw new NotImplementedException();
        }

        public IResult QueuePush<T>(string key, T value, bool createQueue)
        {
            throw new NotImplementedException();
        }

        public IResult QueuePush<T>(string key, T value, bool createQueue, TimeSpan timeout)
        {
            throw new NotImplementedException();
        }

        public IResult<T> QueuePop<T>(string key)
        {
            throw new NotImplementedException();
        }

        public IResult<T> QueuePop<T>(string key, TimeSpan timeout)
        {
            throw new NotImplementedException();
        }

        public IResult<int> QueueSize(string key)
        {
            throw new NotImplementedException();
        }

        public IResult<int> QueueSize(string key, TimeSpan timeout)
        {
            throw new NotImplementedException();
        }

        public Task<IResult<TContent>> MapGetAsync<TContent>(string key, string mapkey)
        {
            throw new NotImplementedException();
        }

        public Task<IResult<TContent>> MapGetAsync<TContent>(string key, string mapkey, TimeSpan timeout)
        {
            throw new NotImplementedException();
        }

        public Task<IResult> MapRemoveAsync(string key, string mapkey)
        {
            throw new NotImplementedException();
        }

        public Task<IResult> MapRemoveAsync(string key, string mapkey, TimeSpan timeout)
        {
            throw new NotImplementedException();
        }

        public Task<IResult<int>> MapSizeAsync(string key)
        {
            throw new NotImplementedException();
        }

        public Task<IResult<int>> MapSizeAsync(string key, TimeSpan timeout)
        {
            throw new NotImplementedException();
        }

        public Task<IResult> MapAddAsync(string key, string mapkey, string value, bool createMap)
        {
            throw new NotImplementedException();
        }

        public Task<IResult> MapAddAsync(string key, string mapkey, string value, bool createMap, TimeSpan timeout)
        {
            throw new NotImplementedException();
        }

        public Task<IResult<TContent>> ListGetAsync<TContent>(string key, int index)
        {
            throw new NotImplementedException();
        }

        public Task<IResult<TContent>> ListGetAsync<TContent>(string key, int index, TimeSpan timeout)
        {
            throw new NotImplementedException();
        }

        public Task<IResult> ListAppendAsync(string key, object value, bool createList)
        {
            throw new NotImplementedException();
        }

        public Task<IResult> ListAppendAsync(string key, object value, bool createList, TimeSpan timeout)
        {
            throw new NotImplementedException();
        }

        public Task<IResult> ListPrependAsync(string key, object value, bool createList)
        {
            throw new NotImplementedException();
        }

        public Task<IResult> ListPrependAsync(string key, object value, bool createList, TimeSpan timeout)
        {
            throw new NotImplementedException();
        }

        public Task<IResult> ListRemoveAsync(string key, int index)
        {
            throw new NotImplementedException();
        }

        public Task<IResult> ListRemoveAsync(string key, int index, TimeSpan timeout)
        {
            throw new NotImplementedException();
        }

        public Task<IResult> ListSetAsync(string key, int index, string value)
        {
            throw new NotImplementedException();
        }

        public Task<IResult> ListSetAsync(string key, int index, string value, TimeSpan timeout)
        {
            throw new NotImplementedException();
        }

        public Task<IResult<int>> ListSizeAsync(string key)
        {
            throw new NotImplementedException();
        }

        public Task<IResult<int>> ListSizeAsync(string key, TimeSpan timeout)
        {
            throw new NotImplementedException();
        }

        public Task<IResult> SetAddAsync(string key, string value, bool createSet)
        {
            throw new NotImplementedException();
        }

        public Task<IResult> SetAddAsync(string key, string value, bool createSet, TimeSpan timeout)
        {
            throw new NotImplementedException();
        }

        public Task<IResult<bool>> SetContainsAsync(string key, string value)
        {
            throw new NotImplementedException();
        }

        public Task<IResult<bool>> SetContainsAsync(string key, string value, TimeSpan timeout)
        {
            throw new NotImplementedException();
        }

        public Task<IResult<int>> SetSizeAsync(string key)
        {
            throw new NotImplementedException();
        }

        public Task<IResult<int>> SetSizeAsync(string key, TimeSpan timeout)
        {
            throw new NotImplementedException();
        }

        public Task<IResult> SetRemoveAsync<T>(string key, T value)
        {
            throw new NotImplementedException();
        }

        public Task<IResult> SetRemoveAsync<T>(string key, T value, TimeSpan timeout)
        {
            throw new NotImplementedException();
        }

        public Task<IResult> QueuePushAsync<T>(string key, T value, bool createQueue)
        {
            throw new NotImplementedException();
        }

        public Task<IResult> QueuePushAsync<T>(string key, T value, bool createQueue, TimeSpan timeout)
        {
            throw new NotImplementedException();
        }

        public Task<IResult<T>> QueuePopAsync<T>(string key)
        {
            throw new NotImplementedException();
        }

        public Task<IResult<T>> QueuePopAsync<T>(string key, TimeSpan timeout)
        {
            throw new NotImplementedException();
        }

        public Task<IResult<int>> QueueSizeAsync(string key)
        {
            throw new NotImplementedException();
        }

        public ClusterVersion? GetClusterVersion()
        {
            throw new NotImplementedException();
        }

        public Task<ClusterVersion?> GetClusterVersionAsync()
        {
            throw new NotImplementedException();
        }

        public Task<IResult<int>> QueueSizeAsync(string key, TimeSpan timeout)
        {
            throw new NotImplementedException();
        }

        public IOperationResult Touch(string key, TimeSpan expiration, TimeSpan timeout)
        {
            throw new NotImplementedException();
        }

        public string Name { get; }
        public BucketTypeEnum BucketType { get; }
        public ICluster Cluster { get; }
        public bool IsSecure { get; }
        public bool SupportsEnhancedDurability { get; }
        public bool SupportsKvErrorMap { get; }
        public BucketConfiguration Configuration { get; }
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
