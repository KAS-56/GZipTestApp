using System;
using System.IO;

namespace GZipTestApp.Readers
{
    public abstract class BaseReader : IReader
    {
        private readonly string sourceFile;
        private readonly int maxThreads;

        private bool forcedStop;

        public event EventHandler DataIsOver;

        protected BaseReader(string sourceFile, int maxThreads)
        {
            this.sourceFile = sourceFile;
            this.maxThreads = maxThreads;
        }

        public void ReadSource(Action<int, byte[]> produceSource)
        {
            int i = 0;
            using (FileStream input = new FileStream(sourceFile, FileMode.Open, FileAccess.Read, FileShare.Read))
            {
                if (input.Length == 0) throw new CustomException("Source file is empty.");

                while (!forcedStop && input.Position < input.Length)
                {
                    byte[] sourceBlock = ReadSourceBlock(input);

                    produceSource(i, sourceBlock);

                    i++;
                    i = i == maxThreads ? 0 : i;
                }

                DataIsOver?.Invoke(null, EventArgs.Empty);
            }
        }

        protected abstract byte[] ReadSourceBlock(FileStream input);

        public void StopByForce()
        {
            forcedStop = true;
        }
    }
}
