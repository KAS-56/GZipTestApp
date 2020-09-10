using System;
using System.IO;
using System.IO.Compression;

namespace GZipTestApp.Workers
{
    public class CompressorWorker : BaseWorker
    {
        protected override byte[] ProcessBlock(byte[] block)
        {
            byte[] bytes;
            using (MemoryStream ms = new MemoryStream())
            {
                using (GZipStream gzipStream = new GZipStream(ms, CompressionMode.Compress))
                {
                    gzipStream.Write(block, 0, block.Length);
                }

                bytes = ms.ToArray();
            }

            bytes[Consts.FlagsHeaderPosition] = Consts.FlagsValue;
            int compressedBlockLength = bytes.Length + Consts.ExtraFieldLength;

            byte[] extraBytes = GetExtraFieldBytes(compressedBlockLength);
            byte[] compressedBlock = new byte[compressedBlockLength];

            Buffer.BlockCopy(bytes, 0, compressedBlock, 0, Consts.GzipHeaderLength);
            Buffer.BlockCopy(extraBytes, 0, compressedBlock, Consts.GzipHeaderLength, Consts.ExtraFieldLength);
            Buffer.BlockCopy(bytes, Consts.GzipHeaderLength, compressedBlock, Consts.GzipHeaderWithExtraFieldLength, bytes.Length - Consts.GzipHeaderLength);

            return compressedBlock;
        }

        private static byte[] GetExtraFieldBytes(int compressBlockLength)
        {
            // Fixing error 'Arithmetic operation resulted in an overflow.'
            // Direct cast int to byte can throw OverflowException: Arithmetic operation resulted in an overflow.
            // Possible two way to fix:
            // - use 'unchecked' block
            // - prevent overflow by zeroing the first 24 bit in int
            return new byte[]
                   {
                       8, 0, // XLEN
                       90, 90, // SI1, SI2
                       4, 0, // LEN
                       (byte) (0x000000FF & compressBlockLength),
                       (byte) (0x000000FF & (compressBlockLength >> 8)),
                       (byte) (0x000000FF & (compressBlockLength >> 16)),
                       (byte) (compressBlockLength >> 24)
                   };
        }
    }
}
