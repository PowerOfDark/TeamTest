using System;
using System.IO;
using TeamTest.Adapters;

namespace TeamTest.SKI2017
{
    public class STE : ITest
    {
        public readonly IValue<int> AddX;
        public readonly IValue<int> AddY;
        public readonly IValue<int> Chance;

        public readonly IValue<int> X;
        public readonly IValue<int> Y;

        /// <summary>
        /// Stempel - SKI 2017 Round 2
        /// </summary>
        /// <param name="x">Pattern width</param>
        /// <param name="y">Pattern height</param>
        /// <param name="addX">Additional width added to <paramref name="x"/></param>
        /// <param name="addY">Additional height added to <paramref name="y"/></param>
        /// <param name="chance">1/x chance of each block being '#'</param>
        public STE(IValue<int> x, IValue<int> y, IValue<int> addX, IValue<int> addY, IValue<int> chance)
        {
            X = x;
            Y = y;
            AddX = addX;
            AddY = addY;
            Chance = chance;
        }

        public string JobName => "ste";

        public string GenerateTest(int id, string directory)
        {
            var path = Path.Combine(directory, $"{JobName}{id}.in");
            var r = new Random(id);
            using (var sw = new StreamWriter(path, false))
            {
                var x = X.Get(r);
                var y = Y.Get(r);
                var px = 2 * x + AddX.Get(r);
                var py = 2 * y + AddY.Get(r);

                sw.WriteLine($"{px} {py}");
                sw.WriteLine($"{x} {y}");

                var chance = Chance.Get(r);

                for (var i = 0; i < x; i++)
                {
                    for (var j = 0; j < y; j++)
                    {
                        sw.Write(r.Next(0, chance) == 0 ? '#' : '.');
                    }
                    sw.WriteLine();
                }
            }
            return path;
        }

        public bool HasNext(int nextId)
        {
            return true;
        }
    }
}