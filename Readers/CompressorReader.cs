using System;
using System.IO;

namespace GZipTestApp.Readers
{
    public class CompressorReader : BaseReader
    {
        private readonly int blockSize;

        public CompressorReader(int blockSize, string sourceFile, int maxThreads) : base(sourceFile, maxThreads)
        {
            this.blockSize = blockSize;
        }

        protected override byte[] ReadSourceBlock(FileStream input)
        {
            byte[] block = new byte[blockSize];
            int readBytes = input.Read(block, 0, blockSize);

            if (readBytes != blockSize && input.Position != input.Length) throw new CustomException("Unexpected count of reading bytes.");

            if (readBytes != blockSize)
            {
                Array.Resize(ref block, readBytes);
            }

            return block;
        }
    }
}
