using System;
using System.Collections.Generic;
using Couchbase.Extensions.MultiOp.Internal;
using Couchbase.KeyValue;

namespace Couchbase.Extensions.MultiOp
{
    /// <summary>
    /// Extensions to perform multiple operations in parallel on <see cref="CouchbaseCollection"/>.
    /// </summary>
    public static partial class MultiOpCollectionExtensions
    {
        /// <summary>
        /// Unlocks multiple documents.
        /// </summary>
        /// <typeparam name="T">Document model.</typeparam>
        /// <param name="collection">Collection to use.</param>
        /// <param name="items">Document keys and CAS values to unlock.</param>
        /// <param name="multiOpOptions">Options to control parallelization.</param>
        /// <returns>An observable which receives results as they arrive.</returns>
        /// <remarks>
        /// Operations are not executed until the observable is subscribed.
        /// </remarks>
        public static IObservable<MultiOpResult> Unlock<T>(this ICouchbaseCollection collection,
            IEnumerable<KeyValuePair<string, ulong>> items, MultiOpOptions? multiOpOptions = null) =>
            collection.Unlock<T>(items, (UnlockOptions?) null, multiOpOptions);

        /// <summary>
        /// Unlocks multiple documents.
        /// </summary>
        /// <typeparam name="T">Document model.</typeparam>
        /// <param name="collection">Collection to use.</param>
        /// <param name="items">Document keys and CAS values to unlock.</param>
        /// <param name="optionsBuilder">Builder which configures the options to apply to each operation.</param>
        /// <param name="multiOpOptions">Options to control parallelization.</param>
        /// <returns>An observable which receives results as they arrive.</returns>
        /// <remarks>
        /// Operations are not executed until the observable is subscribed.
        /// </remarks>
        public static IObservable<MultiOpResult> Unlock<T>(this ICouchbaseCollection collection,
            IEnumerable<KeyValuePair<string, ulong>> items, Action<UnlockOptions> optionsBuilder, MultiOpOptions? multiOpOptions = null)
        {
            var options = new UnlockOptions();
            optionsBuilder(options);

            return collection.Unlock<T>(items, options, multiOpOptions);
        }

        /// <summary>
        /// Unlocks multiple documents.
        /// </summary>
        /// <typeparam name="T">Document model.</typeparam>
        /// <param name="collection">Collection to use.</param>
        /// <param name="items">Document keys and CAS values to unlock.</param>
        /// <param name="unlockOptions">Options to apply to the operation.</param>
        /// <param name="multiOpOptions">Options to control parallelization.</param>
        /// <returns>An observable which receives results as they arrive.</returns>
        /// <remarks>
        /// Operations are not executed until the observable is subscribed.
        /// </remarks>
        public static IObservable<MultiOpResult> Unlock<T>(this ICouchbaseCollection collection,
            IEnumerable<KeyValuePair<string, ulong>> items, UnlockOptions? unlockOptions, MultiOpOptions? multiOpOptions = null) =>
            Multi.ObserveMulti(items,
                item => collection.UnlockAsync(item.Key, item.Value, unlockOptions),
                multiOpOptions);

        /// <summary>
        /// Unlocks multiple documents.
        /// </summary>
        /// <typeparam name="T">Document model.</typeparam>
        /// <param name="collection">Collection to use.</param>
        /// <param name="items">Document keys and CAS values to unlock.</param>
        /// <param name="optionsFactory">A factory which returns the options to use for each key.</param>
        /// <param name="multiOpOptions">Options to control parallelization.</param>
        /// <returns>An observable which receives results as they arrive.</returns>
        /// <remarks>
        /// Operations are not executed until the observable is subscribed.
        /// </remarks>
        public static IObservable<MultiOpResult> Unlock<T>(this ICouchbaseCollection collection,
            IEnumerable<KeyValuePair<string, ulong>> items, Func<string, UnlockOptions?> optionsFactory,
            MultiOpOptions? multiOpOptions = null) =>
            Multi.ObserveMulti(items,
                item => collection.UnlockAsync(item.Key, item.Value, optionsFactory(item.Key)),
                multiOpOptions);
    }
}
