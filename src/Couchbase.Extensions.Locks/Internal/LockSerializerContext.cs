using System.Text.Json.Serialization;

namespace Couchbase.Extensions.Locks.Internal
{
    [JsonSourceGenerationOptions(
        PropertyNameCaseInsensitive = true, // backward compatibility
        PropertyNamingPolicy = JsonKnownNamingPolicy.CamelCase)]
    [JsonSerializable(typeof(LockDocument))]
    internal sealed partial class LockSerializerContext : JsonSerializerContext
    {
    }
}
