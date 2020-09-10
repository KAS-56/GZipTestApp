using System.IO;
using System.IO.Compression;

namespace GZipTestApp.Workers
{
    public class DecompressorWorker : BaseWorker
    {
        protected override byte[] ProcessBlock(byte[] block)
        {
            using (MemoryStream inStream = new MemoryStream(block))
            {
                using (GZipStream gzipStream = new GZipStream(inStream, CompressionMode.Decompress))
                {
                    using (MemoryStream outStream = new MemoryStream())
                    {
                        gzipStream.CopyTo(outStream);
                        return outStream.ToArray();
                    }
                }
            }
        }
    }
}
