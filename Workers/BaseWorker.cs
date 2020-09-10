using System;

namespace GZipTestApp.Workers
{
    public abstract class BaseWorker : IWorker
    {
        private bool forcedStop;

        public void DoWork(int threadId, Func<int, byte[]> consumeSource, Action<int, byte[]> produceResult)
        {
            while (!forcedStop)
            {
                byte[] block = consumeSource(threadId);

                if (block == null) break;

                byte[] processedBlock = ProcessBlock(block);

                produceResult(threadId, processedBlock);
            }
        }

        public void StopByForce()
        {
            forcedStop = true;
        }

        protected abstract byte[] ProcessBlock(byte[] block);
    }
}
