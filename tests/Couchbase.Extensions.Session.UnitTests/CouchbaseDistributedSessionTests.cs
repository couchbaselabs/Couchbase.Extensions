using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Couchbase.Core;
using Couchbase.Extensions.Caching;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Logging;
using Moq;
using Newtonsoft.Json;
using Xunit;

namespace Couchbase.Extensions.Session.UnitTests
{
    public class CouchbaseDistributedSessionTests
    {
        [Fact]
        public async Task LoadAsync_When_Cache_Has_BackingStore_Session_IsAvailable_True()
        {
            var op = new Mock<IOperationResult<Dictionary<string, object>>>();
            op.Setup(x => x.Value).Returns(new Dictionary<string, object>());

            var bucket = new Mock<IBucket>();
            bucket.Setup(x => x.GetAsync<Dictionary<string, object>>(It.IsAny<string>()))
                .Returns(Task.FromResult(op.Object));

            var provider = new Mock<ICouchbaseCacheBucketProvider>();
            provider.Setup(x => x.GetBucket()).Returns(bucket.Object);

            var cache = new CouchbaseCache(provider.Object, new CouchbaseCacheOptions());
            var session = new CouchbaseSession(cache, "thekey", new TimeSpan(0,0, 10, 0), ()=>true, new LoggerFactory(), true);

            await session.LoadAsync();

            Assert.True(session.IsAvailable);
        }

        [Fact]
        public async Task LoadAsync_When_Cache_DoesNotHave_BackingStore_Session_IsAvailable_True()
        {
            var op = new Mock<IOperationResult<Dictionary<string, object>>>(); //value is null

            var bucket = new Mock<IBucket>();
            bucket.Setup(x => x.GetAsync<Dictionary<string, object>>(It.IsAny<string>()))
                .Returns(Task.FromResult(op.Object));

            var provider = new Mock<ICouchbaseCacheBucketProvider>();
            provider.Setup(x => x.GetBucket()).Returns(bucket.Object);

            var cache = new CouchbaseCache(provider.Object, new CouchbaseCacheOptions());
            var session = new CouchbaseSession(cache, "thekey", new TimeSpan(0, 0, 10, 0), () => true, new LoggerFactory(), true);

            await session.LoadAsync();

            Assert.True(session.IsAvailable);
        }

        [Fact]
        public async Task LoadAsync_When_Cache_ThrowsException_BackingStore_Session_IsAvailable_False()
        {
            var bucket = new Mock<IBucket>();
            bucket.Setup(x => x.GetAsync<Dictionary<string, object>>(It.IsAny<string>()))
                .Throws(new BucketNotFoundException());

            var provider = new Mock<ICouchbaseCacheBucketProvider>();
            provider.Setup(x => x.GetBucket()).Returns(bucket.Object);

            var cache = new CouchbaseCache(provider.Object, new CouchbaseCacheOptions());
            var session = new CouchbaseSession(cache, "thekey", new TimeSpan(0, 0, 10, 0), () => true, new LoggerFactory(), true);

            await session.LoadAsync();

            Assert.False(session.IsAvailable);
        }

        [Fact]
        public void TryGetValue_Success()
        {
            var op = new Mock<IOperationResult<Dictionary<string, object>>>(); //value is null

            var bucket = new Mock<IBucket>();
            bucket.Setup(x => x.GetAsync<Dictionary<string, object>>(It.IsAny<string>()))
                .Returns(Task.FromResult(op.Object));

            var provider = new Mock<ICouchbaseCacheBucketProvider>();
            provider.Setup(x => x.GetBucket()).Returns(bucket.Object);

            var cache = new CouchbaseCache(provider.Object, new CouchbaseCacheOptions());
            var session = new CouchbaseSession(cache, "thesessionkey", new TimeSpan(0, 0, 10, 0), () => true, new LoggerFactory(), true);

            session.Set("thekey", "thevalue");

            string value;
            var result = session.TryGetValue("thekey", out value);
            Assert.True(result);
            Assert.NotNull(value);
        }

        [Fact]
        public void TryGetValueOfT_String_Success()
        {
            var op = new Mock<IOperationResult<Dictionary<string, object>>>(); //value is null

            var bucket = new Mock<IBucket>();
            bucket.Setup(x => x.GetAsync<Dictionary<string, object>>(It.IsAny<string>()))
                .Returns(Task.FromResult(op.Object));

            var provider = new Mock<ICouchbaseCacheBucketProvider>();
            provider.Setup(x => x.GetBucket()).Returns(bucket.Object);

            var cache = new CouchbaseCache(provider.Object, new CouchbaseCacheOptions());
            var session = new CouchbaseSession(cache, "thesessionkey", new TimeSpan(0, 0, 10, 0), () => true, new LoggerFactory(), true);

            session.Set("thekey", "thevalue");

            string value;
            var result = session.TryGetValue("thekey", out value);
            Assert.True(result);
            Assert.NotNull(value);
        }

        [Fact]
        public void TryGetValueOfT_Poco_Success()
        {
            var op = new Mock<IOperationResult<Dictionary<string, object>>>(); //value is null

            var bucket = new Mock<IBucket>();
            bucket.Setup(x => x.GetAsync<Dictionary<string, object>>(It.IsAny<string>()))
                .Returns(Task.FromResult(op.Object));

            var provider = new Mock<ICouchbaseCacheBucketProvider>();
            provider.Setup(x => x.GetBucket()).Returns(bucket.Object);

            var cache = new CouchbaseCache(provider.Object, new CouchbaseCacheOptions());
            var session = new CouchbaseSession(cache, "thesessionkey", new TimeSpan(0, 0, 10, 0), () => true, new LoggerFactory(), true);

            var poco = new Poco { Name = "jeff" };
            session.Set("thekey", poco);

            Poco value;
            var result = session.TryGetValue("thekey", out value);
            Assert.True(result);
            Assert.Same(poco, value);
        }

        [Fact]
        public void GetValue_WhenString_Success()
        {
            var op = new Mock<IOperationResult<Dictionary<string, object>>>(); //value is null

            var bucket = new Mock<IBucket>();
            bucket.Setup(x => x.GetAsync<Dictionary<string, object>>(It.IsAny<string>()))
                .Returns(Task.FromResult(op.Object));

            var provider = new Mock<ICouchbaseCacheBucketProvider>();
            provider.Setup(x => x.GetBucket()).Returns(bucket.Object);

            var cache = new CouchbaseCache(provider.Object, new CouchbaseCacheOptions());
            var session = new CouchbaseSession(cache, "thesessionkey", new TimeSpan(0, 0, 10, 0), () => true, new LoggerFactory(), true);

            session.Set("thekey", "thevalue");

            var result = session.Get("thekey");
            Assert.Equal(ConvertToBytes("thevalue"), result);
        }

        [Fact]
        public void GetValue_WhenPoco_Success()
        {
            var op = new Mock<IOperationResult<Dictionary<string, object>>>(); //value is null

            var bucket = new Mock<IBucket>();
            bucket.Setup(x => x.GetAsync<Dictionary<string, object>>(It.IsAny<string>()))
                .Returns(Task.FromResult(op.Object));

            var provider = new Mock<ICouchbaseCacheBucketProvider>();
            provider.Setup(x => x.GetBucket()).Returns(bucket.Object);

            var cache = new CouchbaseCache(provider.Object, new CouchbaseCacheOptions());
            var session = new CouchbaseSession(cache, "thesessionkey", new TimeSpan(0, 0, 10, 0), () => true, new LoggerFactory(), true);

            session.Set("thekeyforpoco", new Poco { Name = "jeff" });

            var result = session.Get("thekeyforpoco");
            Assert.Equal(ConvertToBytes(new Poco {Name = "jeff"}), result);
        }

        [Fact]
        public void Remove_Success()
        {
            var op = new Mock<IOperationResult<Dictionary<string, object>>>(); //value is null

            var bucket = new Mock<IBucket>();
            bucket.Setup(x => x.GetAsync<Dictionary<string, object>>(It.IsAny<string>()))
                .Returns(Task.FromResult(op.Object));

            var provider = new Mock<ICouchbaseCacheBucketProvider>();
            provider.Setup(x => x.GetBucket()).Returns(bucket.Object);

            var cache = new CouchbaseCache(provider.Object, new CouchbaseCacheOptions());
            var session = new CouchbaseSession(cache, "thesessionkey", new TimeSpan(0, 0, 10, 0), () => true, new LoggerFactory(), true);

            var poco = new Poco {Name = "jeff"};
            session.Set("remove_success", poco);
            Assert.Contains("remove_success", session.Keys.Cast<string>());

            session.Remove("remove_success");
            Assert.DoesNotContain("remove_success", session.Keys.Cast<string>());
        }

        [Fact]
        public void SetOfT_Test()
        {
            var op = new Mock<IOperationResult<Dictionary<string, object>>>(); //value is null

            var bucket = new Mock<IBucket>();
            bucket.Setup(x => x.GetAsync<Dictionary<string, object>>(It.IsAny<string>()))
                .Returns(Task.FromResult(op.Object));

            var provider = new Mock<ICouchbaseCacheBucketProvider>();
            provider.Setup(x => x.GetBucket()).Returns(bucket.Object);

            var cache = new CouchbaseCache(provider.Object, new CouchbaseCacheOptions());
            var session = new CouchbaseSession(cache, "thesessionkey", new TimeSpan(0, 0, 10, 0), () => true, new LoggerFactory(), true);

            var poco = new Poco { Name = "jeff" };
            session.Set("SetOfT_Tests", poco);

            Poco actual;
            var result = session.TryGetValue("SetOfT_Tests", out actual);

            Assert.True(result);
            Assert.Same(poco, actual);
        }

        [Fact]
        public void Set_Test()
        {
            var op = new Mock<IOperationResult<Dictionary<string, object>>>(); //value is null

            var bucket = new Mock<IBucket>();
            bucket.Setup(x => x.GetAsync<Dictionary<string, object>>(It.IsAny<string>()))
                .Returns(Task.FromResult(op.Object));

            var provider = new Mock<ICouchbaseCacheBucketProvider>();
            provider.Setup(x => x.GetBucket()).Returns(bucket.Object);

            var cache = new CouchbaseCache(provider.Object, new CouchbaseCacheOptions());
            var session = new CouchbaseSession(cache, "thesessionkey", new TimeSpan(0, 0, 10, 0), () => true, new LoggerFactory(), true);

            var poco = ConvertToBytes(new Poco { Name = "jeff" });
            session.Set("Set_Tests", poco);

            byte[] actual;
            var result = session.TryGetValue("Set_Tests", out actual);

            Assert.True(result);
            Assert.Same(poco, actual);
        }

        [Fact]
        public void TryGetValue_When_NotFound_Return_Null()
        {
            var op = new Mock<IOperationResult<Dictionary<string, object>>>(); //value is null

            var bucket = new Mock<IBucket>();
            bucket.Setup(x => x.GetAsync<Dictionary<string, object>>(It.IsAny<string>()))
                .Returns(Task.FromResult(op.Object));

            var provider = new Mock<ICouchbaseCacheBucketProvider>();
            provider.Setup(x => x.GetBucket()).Returns(bucket.Object);

            var cache = new CouchbaseCache(provider.Object, new CouchbaseCacheOptions());
            var session = new CouchbaseSession(cache, "thesessionkey", new TimeSpan(0, 0, 10, 0), () => true, new LoggerFactory(), true);

            byte[] actual;
            var result = session.TryGetValue("Set_Tests", out actual);

            Assert.False(result);
            Assert.Same(null, actual);
        }

        [Fact]
        public void TryGetValue_When_ByteArray_Return_ByteArray()
        {
            var op = new Mock<IOperationResult<Dictionary<string, object>>>(); //value is null

            var bucket = new Mock<IBucket>();
            bucket.Setup(x => x.GetAsync<Dictionary<string, object>>(It.IsAny<string>()))
                .Returns(Task.FromResult(op.Object));

            var provider = new Mock<ICouchbaseCacheBucketProvider>();
            provider.Setup(x => x.GetBucket()).Returns(bucket.Object);

            var cache = new CouchbaseCache(provider.Object, new CouchbaseCacheOptions());
            var session = new CouchbaseSession(cache, "thesessionkey", new TimeSpan(0, 0, 10, 0), () => true, new LoggerFactory(), true);

            var poco = ConvertToBytes(new Poco { Name = "jeff" });
            session.Set("Set_Tests", poco);

            byte[] actual;
            var result = session.TryGetValue("Set_Tests", out actual);

            Assert.True(result);
            Assert.Same(poco, actual);
        }

        [Fact]
        public void Clear_Test()
        {
            var op = new Mock<IOperationResult<Dictionary<string, object>>>(); //value is null

            var bucket = new Mock<IBucket>();
            bucket.Setup(x => x.GetAsync<Dictionary<string, object>>(It.IsAny<string>()))
                .Returns(Task.FromResult(op.Object));

            var provider = new Mock<ICouchbaseCacheBucketProvider>();
            provider.Setup(x => x.GetBucket()).Returns(bucket.Object);

            var cache = new CouchbaseCache(provider.Object, new CouchbaseCacheOptions());
            var session = new CouchbaseSession(cache, "thesessionkey", new TimeSpan(0, 0, 10, 0), () => true, new LoggerFactory(), true);

            var poco = new Poco { Name = "jeff" };
            session.Set("Clear_Test", poco);

            Assert.NotEmpty(session.Keys);

            session.Clear();

            Assert.Empty(session.Keys);
        }

        [Fact]
        public void Ctor_When_Cache_Is_Null_ArgumentNullException()
        {
            var loggerFactory = new Mock<ILoggerFactory>();
            Assert.Throws<ArgumentNullException>(()=>new CouchbaseSession(null, "thekey", new TimeSpan(0, 0, 10, 0), () => true, loggerFactory.Object, true));
        }

        [Fact]
        public void Ctor_When_SessionKey_Is_Null_ArgumentException()
        {
            var cache = new Mock<IDistributedCache>();
            var loggerFactory = new Mock<ILoggerFactory>();
            Assert.Throws<ArgumentException>(() => new CouchbaseSession(cache.Object, null, new TimeSpan(0, 0, 10, 0), () => true, loggerFactory.Object, true));
        }

        [Fact]
        public void Ctor_When_TryEstablishSession_Is_Null_ArgumentNullException()
        {
            var cache = new Mock<IDistributedCache>();
            var loggerFactory = new Mock<ILoggerFactory>();
            Assert.Throws<ArgumentNullException>(() => new CouchbaseSession(cache.Object, "thekey", new TimeSpan(0, 0, 10, 0), null, loggerFactory.Object, true));
        }

        [Fact]
        public void Ctor_When_LoggerFactory_Is_Null_ArgumentNullException()
        {
            var cache = new Mock<IDistributedCache>();

            Assert.Throws<ArgumentNullException>(() => new CouchbaseSession(cache.Object, "thekey", new TimeSpan(0, 0, 10, 0), () => true, null, true));
        }

        public byte[] ConvertToBytes(object value)
        {
            using (var ms = new MemoryStream())
            {
                using (var sw = new StreamWriter(ms))
                {
                    using (var jr = new JsonTextWriter(sw))
                    {
                        var serializer = JsonSerializer.Create();
                        serializer.Serialize(jr, value);
                    }
                }
                return ms.ToArray();
            }
        }

        public class Poco
        {
            public string Name { get; set; }
        }
    }
}
