using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Couchbase.Configuration.Client;
using Couchbase.Extensions.DnsDiscovery.Internal;
using DnsClient;
using DnsClient.Protocol;
using Microsoft.Extensions.Logging;
using Moq;
using Xunit;

namespace Couchbase.Extensions.DnsDiscovery.UnitTests.Internal
{
    public class CouchbaseDnsLookupTests
    {
        private const string RecordName = "services.local";
        private const string FullRecordName = "_couchbase._tcp.services.local";

        private static readonly Uri ServerRecord1ExpectedUrl =
            new Uri("http://couchbaseserver1.services.local:8091/pools");

        private static readonly Uri ServerRecord2ExpectedUrl =
            new Uri("http://couchbaseserver2.services.local:8091/pools");

        private static readonly SrvRecord SrvRecord1Priority10 = new SrvRecord(
            new ResourceRecordInfo(RecordName, ResourceRecordType.SRV, QueryClass.IN, 60, 0),
            10, 10, 8091, new DnsName("couchbaseserver1.services.local."));

        private static readonly SrvRecord SrvRecord2Priority10 = new SrvRecord(
            new ResourceRecordInfo(RecordName, ResourceRecordType.SRV, QueryClass.IN, 60, 0),
            10, 10, 8091, new DnsName("couchbaseserver2.services.local."));

        private static readonly SrvRecord SrvRecord3Priority20 = new SrvRecord(
            new ResourceRecordInfo(RecordName, ResourceRecordType.SRV, QueryClass.IN, 60, 0),
            20, 10, 8091, new DnsName("couchbaseserver3.services.local."));

        private static readonly SrvRecord SrvRecord4Priority20 = new SrvRecord(
            new ResourceRecordInfo(RecordName, ResourceRecordType.SRV, QueryClass.IN, 60, 0),
            20, 10, 8091, new DnsName("couchbaseserver4.services.local."));

        #region ctor

        [Fact]
        public void ctor_NoLookupClient_Exception()
        {
            // Arrange

            var logger = new Mock<ILogger<CouchbaseDnsLookup>>();

            // Act/Assert

            var ex = Assert.Throws<ArgumentNullException>(() => new CouchbaseDnsLookup(null, logger.Object));

            Assert.Equal("lookupClient", ex.ParamName);
        }


        [Fact]
        public void ctor_NoLogger_Exception()
        {
            // Arrange

            var lookupClient = new  Mock<ILookupClientAdapter>();

            // Act/Assert

            var ex = Assert.Throws<ArgumentNullException>(() => new CouchbaseDnsLookup(lookupClient.Object, null));

            Assert.Equal("logger", ex.ParamName);
        }

        #endregion

        #region Apply

        [Fact]
        public void Apply_NoClientDefinition_Exception()
        {
            // Arrange

            var lookupClient = new Mock<ILookupClientAdapter>();
            var logger = new Mock<ILogger<CouchbaseDnsLookup>>();

            var lookup = new CouchbaseDnsLookup(lookupClient.Object, logger.Object);

            // Act/Assert

            var ex = Assert.Throws<ArgumentNullException>(() => lookup.Apply(null, RecordName));

            Assert.Equal("clientDefinition", ex.ParamName);
        }

        [Fact]
        public void Apply_NoRecordName_Exception()
        {
            // Arrange

            var lookupClient = new Mock<ILookupClientAdapter>();
            var logger = new Mock<ILogger<CouchbaseDnsLookup>>();
            var clientDefinition = new CouchbaseClientDefinition();

            var lookup = new CouchbaseDnsLookup(lookupClient.Object, logger.Object);

            // Act/Assert

            var ex = Assert.Throws<ArgumentNullException>(() => lookup.Apply(clientDefinition, null));

            Assert.Equal("recordName", ex.ParamName);
        }

        [Fact]
        public void Apply_GotResults_ReturnsServersInOrder()
        {
            // Arrange

            var lookupClient = new Mock<ILookupClientAdapter>();
            lookupClient
                .Setup(m => m.QuerySrvAsync(RecordName))
                .Returns(Task.FromResult(new[]
                {
                    SrvRecord3Priority20,
                    SrvRecord4Priority20,
                    SrvRecord1Priority10,
                    SrvRecord2Priority10
                }.AsEnumerable()));

            var logger = new Mock<ILogger<CouchbaseDnsLookup>>();

            var clientDefinition = new CouchbaseClientDefinition();

            var lookup = new CouchbaseDnsLookup(lookupClient.Object, logger.Object);

            // Act

            lookup.Apply(clientDefinition, RecordName);

            // Assert

            Assert.Equal(2, clientDefinition.Servers.Count);
            Assert.Equal(ServerRecord1ExpectedUrl, clientDefinition.Servers[0]);
            Assert.Equal(ServerRecord2ExpectedUrl, clientDefinition.Servers[1]);
        }

        [Fact]
        public void Apply_GotResults_ReturnsServersForHighestPriorityOnly()
        {
            // Arrange

            var lookupClient = new Mock<ILookupClientAdapter>();
            lookupClient
                .Setup(m => m.QuerySrvAsync(RecordName))
                .Returns(Task.FromResult(new[]
                {
                    SrvRecord1Priority10,
                    SrvRecord2Priority10
                }.AsEnumerable()));

            var logger = new Mock<ILogger<CouchbaseDnsLookup>>();

            var clientDefinition = new CouchbaseClientDefinition();

            var lookup = new CouchbaseDnsLookup(lookupClient.Object, logger.Object);

            // Act

            lookup.Apply(clientDefinition, RecordName);

            // Assert

            Assert.Equal(2, clientDefinition.Servers.Count);
            Assert.Equal(ServerRecord1ExpectedUrl, clientDefinition.Servers[0]);
            Assert.Equal(ServerRecord2ExpectedUrl, clientDefinition.Servers[1]);
        }

        [Fact]
        public void Apply_HasServerList_ClearsListFirst()
        {
            // Arrange

            var lookupClient = new Mock<ILookupClientAdapter>();
            lookupClient
                .Setup(m => m.QuerySrvAsync(RecordName))
                .Returns(Task.FromResult(new []
                {
                    SrvRecord1Priority10,
                    SrvRecord2Priority10
                }.AsEnumerable()));

            var logger = new Mock<ILogger<CouchbaseDnsLookup>>();

            var clientDefinition = new CouchbaseClientDefinition
            {
                Servers = new List<Uri>
                {
                    new Uri("http://shouldremove/")
                }
            };

            var lookup = new CouchbaseDnsLookup(lookupClient.Object, logger.Object);

            // Act

            lookup.Apply(clientDefinition, RecordName);

            // Assert

            Assert.Equal(2, clientDefinition.Servers.Count);
        }

        [Fact]
        public void Apply_NoResults_ReturnsEmptyList()
        {
            // Arrange

            var lookupClient = new Mock<ILookupClientAdapter>();
            lookupClient
                .Setup(m => m.QuerySrvAsync(RecordName))
                .Returns(Task.FromResult(new SrvRecord[] {}.AsEnumerable()));

            var logger = new Mock<ILogger<CouchbaseDnsLookup>>();

            var clientDefinition = new CouchbaseClientDefinition();

            var lookup = new CouchbaseDnsLookup(lookupClient.Object, logger.Object);

            // Act

            lookup.Apply(clientDefinition, RecordName);

            // Assert

            Assert.Equal(0, clientDefinition.Servers.Count);
        }

        [Fact]
        public void Apply_ShortRecordName_ReturnsServersFromFullRecordNameIfPresent()
        {
            // Arrange

            var lookupClient = new Mock<ILookupClientAdapter>();
            lookupClient
                .Setup(m => m.QuerySrvAsync(RecordName))
                .Returns(Task.FromResult(new[]
                {
                    SrvRecord2Priority10
                }.AsEnumerable()));
            lookupClient
                .Setup(m => m.QuerySrvAsync(FullRecordName))
                .Returns(Task.FromResult(new[]
                {
                    SrvRecord1Priority10
                }.AsEnumerable()));

            var logger = new Mock<ILogger<CouchbaseDnsLookup>>();

            var clientDefinition = new CouchbaseClientDefinition();

            var lookup = new CouchbaseDnsLookup(lookupClient.Object, logger.Object);

            // Act

            lookup.Apply(clientDefinition, RecordName);

            // Assert

            Assert.Equal(1, clientDefinition.Servers.Count);
            Assert.Equal(ServerRecord1ExpectedUrl, clientDefinition.Servers[0]);
        }

        [Fact]
        public void Apply_UnhandledException_ReturnsEmptyList()
        {
            // Arrange

            var lookupClient = new Mock<ILookupClientAdapter>();
            lookupClient
                .Setup(m => m.QuerySrvAsync(RecordName))
                .ThrowsAsync(new Exception("Badness Happened"));

            var logger = new Mock<ILogger<CouchbaseDnsLookup>>();

            var clientDefinition = new CouchbaseClientDefinition();

            var lookup = new CouchbaseDnsLookup(lookupClient.Object, logger.Object);

            // Act

            lookup.Apply(clientDefinition, RecordName);

            // Assert

            Assert.Equal(0, clientDefinition.Servers.Count);
        }

        [Fact]
        public void Apply_UnhandledException_LogsError()
        {
            // Arrange

            var lookupClient = new Mock<ILookupClientAdapter>();
            lookupClient
                .Setup(m => m.QuerySrvAsync(RecordName))
                .ThrowsAsync(new Exception("Badness Happened"));

            var logger = new Mock<ILogger<CouchbaseDnsLookup>>();
            logger.Setup(
                m =>
                    m.Log(LogLevel.Error, It.IsAny<EventId>(), It.IsAny<object>(), It.IsAny<Exception>(),
                        It.IsAny<Func<object, Exception, string>>()));

            var clientDefinition = new CouchbaseClientDefinition();

            var lookup = new CouchbaseDnsLookup(lookupClient.Object, logger.Object);

            // Act

            lookup.Apply(clientDefinition, RecordName);

            // Assert

            logger.Verify(
                m =>
                    m.Log(LogLevel.Error, It.IsAny<EventId>(), It.IsAny<object>(), It.IsAny<Exception>(),
                        It.IsAny<Func<object, Exception, string>>()),
                Times.AtLeastOnce);
        }

        #endregion
    }
}
