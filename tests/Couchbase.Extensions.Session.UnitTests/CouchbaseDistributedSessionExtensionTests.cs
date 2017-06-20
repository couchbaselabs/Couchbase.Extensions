using System;
using Microsoft.AspNetCore.Http;
using Moq;
using Xunit;

namespace Couchbase.Extensions.Session.UnitTests
{
    public class CouchbaseDistributedSessionExtensionTests
    {
        [Fact]
        public void When_Session_Is_Not_CouchbaseDistributedSession_SetObject_Throws_NotSupportedException()
        {
           var session = new Mock<ISession>();
           Assert.Throws<NotSupportedException>(()=>session.Object.SetObject("key", "value"));
        }

        [Fact]
        public void When_Session_Is_Not_CouchbaseDistributedSession_GetObject_Throws_NotSupportedException()
        {
            var session = new Mock<ISession>();
            Assert.Throws<NotSupportedException>(() => session.Object.GetObject<string>("key"));
        }
    }
}
