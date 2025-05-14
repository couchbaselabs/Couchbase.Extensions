using System;
using System.Text;
using System.Text.Json;
using Couchbase.Core.IO.Serializers;
using Couchbase.Extensions.Locks.Internal;
using Xunit;

namespace Couchbase.Extensions.Locks.UnitTests.Internal
{
    public class TranscoderTests
    {
        [Fact]
        public void LegacyNewtonsoft_DeserializeSuccess()
        {
            // Arrange

            var legacyLockDocument = new LegacyLockDocument()
            {
                Name = "name",
                Holder = "holder",
                RequestedDateTime = DateTime.UtcNow
            };

            var json = Encoding.UTF8.GetBytes(
                Newtonsoft.Json.JsonConvert.SerializeObject(legacyLockDocument));

            // Act

            var deserialized = CouchbaseMutex.Transcoder.Serializer.Deserialize<LockDocument>(json);

            // Assert

            Assert.Equal(legacyLockDocument.Name, deserialized.Name);
            Assert.Equal(legacyLockDocument.Holder, deserialized.Holder);
            Assert.InRange(legacyLockDocument.RequestedDateTime.Ticks - deserialized.RequestedDateTime.Ticks,
                -1000, 1000);
        }

        [Fact]
        public void LegacySystemTextJson_DeserializeSuccess()
        {
            // Arrange

            var legacyLockDocument = new LegacyLockDocument()
            {
                Name = "name",
                Holder = "holder",
                RequestedDateTime = DateTime.UtcNow
            };

            var json = Encoding.UTF8.GetBytes(
                JsonSerializer.Serialize(legacyLockDocument));

            // Act

            var deserialized = CouchbaseMutex.Transcoder.Serializer.Deserialize<LockDocument>(json);

            // Assert

            Assert.Equal(legacyLockDocument.Name, deserialized.Name);
            Assert.Equal(legacyLockDocument.Holder, deserialized.Holder);
            Assert.InRange(legacyLockDocument.RequestedDateTime.Ticks - deserialized.RequestedDateTime.Ticks,
                -1000, 1000);
        }

        private class LegacyLockDocument
        {
            [Newtonsoft.Json.JsonProperty(PropertyName = "name")]
            public string Name { get; set; }

            [Newtonsoft.Json.JsonProperty(PropertyName = "holder")]
            public string Holder { get; set; }

            [Newtonsoft.Json.JsonConverter(typeof(UnixMillisecondsConverter))]
            [Newtonsoft.Json.JsonProperty(PropertyName = "requestedDateTime")]
            public DateTime RequestedDateTime { get; set; }
        }
    }
}
