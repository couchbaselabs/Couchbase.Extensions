using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Couchbase.Core.Exceptions;
using Couchbase.Core.Exceptions.KeyValue;
using Couchbase.Core.IO.Serializers;
using Couchbase.Core.IO.Transcoders;
using Couchbase.KeyValue;
using Microsoft.Extensions.Logging;

namespace Couchbase.Extensions.Locks.Internal
{
    /// <inheritdoc/>
    internal sealed class CouchbaseMutex : ICouchbaseMutex
    {
        internal static readonly ITypeTranscoder Transcoder = new JsonTranscoder(
            SystemTextJsonSerializer.Create(LockSerializerContext.Default));

        private readonly ICouchbaseCollection _collection;
        private readonly ILogger<CouchbaseMutex> _logger;

        private ulong _cas;
        private TimeSpan _expirationInterval;
        private CancellationTokenSource? _cancellationTokenSource;

        /// <inheritdoc/>
        public string Name { get; }

        /// <inheritdoc/>
        public string Holder { get; }

        public CouchbaseMutex(ICouchbaseCollection collection, string name, string holder, ILogger<CouchbaseMutex> logger)
        {
            ArgumentNullException.ThrowIfNull(collection);
            ArgumentNullException.ThrowIfNull(name);
            ArgumentNullException.ThrowIfNull(holder);
            ArgumentNullException.ThrowIfNull(logger);

            _collection = collection;
            Name = name;
            Holder = holder;
            _logger = logger;
        }

        /// <inheritdoc/>
        public async Task Renew(TimeSpan expiration, CancellationToken cancellationToken = default)
        {
            if (expiration <= TimeSpan.Zero)
            {
                throw new ArgumentOutOfRangeException(nameof(expiration), "Value must be positive.");
            }

            var key = LockDocument.GetKey(Name);
            var document = new LockDocument
            {
                Holder = Holder,
                RequestedDateTime = DateTime.UtcNow
            };

            bool lockAcquired = false;
            IMutationResult? result = null;
            try
            {
                if (_cas == 0)
                {
                    // We're creating a new lock
                    _logger.LogDebug("Requesting lock '{name}' for holder '{holder}' for {expiration}", Name, Holder,
                        expiration);
                    result = await _collection.InsertAsync(key, document,
                            new InsertOptions()
                                .Expiry(expiration)
                                .Transcoder(Transcoder)
                                .CancellationToken(cancellationToken))
                        .ConfigureAwait(false);
                }
                else
                {
                    _logger.LogDebug("Renewing lock '{name}' for holder '{holder}' for {expiration}", Name, Holder,
                        expiration);
                    result = await _collection.ReplaceAsync(key, document,
                            new ReplaceOptions()
                                .Expiry(expiration)
                                .Transcoder(Transcoder)
                                .CancellationToken(cancellationToken)
                                .Cas(_cas))
                        .ConfigureAwait(false);
                }

                lockAcquired = true;
            }
            catch (CasMismatchException)
            {
                // This is a valid case, trap the exception
                _logger.LogDebug("CAS mismatch updating lock '{name}'", Name);
            }
            catch (DocumentExistsException)
            {
                // This is a valid case, trap the exception
                _logger.LogDebug("Lock document already exists for lock '{name}'", Name);
            }
            catch (DocumentNotFoundException)
            {
                // This is a valid, but rare, case where the document being renewed expired before
                // the renewal. In this case, we'll let the logic move on, which will recreate the document.
                _logger.LogDebug("Lock document missing for lock '{name}'", Name);
            }

            if (lockAcquired)
            {
                _logger.LogDebug("Lock '{name}' issued to holder '{holder}'", Name, Holder);

                _expirationInterval = expiration;
                _cas = result!.Cas;

                return;
            }

            if (!lockAcquired)
            {
                _logger.LogDebug("Lock '{name}' unavailable, getting lock info", Name);

                IGetResult getResult;
                try
                {
                    getResult = await _collection.GetAsync(key,
                        new GetOptions()
                            .Transcoder(Transcoder)
                            .CancellationToken(cancellationToken))
                        .ConfigureAwait(false);
                }
                catch (DocumentNotFoundException)
                {
                    try
                    {
                        // Couldn't find the lock, must have expired between Insert and Get, try one more time
                        result = await _collection.InsertAsync(key, document,
                                new InsertOptions()
                                .Expiry(expiration)
                                .Transcoder(Transcoder)
                                .CancellationToken(cancellationToken))
                            .ConfigureAwait(false);
                    }
                    catch (DocumentExistsException)
                    {
                        throw new CouchbaseLockUnavailableException(Name);
                    }

                    _logger.LogDebug("Lock '{name}' issued to holder '{holder}'", Name, Holder);

                    _expirationInterval = expiration;
                    _cas = result.Cas;
                    return;
                }

                _logger.LogDebug("Unable to acquire lock '{name}' for holder '{holder}'", Name, Holder);

                throw new CouchbaseLockUnavailableException(Name)
                {
                    Holder = getResult.ContentAs<LockDocument>()!.Holder
                };
            }
        }

        /// <inheritdoc/>
        public void AutoRenew(TimeSpan renewalInterval, TimeSpan maximumLifespan)
        {
            if (renewalInterval <= TimeSpan.Zero)
            {
                throw new ArgumentOutOfRangeException(nameof(renewalInterval), "Value must be positive.");
            }
            if (maximumLifespan <= TimeSpan.Zero)
            {
                throw new ArgumentOutOfRangeException(nameof(maximumLifespan), "Value must be positive.");
            }

            // Abort previous AutoRenew
            _cancellationTokenSource?.Cancel();
            _cancellationTokenSource?.Dispose();

            _cancellationTokenSource = new CancellationTokenSource();
            var token = _cancellationTokenSource.Token;

            var startTime = DateTime.Now;
            Task.Run(async () =>
            {
                while (!token.IsCancellationRequested && (maximumLifespan - (DateTime.Now - startTime)) > TimeSpan.Zero)
                {
                    await Task.Delay(renewalInterval, token).ConfigureAwait(false);

                    var lifespanRemaining = maximumLifespan - (DateTime.Now - startTime);
                    if (!token.IsCancellationRequested && lifespanRemaining > TimeSpan.Zero)
                    {
                        try
                        {
                            var lockDuration = TimeSpan.FromTicks(
                                Math.Min(_expirationInterval.Ticks, lifespanRemaining.Ticks));

                            await Renew(lockDuration, token).ConfigureAwait(false);
                        }
                        catch (Exception ex)
                        {
                            _logger.LogError(ex, "Error auto-renewing lock '{name}' for holder '{holder}'", Name, Holder);
                        }
                    }
                }
            }, token);
        }

        /// <inheritdoc/>
        public void Dispose()
        {
            if (_cancellationTokenSource != null)
            {
                _cancellationTokenSource.Cancel(false);
                _cancellationTokenSource.Dispose();
                _cancellationTokenSource = null;
            }

            if (_cas == 0)
            {
                // Never locked
                return;
            }

            var key = LockDocument.GetKey(Name);

            _collection.RemoveAsync(key, new RemoveOptions().Cas(_cas))
                .ContinueWith(t =>
                {
                    if (t.Exception!.InnerExceptions.OfType<DocumentNotFoundException>().Any())
                    {
                        _logger.LogDebug("Did not release lock '{name}' for holder '{holder}' because it was already released.",
                            Name,
                            Holder);
                    }
                    else if (t.Exception!.InnerExceptions.OfType<CasMismatchException>().Any())
                    {
                        _logger.LogDebug(
                            "Did not release lock '{name}' for holder '{holder}' because it was already held by another.",
                            Name,
                            Holder);
                    }
                    else
                    {
                        _logger.LogWarning(t.Exception, "Error releasing lock '{name}' for holder '{holder}'", Name, Holder);
                    }
                }, TaskContinuationOptions.OnlyOnFaulted);
        }
    }
}
