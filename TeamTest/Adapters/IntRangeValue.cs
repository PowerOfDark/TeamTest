using System;

namespace TeamTest.Adapters
{
    public class IntRangeValue : IValue<int>
    {
        public IntRangeValue(int low, int high)
        {
            Low = low;
            High = high;
        }

        public int Low { get; protected set; }
        public int High { get; protected set; }

        public int Get(Random r)
        {
            return r.Next(Low, High);
        }
    }
}