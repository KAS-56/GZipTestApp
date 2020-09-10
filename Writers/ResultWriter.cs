using System;
using System.IO;

namespace GZipTestApp.Writers
{
    public class ResultWriter : IWriter
    {
        private readonly string targetFile;
        private readonly int maxThreads;
        private bool forcedStop;

        public ResultWriter(string targetFile, int maxThreads)
        {
            this.targetFile = targetFile;
            this.maxThreads = maxThreads;
        }

        public void WriteResult(Func<int, byte[]> consumeResult)
        {
            int i = 0;
            using (FileStream output = new FileStream(targetFile, FileMode.Create, FileAccess.Write, FileShare.None))
            {
                while (!forcedStop)
                {
                    byte[] block = consumeResult(i);

                    if (block == null) break;

                    output.Write(block, 0, block.Length);

                    i++;
                    i = i == maxThreads ? 0 : i;
                }
            }
        }

        public void StopByForce()
        {
            forcedStop = true;
        }
    }
}
