using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Win32Lib;

namespace ProcessLib
{
    public class ProcessAffinity
    {

        public static uint GetNumberOfCPUs()
        {
            SYSTEM_INFO si;
            Win32.GetSystemInfo(out si);
            return si.numberOfProcessors;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="cpuToRunOn">List of 0-indexed cpu cores</param>
        public static UIntPtr GetAffinityMask(IEnumerable<int> cpuToRunOn)
        {
            var toRunOn = cpuToRunOn as IList<int> ?? cpuToRunOn.ToList();
            bool[] b = new bool[toRunOn.Max() + 1];
            foreach (int i in toRunOn)
                b[i] = true;
            return GetAffinityMask(b);
        }

        public static UIntPtr GetAffinityMask(bool[] cpuToRunOn)
        {
            uint mask = 0;
            for (int i = 0; i < cpuToRunOn.Length; i++)
            {
                if (cpuToRunOn[i])
                    mask |= (1u << i);
            }

            return (UIntPtr)mask;
        }
    }
}
