using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TeamTest.Adapters;

namespace TeamTest.SKI2017
{
    public class KOL : ITest
    {
        public readonly IValue<int> N;
        public readonly IValue<int> M;

        /// <summary>
        /// Kolory tej jesieni - SKI 2017 Round 3
        /// </summary>
        /// <param name="n">Women count</param>
        /// <param name="m">John's friends count</param>

        public KOL(IValue<int> n, IValue<int> m)
        {
            N = n;
            M = m;
        }

        public string JobName => "kol";

        public string GenerateTest(int id, string directory)
        {
            var path = Path.Combine(directory, $"{JobName}{id}.in");
            using (var sw = new StreamWriter(path, false))
            {
                var r = new Random(id);
                var n = N.Get(r);
                var m = M.Get(r);

                sw.WriteLine($"{n} {m}");
                for (int i = 0; i < n; i++)
                {
                    sw.Write($"{(char)(r.Next('a', 'z' + 1))}");
                }
                sw.WriteLine("");
                for (int i = 0; i < m; i++)
                {
                    var b = r.Next(1, n + 1);
                    var a = r.Next(1, b + 1);
                    sw.WriteLine($"{a} {b}");
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
