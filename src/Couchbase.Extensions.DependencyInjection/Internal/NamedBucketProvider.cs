using System;
using Couchbase.Core;

namespace Couchbase.Extensions.DependencyInjection.Internal
{
    /// <summary>
    /// Base implementation for <see cref="INamedBucketProvider"/>, used as the base
    /// class by <see cref="NamedBucketProxyGenerator"/>.
    /// </summary>
    internal abstract class NamedBucketProvider : INamedBucketProvider
    {
        private readonly IBucketProvider _bucketProvider;
        private readonly string _password;

        public string BucketName { get; }

        // ReSharper disable once PublicConstructorInAbstractClass
        public NamedBucketProvider(IBucketProvider bucketProvider, string bucketName, string password)
        {
            _bucketProvider = bucketProvider ?? throw new ArgumentNullException(nameof(bucketProvider));
            BucketName = bucketName ?? throw new ArgumentNullException(nameof(bucketName));
            _password = password;
        }

        public IBucket GetBucket()
        {
            return _bucketProvider.GetBucket(BucketName, _password);
        }
    }
}
