using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace TeamTest
{
    public class Block
    {
        public int x, y, z;

        public Block(int x, int y, int z)
        {
            this.x = x;
            this.y = y;
            this.z = z;
        }

        public override string ToString()
        {
            return $"{x} {y} {z}";
        }
    }

    public class FLA : ITest
    {
        public string JobName => "fla";

        public string GenerateTest(int id, string directory)
        {
            var r = new Random(id);
            var fileName = Path.Combine(directory, $"{JobName}{id}.in");
            var blocksCount = r.Next(0, 301);
            var blocks = new List<Block>(blocksCount);
            var random = RNG.GenerateRandom(blocksCount, 1, 1001, r);
            random.Sort();
            int x, y, z;
            for (var i = 0; i < random.Count; i++)
            {
                x = random[i];
                y = r.Next(-100, 20);
                z = r.Next(y + 1, 100);
                blocks.Add(new Block(x, y, z));
            }
            var X = random.Count > 0 ? random.Last() + 1 : r.Next(0, 1000000000);
            using (var sw = new StreamWriter(fileName, false))
            {
                sw.WriteLine($"{blocksCount} {X}");
                for (var i = 0; i < blocksCount; i++)
                {
                    sw.WriteLine(blocks[i].ToString());
                }
            }
            return fileName;
        }

        public bool HasNext(int nextId)
        {
            return true;
        }
    }
}