using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TeamTest.Adapters;

namespace TeamTest.OI2017
{
    public class WIC : ITest
    {
        public readonly IValue<int> N;

        /// <summary>
        /// Przecietny wicemaks - WIC 2017 Round 2
        /// </summary>
        /// <param name="n">Vector count</param>
        public WIC(IValue<int> n)
        {
            N = n;
        }

        public string JobName => "wic";

        public string GenerateTest(int id, string directory)
        {
            var path = Path.Combine(directory, $"{JobName}{id}.in");
            using (var sw = new StreamWriter(path, false))
            {
                var r = new Random(id);
                var n = N.Get(r);
                sw.WriteLine($"{n}");
                for (int i = 0; i < n; i++)
                {
                    sw.Write("{0} ",r.Next(0,2000000001)-1000000000);
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
