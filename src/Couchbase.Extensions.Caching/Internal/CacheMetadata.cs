using System;
using System.Text.Json.Serialization;
using Microsoft.Extensions.Caching.Distributed;

namespace Couchbase.Extensions.Caching.Internal
{
    internal class CacheMetadata
    {
        // Note: at least one of AbsoluteExpiration and SlidingExpiration must be non-null

        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public DateTimeOffset? AbsoluteExpiration { get; set; }

        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public TimeSpan? SlidingExpiration { get; set; }

        public static CacheMetadata Create(DistributedCacheEntryOptions options, DateTimeOffset utcNow)
        {
            var metadata = new CacheMetadata
            {
                SlidingExpiration = options.SlidingExpiration
            };

            if (options.AbsoluteExpirationRelativeToNow is not null)
            {
                // Prefer AbsoluteExpirationRelativeToNow over AbsoluteExpiration
                metadata.AbsoluteExpiration =
                    utcNow.Add(options.AbsoluteExpirationRelativeToNow.Value);
            }
            else if (options.AbsoluteExpiration is not null)
            {
                if (options.AbsoluteExpiration.Value <= utcNow)
                {
                    throw new InvalidOperationException("The absolute expiration value must be in the future.");
                }

                metadata.AbsoluteExpiration = options.AbsoluteExpiration;
            }
            else if (options.SlidingExpiration is null)
            {
                throw new InvalidOperationException("At least one of the absolute or sliding expiration values must be set.");
            }

            return metadata;
        }

        public TimeSpan GetRelativeExpiration(DateTimeOffset utcNow)
        {
            var slidingExpiration = SlidingExpiration;
            if (AbsoluteExpiration is not null)
            {
                if (slidingExpiration is null || utcNow.Add(slidingExpiration.GetValueOrDefault()) > AbsoluteExpiration.GetValueOrDefault())
                {
                    return AbsoluteExpiration.GetValueOrDefault() - utcNow;
                }
            }

            return slidingExpiration.GetValueOrDefault();
        }
    }
}
