using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Moq;
using Xunit;

namespace Couchbase.Extensions.Session.UnitTests
{
    public class CouchbaseDistributedSessionExtensionTests
    {
        [Fact]
        public async Task When_Session_Is_Not_CouchbaseDistributedSession_SetObject_Throws_NotSupportedException()
        {
           var session = new Mock<ISession>();
           await Assert.ThrowsAsync<NotSupportedException>(()=>session.Object.SetObject("key", "value"));
        }

        [Fact]
        public async Task When_Session_Is_Not_CouchbaseDistributedSession_GetObject_Throws_NotSupportedException()
        {
            var session = new Mock<ISession>();
            await Assert.ThrowsAsync<NotSupportedException>(() => session.Object.GetObject<string>("key"));
        }
    }
}
