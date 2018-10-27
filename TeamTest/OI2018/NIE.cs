using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TeamTest.Adapters;

namespace TeamTest.OI2018
{
    public class NIE : ITest
    {
        public readonly IValue<int> N;
        public readonly IValue<int> M;

        /// <summary>
        /// Niedbalosc - OI 2018 Round 1
        /// </summary>
        /// <param name="n">First genotype length</param>
        /// <param name="m">Second genotype length</param>

        public NIE(IValue<int> n, IValue<int> m)
        {
            N = n;
            M = m;
        }

        public string JobName => "nie";

        public string GenerateTest(int id, string directory)
        {
            var path = Path.Combine(directory, $"{JobName}{id}.in");
            var r = new Random(id);
            using (var sw = new StreamWriter(path, false))
            {
                var n = N.Get(r);
                var m = M.Get(r);
                const string chars = "ATGC";
                sw.WriteLine(Enumerable.Repeat(chars, n).Select(s => s[r.Next(s.Length)]).ToArray());
                sw.WriteLine(Enumerable.Repeat(chars, m).Select(s => s[r.Next(s.Length)]).ToArray());
            }
            return path;
        }

        public bool HasNext(int nextId)
        {
            return true;
        }
    }
}
