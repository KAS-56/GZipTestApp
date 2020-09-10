using System;

namespace GZipTestApp.Writers
{
    public interface IWriter : IStoppableByForce
    {
        void WriteResult(Func<int, byte[]> consumeResult);
    }
}
