﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Reactive.Linq;
using System.Reactive.Threading.Tasks;
using System.Threading.Tasks;
using Xunit;

namespace Couchbase.Extensions.MultiOp.IntegrationTests
{
    public class ReplaceTests : IClassFixture<ClusterFixture>
    {
        private static readonly MultiOpOptions HighParallelism = new MultiOpOptions
        {
            DegreeOfParallelism = 100
        };

        public ReplaceTests(ClusterFixture fixture)
        {
            _fixture = fixture;
        }

        private readonly ClusterFixture _fixture;

        [Theory]
        [InlineData(10)]
        [InlineData(100_000)]
        public async Task Replace(int size)
        {
            var collection = (await _fixture.GetDefaultBucketAsync()).DefaultCollection();

            var keys = Enumerable.Range(1, size)
                .Select(_ => Guid.NewGuid().ToString())
                .ToList();

            try
            {
                await collection.Insert(keys.Select(p => new KeyValuePair<string, TestDoc>(p, new TestDoc {Value = p})), HighParallelism)
                    .EnsureSuccessfulAsync();

                var result = await collection.Replace(keys.Select(p => new KeyValuePair<string, TestDoc>(p, new TestDoc {Value = p})), HighParallelism)
                    .ToList();

                Assert.Equal(size, result.Count);
                Assert.All(result, p =>
                {
                    Assert.True(p.IsSuccessful);
                    Assert.NotNull(p.Result);
                });
            }
            finally
            {
                await collection.Remove(keys, HighParallelism);
            }
        }

        private class TestDoc
        {
            public string Value { get; set; }
        }
    }
}
