using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TeamTest.Adapters;

namespace TeamTest.OI2017
{
    public class PIO : ITest
    {
        public readonly IValue<int> N;
        public readonly IValue<int> Vx;
        public readonly IValue<int> Vy;

        /// <summary>
        /// Pionek - OI 2017 Round 1
        /// </summary>
        /// <param name="x">Vector X</param>
        /// <param name="y">Vector Y</param>
        /// <param name="n">Vector count</param>
        public PIO(IValue<int> x, IValue<int> y, IValue<int> n)
        {
            N = n;
            Vx = x;
            Vy = y;
        }

        public string JobName => "pio";

        public string GenerateTest(int id, string directory)
        {
            var path = Path.Combine(directory, $"{JobName}{id}.in");
            var r = new Random(id);
            using (var sw = new StreamWriter(path, false))
            {
                int n = N.Get(r);
                sw.WriteLine($"{n}");
                for (int i = 0; i < n; i++)
                {
                    sw.WriteLine($"{Vx.Get(r)} {Vy.Get(r)}");
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
