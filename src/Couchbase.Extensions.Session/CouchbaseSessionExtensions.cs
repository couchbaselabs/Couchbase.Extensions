using System;
using System.IO;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Newtonsoft.Json;

namespace Couchbase.Extensions.Session
{
    /// <summary>
    /// Extensions for persisting POCO's to the session store.
    /// </summary>
    public static class CouchbaseSessionExtensions
    {
        /// <summary>
        /// Retrieve the value of the given key, if present from session state.
        /// </summary>
        /// <typeparam name="T">The <see cref="Type"/> of the value stored in session state.</typeparam>
        /// <param name="session">The <see cref="CouchbaseSession"/> instance where the session is stored.</param>
        /// <param name="key">The unique identifier for the value.</param>
        /// <param name="value">The value to store in session state.</param>
        /// <returns>True if key is present, otherwise false.</returns>
        public static bool TryGetValue<T>(this ISession session, string key, out T value)
        {
            value = default;

            try
            {
                var success = session.TryGetValue(key, out var item);
                if (success)
                {
                    var buffer = item.AsMemory();
                    value = JsonConvert.DeserializeObject<T>(Encoding.UTF8.GetString(buffer.ToArray()));
                }

                return success;
            }
            catch (Exception)
            {
                //need to log this
                return false;
            }
        }

        /// <summary>
        /// Stores a value in session state given a key. The value is stored as a byte array in Couchbase.
        /// </summary>
        /// <typeparam name="T">The <see cref="Type"/> of the value.</typeparam>
        /// <param name="session">The <see cref="CouchbaseSession"/> instance where the session is stored.</param>
        /// <param name="key">The unique identifier for the value.</param>
        /// <param name="value">The value to store in session state.</param>
        public static void Set<T>(this ISession session, string key, T value)
        {
            if (value == null)
            {
                throw new ArgumentNullException(nameof(value));
            }

            var bytes = ConvertToBytes(value);
            session.Set(key, bytes);
        }

        internal static byte[] ConvertToBytes(object value)
        {
            //if already a byte array just return it
            if (value.GetType() == typeof(byte[]))
            {
                return (byte[])value;
            }

            //if not convert it to a JSON byte array
            using var ms = new MemoryStream();
            using (var sw = new StreamWriter(ms))
            {
                using (var jr = new JsonTextWriter(sw))
                {
                    var serializer = JsonSerializer.Create();
                    serializer.Serialize(jr, value);
                }
            }
            return ms.ToArray();
        }

        /// <summary>
        /// Sets the object of Type T into the Couchbase session store.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="session">The session.</param>
        /// <param name="key">The key for the item.</param>
        /// <param name="value">The value to persist into the session store.</param>
        /// <exception cref="System.NotSupportedException">Only supported by CouchbaseDistributedSession!</exception>
        public static async Task SetObject<T>(this ISession session, string key, T value)
        {
            if (!(session is CouchbaseSession couchbaseSession))
            {
                throw new NotSupportedException("Only supported by CouchbaseSession!");
            }
            await session.LoadAsync().ConfigureAwait(false);
            Set(session, key, value);
        }

        /// <summary>
        /// Gets the object of Type T from the Couchbase session store.
        /// </summary>
        /// <typeparam name="T">The <see cref="Type"/> to persist to the session store.</typeparam>
        /// <param name="session">The session.</param>
        /// <param name="key">The key for the item.</param>
        /// <returns>The item for the key as "T".</returns>
        /// <exception cref="System.NotSupportedException">Only supported by CouchbaseDistributedSession!</exception>
        public static async Task<T> GetObject<T>(this ISession session, string key)
        {
            if (!(session is CouchbaseSession couchbaseSession))
            {
                throw new NotSupportedException("Only supported by CouchbaseSession!");
            }

            await session.LoadAsync().ConfigureAwait(false);
            TryGetValue(session, key, out T value);
            return value;
        }
    }
}
