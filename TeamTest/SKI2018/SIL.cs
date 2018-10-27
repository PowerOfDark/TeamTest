using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TeamTest.Adapters;

namespace TeamTest.SKI2018
{
    public class SIL : ITest
    {
        public readonly IValue<ulong> A;
        public readonly IValue<ulong> B;
        public readonly IValue<uint> C;
        /// <summary>
        /// Iloraz silni - SKI 2018 Round 1
        /// </summary>
        /// <param name="A">a</param>
        /// <param name="B">b</param>
        /// <param name="C">c</param>

        public SIL(IValue<ulong> a, IValue<ulong> b, IValue<uint> c)
        {
            A = a;
            B = b;
            C = c;
        }

        public string JobName => "sil";

        public string GenerateTest(int id, string directory)
        {
            var path = Path.Combine(directory, $"{JobName}{id}.in");
            using (var sw = new StreamWriter(path, false))
            {
                var r = new Random(id);
                var a = A.Get(r);
                var b = B.Get(r);
                var c = C.Get(r);

                sw.WriteLine($"{a} {b} {c}");
            }
            return path;
        }

        public bool HasNext(int nextId)
        {
            return true;
        }
    }
}
