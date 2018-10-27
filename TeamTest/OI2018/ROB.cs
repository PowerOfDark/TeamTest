using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TeamTest.Adapters;

namespace TeamTest.OI2018
{
    public class ROB : ITest
    {
        public readonly IValue<uint> N;
        public readonly IValue<ulong> T;
        public readonly IValue<int> X;
        public readonly IValue<int> Y;
        public readonly IValue<uint> D;

        /// <summary>
        /// Robocik - OI 2018 Round 1
        /// </summary>
        /// <param name="n">Program length</param>
        /// <param name="t">Battery life</param>
        /// <param name="x">X</param>
        /// <param name="y">Y</param>
        /// <param name="d">Max d</param>

        public ROB(IValue<uint> n, IValue<ulong> t, IValue<int> x, IValue<int> y, IValue<uint> d)
        {
            N = n;
            T = t;
            X = x;
            Y = y;
            D = d;
        }

        public string JobName => "rob";

        public string GenerateTest(int id, string directory)
        {
            var path = Path.Combine(directory, $"{JobName}{id}.in");
            var r = new Random(id);
            using (var sw = new StreamWriter(path, false))
            {
                var n = N.Get(r);
                var t = T.Get(r);

                sw.WriteLine($"{n} {t}");
                for (int i = 0; i < n; i++)
                {
                    sw.Write($"{D.Get(r)} ");
                }
                sw.WriteLine();
                sw.WriteLine($"{X.Get(r)} {Y.Get(r)}");
            }
            return path;
        }

        public bool HasNext(int nextId)
        {
            return true;
        }
    }
}
