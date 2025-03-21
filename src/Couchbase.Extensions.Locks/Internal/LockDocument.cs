using System;
using System.Text.Json.Serialization;

namespace Couchbase.Extensions.Locks.Internal
{
    internal class LockDocument
    {
        private const string LockPrefix = "__lock_";

        public string? Name { get; set; }

        public string? Holder { get; set; }

        [JsonConverter(typeof(UnixMillisecondsJsonConverter))]
        public DateTime RequestedDateTime { get; set; }

        public static string GetKey(string name)
        {
            return LockPrefix + name;
        }
    }
}
