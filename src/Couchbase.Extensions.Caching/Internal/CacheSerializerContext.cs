using System.Text.Json.Serialization;

namespace Couchbase.Extensions.Caching.Internal
{
    [JsonSerializable(typeof(CacheMetadata))]
    [JsonSourceGenerationOptions(
        PropertyNamingPolicy = JsonKnownNamingPolicy.CamelCase,
        DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull)]
    internal partial class CacheSerializerContext : JsonSerializerContext;
}
