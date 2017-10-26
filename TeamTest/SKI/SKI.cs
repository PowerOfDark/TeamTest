using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TeamTest.Adapters;

namespace TeamTest.OI2017
{
    public class SKI : ITest
    {
        public readonly IValue<int> N;

        /// <summary>
        /// Spontaniczne slowo - SKI 2017 Round 0
        /// </summary>
        /// <param name="n">Vector count</param>
        public SKI(IValue<int> n)
        {
            N = n;
        }

        public string JobName => "ski";

        public string GenerateTest(int id, string directory)
        {
            var path = Path.Combine(directory, $"{JobName}{id}.in");
            using (var sw = new StreamWriter(path, false))
            {
                var r = new Random(id);
                var n = N.Get(r);
                for (int i = 0; i < n; i++)
                {
                    sw.Write($"{(char)(r.Next('A', 'Z' + 1))}");
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
