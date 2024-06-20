using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Couchbase.Core.Exceptions;
using Couchbase.Extensions.Caching;
using Couchbase.Extensions.Caching.Internal;
using Couchbase.KeyValue;
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
            var cache = new CouchbaseInMemoryCache(TimeProvider.System);
            await cache.SetAsync("thekey", Array.Empty<byte>(),
                new DistributedCacheEntryOptions { AbsoluteExpirationRelativeToNow = TimeSpan.FromMinutes(1) });

            ISession session = new CouchbaseSession(cache, "thekey", new TimeSpan(0,0, 10, 0), TimeSpan.FromSeconds(10), ()=>true, new LoggerFactory(), true);

            await session.LoadAsync(CancellationToken.None);

            Assert.True(session.IsAvailable);
        }

        [Fact]
        public async Task LoadAsync_When_Cache_DoesNotHave_BackingStore_Session_IsAvailable_True()
        {
            var cache = new CouchbaseInMemoryCache(TimeProvider.System);
            await cache.SetAsync<byte[]>("thekey", null,
                new DistributedCacheEntryOptions { AbsoluteExpirationRelativeToNow = TimeSpan.FromMinutes(1) });

            ISession session = new CouchbaseSession(cache, "thekey", new TimeSpan(0, 0, 10, 0), TimeSpan.FromSeconds(10), () => true, new LoggerFactory(), true);

            await session.LoadAsync(CancellationToken.None);

            Assert.True(session.IsAvailable);
        }


        [Fact]
        public async Task LoadAsync_When_Cache_ThrowsException_BackingStore_Session_IsAvailable_False()
        {
            var cache = new CouchbaseInMemoryCache(TimeProvider.System)
            {
                DisableGet = true
            };

            ISession session = new CouchbaseSession(cache, "thekey", new TimeSpan(0, 0, 10, 0), TimeSpan.FromSeconds(10), () => true, new LoggerFactory(), true);

            try
            {
                await session.LoadAsync(CancellationToken.None);
            }
            catch (Exception)
            {
                Assert.False(session.IsAvailable);
            }
        }


        [Fact]
        public void TryGetValue_Success()
        {
            ISession session = MockSession();
            session.Set("thekey", "thevalue");

            string value;
            var result = session.TryGetValue("thekey", out value);
            Assert.True(result);
            Assert.NotNull(value);
        }

        [Fact]
        public void TryGetValueOfT_String_Success()
        {
            ISession session = MockSession();

            session.Set("thekey", "thevalue");

            string value;
            var result = session.TryGetValue("thekey", out value);
            Assert.True(result);
            Assert.NotNull(value);
        }

        [Fact]
        public void TryGetValueOfT_Poco_Success()
        {
            ISession session = MockSession();

            var poco = new Poco { Name = "jeff" };
            session.Set("thekey", poco);

            Poco value;
            var result = session.TryGetValue("thekey", out value);
            Assert.True(result);
            Assert.Equal(poco.Name, value.Name);
        }

        [Fact]
        public void GetValue_WhenString_Success()
        {
            ISession session = MockSession();

            session.Set("thekey", "thevalue");

            var result = session.Get("thekey");
            Assert.Equal(ConvertToBytes("thevalue"), result);
        }

        [Fact]
        public void GetValue_WhenPoco_Success()
        {
            ISession session = MockSession();

            session.Set("thekeyforpoco", new Poco { Name = "jeff" });

            var result = session.Get("thekeyforpoco");
            Assert.Equal(ConvertToBytes(new Poco {Name = "jeff"}), result);
        }

        [Fact]
        public void Remove_Success()
        {
            ISession session = MockSession();

            var poco = new Poco {Name = "jeff"};
            session.Set("remove_success", poco);
            Assert.Contains("remove_success", session.Keys.Cast<string>());

            session.Remove("remove_success");
            Assert.DoesNotContain("remove_success", session.Keys.Cast<string>());
        }

        [Fact]
        public void SetOfT_Test()
        {
            ISession session = MockSession();

            var poco = new Poco { Name = "jeff" };
            session.Set("SetOfT_Tests", poco);

            Poco actual;
            var result = session.TryGetValue("SetOfT_Tests", out actual);

            Assert.True(result);
            Assert.Equal(poco.Name, actual.Name);
        }

        [Fact]
        public void Set_Test()
        {
            ISession session = MockSession();
            var poco1 = new Poco {Name = "jeff"};
            session.Set("Set_Tests", poco1);

            Poco actual;
            var result = session.TryGetValue("Set_Tests", out actual);

            Assert.True(result);
            Assert.NotStrictEqual(poco1, actual);
        }

        [Fact]
        public void TryGetValue_When_NotFound_Return_Null()
        {
            ISession session = MockSession();

            byte[] actual;
            var result = session.TryGetValue("Set_Tests", out actual);

            Assert.False(result);
            Assert.Null(actual);
        }

        [Fact]
        public void TryGetValue_When_ByteArray_Return_ByteArray()
        {
            ISession session = MockSession();

            var poco = ConvertToBytes(new Poco { Name = "jeff" });
            session.Set("Set_Tests", poco);

            byte[] actual;
            var result = session.TryGetValue("Set_Tests", out actual);

            Assert.True(result);
            Assert.Equal(poco, actual);
        }

        [Fact]
        public void Clear_Test()
        {
            ISession session = MockSession();

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
            Assert.Throws<ArgumentNullException>(()=>new CouchbaseSession(null, "thekey", new TimeSpan(0, 0, 10, 0), TimeSpan.Zero, () => true, loggerFactory.Object, true));
        }

        [Fact]
        public void Ctor_When_SessionKey_Is_Null_ArgumentException()
        {
            var cache = new Mock<ICouchbaseCache>();
            var loggerFactory = new Mock<ILoggerFactory>();
            Assert.Throws<ArgumentException>(() => new CouchbaseSession(cache.Object, null, new TimeSpan(0, 0, 10, 0), TimeSpan.Zero, () => true, loggerFactory.Object, true));
        }

        [Fact]
        public void Ctor_When_TryEstablishSession_Is_Null_ArgumentNullException()
        {
            var cache = new Mock<ICouchbaseCache>();
            var loggerFactory = new Mock<ILoggerFactory>();
            Assert.Throws<ArgumentNullException>(() => new CouchbaseSession(cache.Object, "thekey", new TimeSpan(0, 0, 10, 0), TimeSpan.Zero, null, loggerFactory.Object, true));
        }

        [Fact]
        public void Ctor_When_LoggerFactory_Is_Null_ArgumentNullException()
        {
            var cache = new Mock<ICouchbaseCache>();

            Assert.Throws<ArgumentNullException>(() => new CouchbaseSession(cache.Object, "thekey", new TimeSpan(0, 0, 10, 0), TimeSpan.Zero, () => true, null, true));
        }

        private CouchbaseSession MockSession()
        {
            var op = new Mock<IGetResult>();

            var collection = new Mock<ICouchbaseCollection>();
            collection
                .Setup(x => x.GetAsync(It.IsAny<string>(), It.IsAny<GetOptions>()))
                .Returns(Task.FromResult(op.Object));

            var bucket = new Mock<IBucket>();
            bucket.Setup(x => x.DefaultCollection()).Returns(collection.Object);

            var provider = new Mock<ICouchbaseCacheCollectionProvider>();
            provider.Setup(x => x.GetCollectionAsync()).Returns(new ValueTask<ICouchbaseCollection>(collection.Object));

            var cache = new CouchbaseInMemoryCache(TimeProvider.System);

            return new CouchbaseSession(cache, "thekey", new TimeSpan(0, 0, 10, 0), TimeSpan.FromSeconds(10), () => true, new LoggerFactory(), true);
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
