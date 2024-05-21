using System;
using System.Collections.Generic;
using System.Linq;
using System.Reactive.Linq;
using System.Reactive.Threading.Tasks;
using System.Runtime.CompilerServices;
using System.Threading;
using System.Threading.Tasks;

namespace Couchbase.Extensions.MultiOp
{
    /// <summary>
    /// Extensions for <see cref="IObservable{T}"/> related to Couchbase operations.
    /// </summary>
    public static class MultiOpObservableExtensions
    {
        /// <summary>
        /// Ensures that all operations are successful.
        /// </summary>
        /// <param name="observable">Operations to observe.</param>
        /// <param name="cancellationToken">Cancellation token.</param>
        /// <returns>
        /// A task which completes successfully if all operations are successful,
        /// or contains an <see cref="AggregateException"/> if any operations failed.
        /// </returns>
        /// <remarks>
        /// <para>
        /// Awaiting this method will only return a single exception because the await keyword
        /// will unwrap the <see cref="AggregateException"/>. Read the <see cref="Task.Exception"/>
        /// property directly to access all exceptions.
        ///</para>
        /// <para>
        /// Only Couchbase exceptions are handled as an AggregateException. Any low-level .NET
        /// exception will be thrown directly and immediately, without waiting for other operations
        /// to complete.
        /// </para>
        /// </remarks>
        public static Task EnsureSuccessfulAsync(this IObservable<MultiOpResult> observable,
            CancellationToken cancellationToken = default)
        {
            var tcs = new TaskCompletionSource<bool>();

            observable
                .Where(p => !p.IsSuccessful)
                .Select(p => p.Exception!)
                .ToList()
                .ToTask(cancellationToken)
                .ContinueWith(static (task, state) =>
                {
                    var localTcs = (TaskCompletionSource<bool>) state!;

                    if (task.IsFaulted)
                    {
                        localTcs.SetException(task.Exception!.InnerExceptions);
                    }
                    else if (task.IsCanceled)
                    {
                        localTcs.SetCanceled();
                    }
                    else
                    {
                        var result = task.Result;

                        if (result.Count > 0)
                        {
                            localTcs.TrySetException(result);
                        }
                        else
                        {
                            localTcs.SetResult(true);
                        }
                    }
                }, tcs);

            return tcs.Task;
        }
    }
}
