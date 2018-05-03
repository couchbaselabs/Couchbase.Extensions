using System;
using System.IO;
using System.Reflection;
using System.Runtime.InteropServices;

namespace Snappy
{
    internal abstract class NativeProxy
    {
        public static readonly NativeProxy Instance = RuntimeInformation.IsOSPlatform(OSPlatform.Windows)
            ? (IntPtr.Size == 4 ? (NativeProxy) new Native32() : new Native64())
            : new Native();

        protected NativeProxy(string name)
        {
            if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
            {
                var assembly = typeof(SnappyCodec).GetTypeInfo().Assembly;
                var folder = Path.Combine(Path.GetTempPath(), "Couchbase.Extensions.Compression-" + assembly.GetName().Version);
                Directory.CreateDirectory(folder);
                var path = Path.Combine(folder, name);

                if (!File.Exists(path))
                {
                    byte[] contents;
                    using (var input = assembly.GetManifestResourceStream("Couchbase.Extensions.Compression.lib." + name))
                    using (var buffer = new MemoryStream())
                    {
                        var block = new byte[4096];
                        int copied;
                        while ((copied = input.Read(block, 0, block.Length)) != 0)
                        {
                            buffer.Write(block, 0, copied);
                        }
                        contents = buffer.ToArray();
                    }

                    using (var output = File.Open(path, FileMode.Create, FileAccess.Write, FileShare.None))
                    {
                        output.Write(contents, 0, contents.Length);
                    }
                }

                var ptr = LoadLibrary(path);
                if (ptr == IntPtr.Zero)
                {
                    throw new Exception(path);
                }
            }
        }

        public abstract unsafe SnappyStatus Compress(byte* input, int inLength, byte* output, ref int outLength);
        public abstract unsafe SnappyStatus Uncompress(byte* input, int inLength, byte* output, ref int outLength);
        public abstract int GetMaxCompressedLength(int inLength);
        public abstract unsafe SnappyStatus GetUncompressedLength(byte* input, int inLength, out int outLength);
        public abstract unsafe SnappyStatus ValidateCompressedBuffer(byte* input, int inLength);

        [DllImport("kernel32", SetLastError = true, CharSet = CharSet.Unicode)]
        static extern IntPtr LoadLibrary(string lpFileName);
    }
}
