namespace Couchbase.Core.Snappy
{
    //TODO: Use Couchbase SDK interface
    public interface ISnappyCompressor
    {
        int MinSize { get; set; }
        double MinRatio { get; set; }

        byte[] Compress(byte[] buffer, int offset, int length);
        byte[] Uncompress(byte[] buffer, int offset, int length);
    }
}
