using System;

namespace TeamTest.Adapters
{
    public interface IValue<out T>
    {
        T Get(Random r);
    }
}