using System;
using Couchbase.Extensions.Caching.Internal;
using Xunit;

namespace Couchbase.Extensions.Caching.UnitTests
{
    public class CacheMetadataTests
    {
        [Fact]
        public void GetRelativeExpiration_WhenAbsoluteExpirationIsSet_ReturnsCorrectTimeSpan()
        {
            // Arrange
            var utcNow = DateTimeOffset.UtcNow;
            var cacheInfo = new CacheMetadata
            {
                AbsoluteExpiration = utcNow.AddMinutes(10)
            };

            // Act
            var result = cacheInfo.GetRelativeExpiration(utcNow);

            // Assert
            Assert.Equal(TimeSpan.FromMinutes(10), result);
        }

        [Fact]
        public void GetRelativeExpiration_WhenSlidingExpirationIsSet_ReturnsCorrectTimeSpan()
        {
            // Arrange
            var utcNow = DateTimeOffset.UtcNow;
            var cacheInfo = new CacheMetadata
            {
                SlidingExpiration = TimeSpan.FromMinutes(10)
            };

            // Act
            var result = cacheInfo.GetRelativeExpiration(utcNow);

            // Assert
            Assert.Equal(TimeSpan.FromMinutes(10), result);
        }

        [Fact]
        public void GetRelativeExpiration_WhenSlidingExpirationAndAbsoluteExpirationAreSet_ReturnsCorrectTimeSpan()
        {
            // Arrange
            var utcNow = DateTimeOffset.UtcNow;
            var cacheInfo = new CacheMetadata
            {
                AbsoluteExpiration = utcNow.AddMinutes(10),
                SlidingExpiration = TimeSpan.FromMinutes(5)
            };

            // Act
            var result = cacheInfo.GetRelativeExpiration(utcNow);

            // Assert
            Assert.Equal(TimeSpan.FromMinutes(5), result);
        }
    }
}
