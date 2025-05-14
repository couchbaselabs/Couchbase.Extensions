using System;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace Couchbase.Extensions.Locks.Internal
{
    /// <summary>
    /// System.Text.Json converter that represents DateTime as milliseconds since the Unix epoch.
    /// </summary>
    internal class UnixMillisecondsJsonConverter : JsonConverter<DateTime>
    {
        private static readonly DateTime UnixEpoch = new(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc);

        public override DateTime Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
        {
            if (reader.TokenType == JsonTokenType.Number)
            {
                var dbl = reader.GetDouble();
                var ticks = (long)(dbl * TimeSpan.TicksPerMillisecond);
                return new DateTime(UnixEpoch.Ticks + ticks, DateTimeKind.Utc);
            }
            else if (reader.TokenType == JsonTokenType.String)
            {
                // Backward compatibility for consumers using System.Text.Json when this library was designed
                // for Newtonsoft.Json, in which case ISO8601 strings were stored.

                return reader.GetDateTime();
            }

            throw new JsonException("Expected number or string for DateTime value.");
        }

        public override void Write(Utf8JsonWriter writer, DateTime value, JsonSerializerOptions options)
        {
            if (value.Kind == DateTimeKind.Local)
            {
                value = value.ToUniversalTime();
            }

            var unixMilliseconds = (value - UnixEpoch).TotalMilliseconds;
            writer.WriteNumberValue(unixMilliseconds);
        }
    }
}


/* ************************************************************
 *
 *    @author Couchbase <info@couchbase.com>
 *    @copyright 2021 Couchbase, Inc.
 *
 *    Licensed under the Apache License, Version 2.0 (the "License");
 *    you may not use this file except in compliance with the License.
 *    You may obtain a copy of the License at
 *
 *        http://www.apache.org/licenses/LICENSE-2.0
 *
 *    Unless required by applicable law or agreed to in writing, software
 *    distributed under the License is distributed on an "AS IS" BASIS,
 *    WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
 *    See the License for the specific language governing permissions and
 *    limitations under the License.
 *
 * ************************************************************/
