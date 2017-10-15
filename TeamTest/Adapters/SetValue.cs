using System;
using System.Collections.Generic;

namespace TeamTest.Adapters
{
    public class SetValue<T> : IValue<T>
    {
        public readonly List<T> Values = new List<T>();

        public SetValue(IEnumerable<T> values)
        {
            Values.AddRange(values);
        }

        public T Get(Random r)
        {
            return Values[r.Next(0, Values.Count)];
        }
    }
}