using System;
using System.IO;

namespace GZipTestApp.Readers
{
    public class DecompressorReader : BaseReader
    {
        public DecompressorReader(string sourceFile, int maxThreads) : base(sourceFile, maxThreads)
        {
        }

        protected override byte[] ReadSourceBlock(FileStream input)
        {
            byte[] block = new byte[Consts.GzipHeaderWithExtraFieldLength];
            int readBytes = input.Read(block, 0, Consts.GzipHeaderWithExtraFieldLength);
            if (readBytes != Consts.GzipHeaderWithExtraFieldLength) throw new CustomException("Unexpected count of reading bytes.");

            int blockLength = ExtractCompressedBlockLength(block);

            Array.Resize(ref block, blockLength);

            int remainBytes = blockLength - Consts.GzipHeaderWithExtraFieldLength;

            readBytes = input.Read(block, Consts.GzipHeaderWithExtraFieldLength, remainBytes);
            if (readBytes != remainBytes) throw new CustomException("Unexpected count of reading bytes.");

            return block;
        }

        private static int ExtractCompressedBlockLength(byte[] headerBuffer)
        {
            return headerBuffer[16] | headerBuffer[17] << 8 | headerBuffer[18] << 16 | headerBuffer[19] << 24;
        }
    }
}
