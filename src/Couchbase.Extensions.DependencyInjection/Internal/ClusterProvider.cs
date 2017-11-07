using System;
using Couchbase.Authentication;
using Couchbase.Configuration.Client;
using Couchbase.Core;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace Couchbase.Extensions.DependencyInjection.Internal
{
    internal class ClusterProvider : IClusterProvider
    {
        private readonly IOptions<CouchbaseClientDefinition> _options;
        private readonly ILoggerFactory _loggerFactory;
        private ICluster _cluster;
        private bool _disposed = false;

        public ClusterProvider(IOptions<CouchbaseClientDefinition> options, ILoggerFactory loggerFactory)
        {
            _options = options ?? throw new ArgumentNullException(nameof(options));
            _loggerFactory = loggerFactory ?? throw new ArgumentNullException(nameof(loggerFactory));
        }

        public virtual ICluster GetCluster()
        {
            if (_disposed)
            {
                throw new ObjectDisposedException(nameof(ClusterProvider));
            }

            if (_cluster != null)
            {
                return _cluster;
            }

            _cluster = CreateCluster(_options.Value);

            return _cluster;
        }

        /// <summary>
        /// Seam for injecting mock
        /// </summary>
        protected virtual ICluster CreateCluster(CouchbaseClientDefinition clientDefinition)
        {
            var configuration = new ClientConfiguration(clientDefinition)
            {
                LoggerFactory = _loggerFactory
            };

            ICluster cluster = new Cluster(configuration);
            if (!string.IsNullOrWhiteSpace(clientDefinition.Username))
            {
                cluster.Authenticate(new PasswordAuthenticator(clientDefinition.Username, clientDefinition.Password));
            }

            return cluster;
        }

        public void Dispose()
        {
            if (!_disposed)
            {
                _disposed = true;

                if (_cluster != null)
                {
                    _cluster.Dispose();
                    _cluster = null;
                }
            }
        }
    }
}
