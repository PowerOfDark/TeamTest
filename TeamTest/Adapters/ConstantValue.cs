using System;

namespace TeamTest.Adapters
{
    public class ConstantValue<T> : IValue<T>
    {
        public ConstantValue(T value)
        {
            Value = value;
        }

        public T Value { get; protected set; }

        public T Get(Random r)
        {
            return Value;
        }
    }
}