using System;
using System.IO;

namespace TeamTest
{
    public class POD : ITest
    {
        public string JobName => "pod";

        public string GenerateTest(int id, string directory)
        {
            var path = Path.Combine(directory, $"{JobName}{id}.in");
            var r = new Random(id);
            long all = 0;
            var B = r.Next(2, 1000001);
            var q = 100000;
            var n = new int[B];
            for (var i = 0; i < B; i++)
            {
                all += n[i] = r.Next(1, 1000001);
            }
            using (var sw = new StreamWriter(path, false))
            {
                sw.WriteLine($"{B} {q}");
                for (var i = 0; i < B; i++)
                {
                    sw.Write($"{n[i]} ");
                }
                sw.WriteLine();
                for (var i = 0; i < q; i++)
                {
                    sw.WriteLine(r.NextLong(0, all + 100000L));
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