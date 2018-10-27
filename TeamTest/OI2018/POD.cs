using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TeamTest.Adapters;

namespace TeamTest.OI2018
{
    public class POD : ITest
    {
        public readonly IValue<long> N;
        public readonly IValue<int> Q;

        /// <summary>
        /// Podciagi - OI 2018 Round 1
        /// </summary>
        /// <param name="n">Max n value</param>
        /// <param name="q">Number of test cases</param>

        public POD(IValue<long> n, IValue<int> q)
        {
            N = n;
            Q = q;
        }

        public string JobName => "pod";

        public string GenerateTest(int id, string directory)
        {
            var path = Path.Combine(directory, $"{JobName}{id}.in");
            var r = new Random(id);
            using (var sw = new StreamWriter(path, false))
            {
                var q = Q.Get(r);
                for(int i = 0; i < q; i++)
                {
                    sw.WriteLine($"{N.Get(r)}");
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
