using System;
using System.Threading.Tasks;
using Xunit;

namespace Couchbase.Extensions.Compression.Snappier.IntegrationTests
{
    public class CompressionTests : IClassFixture<ClusterFixture>
    {
        public CompressionTests(ClusterFixture fixture)
        {
            _fixture = fixture;
        }

        private readonly ClusterFixture _fixture;

        [Fact]
        public async Task InsertAndGet()
        {
            var collection = await _fixture.GetDefaultCollectionAsync();
            var key = Guid.NewGuid().ToString();

            try
            {
                await collection.InsertAsync(key, new {name = "mike", data=new string('X', 500)});

                using (var result = await collection.GetAsync(key))
                {
                    var content = result.ContentAs<dynamic>();

                    Assert.Equal("mike", (string) content.name);
                    Assert.Equal(500, ((string) content.data).Length);
                }
            }
            finally
            {
                await collection.RemoveAsync(key);
            }
        }
    }
}
