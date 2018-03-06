using System;
using Couchbase.IO.Operations;
using Xunit;

namespace Couchbase.Snappy.UnitTests
{
    public class SnappyTranscoderTests
    {
        private const byte Snappy = 0x02; // todo: replace with Couchbase DateType enum

        [Fact]
        public void Can_compress_and_decompress()
        {
            var person = new Person("Billy", "Bob", 123);
            var flags = new Flags
            {
                Compression = Compression.None,
                DataFormat = (DataFormat) Snappy,
                TypeCode = TypeCode.String
            };
            var serializer = new SnappyTranscoder();
            var compressed = serializer.Encode(person, flags, OperationCode.Add);
            Assert.NotNull(compressed);
            Assert.True(compressed.Length > 0);

            var result = serializer.Decode<Person>(compressed, 0, compressed.Length, flags, OperationCode.Add);
            Assert.NotNull(result);
            Assert.Equal(person.First, result.First);
            Assert.Equal(person.Last, result.Last);
            Assert.Equal(person.Age, result.Age);
        }

        private class Person
        {
            public string First { get; }
            public string Last { get; }
            public int Age { get; }

            public Person(string first, string last, int age)
            {
                First = first;
                Last = last;
                Age = age;
            }
        }
    }
}
