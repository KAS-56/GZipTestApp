using System;

namespace GZipTestApp.Readers
{
    public interface IReader : IStoppableByForce
    {
        event EventHandler DataIsOver;
        void ReadSource(Action<int, byte[]> produceSource);
    }
}
