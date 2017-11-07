using System;
using System.Reflection;
using Couchbase.Authentication;
using Couchbase.Configuration.Client;
using Couchbase.Core;
using Couchbase.Extensions.DependencyInjection.Internal;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Moq;
using Moq.Protected;
using Xunit;

namespace Couchbase.Extensions.DependencyInjection.UnitTests.Internal
{
    public class ClusterProviderTests
    {

        #region ctor

        [Fact]
        public void ctor_NoOptions_Exception()
        {
            // Act/Assert

            var ex = Assert.Throws<ArgumentNullException>(() => new ClusterProvider(null, new LoggerFactory()));

            Assert.Equal("options", ex.ParamName);
        }

        [Fact]
        public void ctor_NoLoggerFactory_Exception()
        {
            // Arrange

            var options = new Mock<IOptions<CouchbaseClientDefinition>>();

            // Act/Assert

            var ex = Assert.Throws<ArgumentNullException>(() => new ClusterProvider(options.Object, null));

            Assert.Equal("loggerFactory", ex.ParamName);
        }

        #endregion

        #region GetCluster

        [Fact]
        public void GetCluster_If_Cluster_Level_Auth_Use_PasswordAuthenticator()
        {
            // Arrange

            var options = new Mock<IOptions<CouchbaseClientDefinition>>();
            options.Setup(x => x.Value).Returns(new CouchbaseClientDefinition
            {
                Username = "MyUser",
                Password = "MyPassword"
            });

            var loggerFactory = new LoggerFactory();

            var provider = new ClusterProvider(options.Object, loggerFactory);

            // Act/Assert

            var cluster = provider.GetCluster();
            var authenticator = cluster.Configuration.GetType()
                .GetProperty("Authenticator", BindingFlags.NonPublic | BindingFlags.Instance)
                .GetValue(cluster.Configuration);

            Assert.IsType<PasswordAuthenticator>(authenticator);

        }

        [Fact]
        public void GetCluster_If_No_Username_Or_Password_Authenticator_Is_Null()
        {
            // Arrange

            var options = new Mock<IOptions<CouchbaseClientDefinition>>();
            options.Setup(x => x.Value).Returns(new CouchbaseClientDefinition());

            var loggerFactory = new LoggerFactory();

            var provider = new ClusterProvider(options.Object, loggerFactory);

            // Act/Assert

            var cluster = provider.GetCluster();
            var authenticator = cluster.Configuration.GetType()
                .GetProperty("Authenticator", BindingFlags.NonPublic | BindingFlags.Instance)
                .GetValue(cluster.Configuration);

            Assert.Null(authenticator);
        }

        [Fact]
        public void GetCluster_Disposed_Exception()
        {
            // Arrange

            var options = new Mock<IOptions<CouchbaseClientDefinition>>();
            var loggerFactory = new LoggerFactory();

            var provider = new ClusterProvider(options.Object, loggerFactory);
            provider.Dispose();

            // Act/Assert

            var ex = Assert.Throws<ObjectDisposedException>(() => provider.GetCluster());

            Assert.Equal(nameof(ClusterProvider), ex.ObjectName);
        }

        [Fact]
        public void GetCluster_FirstCall_ReturnsNewCluster()
        {
            // Arrange

            var clientDefinition = new CouchbaseClientDefinition();

            var options = new Mock<IOptions<CouchbaseClientDefinition>>();
            options.SetupGet(m => m.Value).Returns(clientDefinition);

            var loggerFactory = new LoggerFactory();

            var cluster = new Mock<ICluster>();

            var provider = new Mock<ClusterProvider>(options.Object, loggerFactory)
            {
                CallBase = true
            };
            provider.Protected()
                .Setup<ICluster>("CreateCluster", clientDefinition)
                .Returns(cluster.Object);

            // Act

            var result = provider.Object.GetCluster();

            // Assert

            Assert.Equal(cluster.Object, result);
        }

        [Fact]
        public void GetCluster_TwoCalls_OnlyCreatesOneCluster()
        {
            // Arrange

            var clientDefinition = new CouchbaseClientDefinition();

            var options = new Mock<IOptions<CouchbaseClientDefinition>>();
            options.SetupGet(m => m.Value).Returns(clientDefinition);

            var loggerFactory = new LoggerFactory();

            var cluster = new Mock<ICluster>();

            var provider = new Mock<ClusterProvider>(options.Object, loggerFactory)
            {
                CallBase = true
            };
            provider.Protected()
                .Setup<ICluster>("CreateCluster", clientDefinition)
                .Returns(cluster.Object);

            // Act

            provider.Object.GetCluster();
            var result = provider.Object.GetCluster();

            // Assert

            Assert.Equal(cluster.Object, result);

            provider.Protected()
                .Verify<ICluster>("CreateCluster", Times.Once(), clientDefinition);
        }

        #endregion

        #region Dispose

        [Fact]
        public void Dispose_DisposesCluster()
        {
            // Arrange

            var clientDefinition = new CouchbaseClientDefinition();

            var options = new Mock<IOptions<CouchbaseClientDefinition>>();
            options.SetupGet(m => m.Value).Returns(clientDefinition);

            var loggerFactory = new LoggerFactory();

            var cluster = new Mock<ICluster>();
            cluster.Setup(m => m.Dispose());

            var provider = new Mock<ClusterProvider>(options.Object, loggerFactory)
            {
                CallBase = true
            };
            provider.Protected()
                .Setup<ICluster>("CreateCluster", clientDefinition)
                .Returns(cluster.Object);

            provider.Object.GetCluster();

            // Act

            provider.Object.Dispose();

            // Assert

            cluster.Verify(m => m.Dispose(), Times.AtLeastOnce);
        }

        #endregion
    }
}
