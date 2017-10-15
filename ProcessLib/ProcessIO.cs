using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Win32Lib;

namespace ProcessLib
{
    public class ProcessIO
    {
        public Win32.HANDLE StandardInput { get; private set; }
        public Win32.HANDLE StandardOutput { get; private set; }

        public ProcessIO() : this(new Win32.INVALID_HANDLE(), new Win32.INVALID_HANDLE())
        {

        }

        public ProcessIO(Win32.HANDLE stdIn) : this(stdIn, new Win32.INVALID_HANDLE())
        {
            
        }

        public ProcessIO(Win32.HANDLE stdIn, Win32.HANDLE stdOut)
        {
            this.StandardInput = stdIn;
            this.StandardOutput = stdOut;
        }
    }
}
