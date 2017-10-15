using System.IO;

namespace TeamTest
{
    public class REP : ITest
    {
        public string JobName => "rep";

        public string GenerateTest(int id, string directory)
        {
            var path = Path.Combine(directory, $"{JobName}{id}.in");
            using (var sw = new StreamWriter(path, false))
            {
                sw.WriteLine(100000);
                for (var i = id * 100000 + 1; i <= (id + 1) * 100000; i++)
                {
                    sw.WriteLine(i);
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