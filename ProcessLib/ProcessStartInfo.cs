using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProcessLib
{
    public class ProcessStartInfo
    {
        /// <summary>
        /// The max amount of RAM (specified in bytes) the process is allowed to use
        /// </summary>
        public uint MemoryLimit { get; set; }
        /// <summary>
        /// The max amount of CPU-time in milliseconds the process is allowed to use
        /// </summary>
        public long CPUTimeLimit { get; set; }
        /// <summary>
        /// The max TOTAL wall-clock time in milliseconds the process is allowed to run.
        /// </summary>
        public long TotalTimeLimit { get; set; }

        public string Executable { get; set; }
        public string Arguments { get; set; }
        public string WorkingDirectory { get; set; }

        public ProcessIO StdIO { get; set; }
        
        public UIntPtr Affinity { get; set; }

        public bool StartSuspended { get; set; }

        public ProcessStartInfo()
        {
            this.MemoryLimit = 0;
            this.CPUTimeLimit = 0;
            this.TotalTimeLimit = 0;
            this.WorkingDirectory = Environment.CurrentDirectory;
            this.StartSuspended = false;
            this.StdIO = new ProcessIO();
        }
    }
}
