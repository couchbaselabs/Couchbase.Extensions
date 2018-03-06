using Couchbase.Core.Transcoders;
using Couchbase.IO.Operations;
using Snappy;

namespace Couchbase.Snappy
{
    public class SnappyTranscoder : DefaultTranscoder
    {
        private const byte Snappy = 0x02; // todo: replace with Couchbase DateType enum

        public override T Decode<T>(byte[] buffer, int offset, int length, Flags flags, OperationCode opcode)
        {
            if ((ushort) flags.DataFormat == Snappy)
            {
                var decodedBuffer = SnappyCodec.Uncompress(buffer);
                var result = base.Decode<T>(decodedBuffer, offset, length, flags, opcode);
                flags.DataFormat = DataFormat.Json; // reset dataformat back to JSON
                return result;
            }

            return base.Decode<T>(buffer, offset, length, flags, opcode);
        }

        public override byte[] Encode<T>(T value, Flags flags, OperationCode opcode)
        {
            var buffer = base.Encode(value, flags, opcode);
            if ((ushort) flags.DataFormat == Snappy)
            {
                return SnappyCodec.Compress(buffer);
            }

            return buffer;
        }
    }
}
