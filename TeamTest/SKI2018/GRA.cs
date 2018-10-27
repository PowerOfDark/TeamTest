using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TeamTest.Adapters;

namespace TeamTest.SKI2018
{
    public class GRA : ITest
    {
        public readonly IValue<ulong> K;
        public readonly IValue<uint> N;
        /// <summary>
        /// Gra w kosci - SKI 2018 Round 0
        /// </summary>
        /// <param name="K">Min dots sum</param>
        /// <param name="N">Walls/dots count</param>

        public GRA(IValue<ulong> k, IValue<uint> n)
        {
            N = n;
            K = k;
        }

        public string JobName => "gra";

        public string GenerateTest(int id, string directory)
        {
            var path = Path.Combine(directory, $"{JobName}{id}.in");
            using (var sw = new StreamWriter(path, false))
            {
                var r = new Random(id);
                var k = K.Get(r);
                var n = N.Get(r);

                sw.WriteLine($"{n} {k}");
            }
            return path;
        }

        public bool HasNext(int nextId)
        {
            return true;
        }
    }
}
