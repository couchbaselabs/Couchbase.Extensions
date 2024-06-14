using System.Diagnostics;
using Couchbase.Extensions.DependencyInjection;
using Microsoft.AspNetCore.Mvc;
using Couchbase.Extensions.Locks.Example.Models;
using Polly;

namespace Couchbase.Extensions.Locks.Example.Controllers
{
    public class HomeController : Controller
    {
        // Retry up to 10 times with a 1 second wait
        private static readonly ResiliencePipeline RetryPolicy =
            new ResiliencePipelineBuilder()
                .AddRetry(new Polly.Retry.RetryStrategyOptions
                {
                    ShouldHandle = new PredicateBuilder().Handle<CouchbaseLockUnavailableException>(),
                    MaxRetryAttempts = 10,
                    Delay = TimeSpan.FromSeconds(1),
                    BackoffType = DelayBackoffType.Constant
                })
                .Build();

        private readonly IBucketProvider _bucketProvider;
        private readonly ILogger<HomeController> _logger;

        public HomeController(IBucketProvider bucketProvider, ILogger<HomeController> logger)
        {
            _bucketProvider = bucketProvider ?? throw new ArgumentNullException(nameof(bucketProvider));
            _logger = logger;
        }

        public IActionResult Index()
        {
            return View();
        }

        public async Task<IActionResult> RequestWithLock(int requester)
        {
            var bucket = await _bucketProvider.GetBucketAsync("default");
            var collection = bucket.DefaultCollection();

            _logger.LogInformation("Starting requester {requester}", requester);

            try
            {
                var startTime = DateTime.Now;

                // Retry policy will try 10 times to get the lock, and will wait 1 second between attempts
                // Lock will be held for 2 seconds if not renewed
                // "collection" is passed to the lambda as a state parameter to avoid the heap allocation of creating a closure
                using (var mutex = await RetryPolicy.ExecuteAsync(
                    static (state, cancellationToken) =>
                        new ValueTask<ICouchbaseMutex>(state.RequestMutexAsync("my_lock_name", TimeSpan.FromSeconds(2), cancellationToken)),
                    collection))
                {
                    // Will renew the lock every second, up to a maximum of 15 seconds, so long as the process keeps running
                    mutex.AutoRenew(TimeSpan.FromSeconds(1), TimeSpan.FromSeconds(15));

                    var lockTime = DateTime.Now;

                    await Task.Delay(5000);

                    return View(new RequestWithLockModel {
                        WasLocked = true,
                        LockDelayTime = lockTime - startTime,
                        LockHoldTime = DateTime.Now - lockTime
                    });

                    // Lock will be released once we exit the using statement, and auto renew will cease
                }
            }
            catch (CouchbaseLockUnavailableException)
            {
                return View(new RequestWithLockModel
                {
                    WasLocked = false
                });
            }
        }

        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
