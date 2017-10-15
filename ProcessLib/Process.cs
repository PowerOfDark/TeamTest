//#define _DEBUG_MSG_
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Win32Lib;

namespace ProcessLib
{
    public class Process
    {

        private static Action<object> _timeout = (p) =>
        {
            var t = p as Process;
            Thread.Sleep((int)t.StartInfo.TotalTimeLimit);
            if (t.IsAlive)
            {
                try
                {
                    t.Kill(256);
                    t.OutOfTime = true;
                }
                catch { }

            }
        };

        public delegate void ProcessEvent(Process process);

        public event ProcessEvent OnStarted;
        public event ProcessEvent OnExited;

        public bool OutOfMemory { get; private set; }
        public bool OutOfTime { get; private set; }
        public bool HasExited { get; private set; }
        public uint? ExitCode
        {
            get
            {
                if (!HasExited) return null;
                uint code = 0;
                Win32.GetExitCodeProcess(this._hProcess, out code);
                return code;
            }
        }
        public DateTime DateStarted { get; private set; }
        public DateTime DateExited { get; private set; }
        public TimeSpan Clock
        {
            get
            {
                if (DateExited == null)
                    return DateTime.Now - DateStarted;
                return DateExited - DateStarted;
            }
        }
        public bool IsAlive
        {
            get
            {
                return (!HasExited && _hProcess != IntPtr.Zero && listener != null && listener.IsAlive);
            }
        }
        public UInt64 ProcessCPUCycles
        {
            get
            {
                if (_hProcess == IntPtr.Zero) return 0;
                UInt64 tmp;
                Win32.QueryProcessCycleTime(_hProcess, out tmp);
                return tmp;
            }
        }
        public UInt64 MainThreadCPUCycles
        {
            get
            {
                if (_hThread == IntPtr.Zero) return 0;
                UInt64 tmp;
                Win32.QueryThreadCycleTime(_hThread, out tmp);
                return tmp;
            }
        }
        /// <summary>
        /// Time in milliseconds the process is running based on MainThreadCPUCycles divided by QueryPerformanceFrequency
        /// </summary>
        public UInt64 FixedProcessTime
        {
            get
            {
                if (this._QPF == 0)
                    return 0;
                return this.MainThreadCPUCycles / this._QPF;
            }
        }
        public IntPtr ProcessHandle { get { return _hProcess; } }
        public IntPtr ThreadHandle { get { return _hThread; } }

        public ProcessStartInfo StartInfo { get; private set; }

        private IntPtr _hProcess, _hJob, _hIOCP, _hThread;

        private Thread listener = null;

        /// <summary>
        /// QueryPerformanceFrequency() at the moment the process has been started.
        /// </summary>
        private UInt64 _QPF;

        //private UInt64 _startupCycles = 0;

        public Process(ProcessStartInfo psi)
        {
            this.StartInfo = psi;
            this.OnExited += Process_OnExited;
        }

        ~Process()
        {
            this.Close();
        }

        public void Start()
        {
            _hJob = _hIOCP = _hProcess = IntPtr.Zero;
            PROCESS_INFORMATION pi = new PROCESS_INFORMATION();

            try
            {

                Dictionary<string, string> additionalEnv = new Dictionary<string, string>();
                pi = StartProcess(this.StartInfo.Arguments, additionalEnv);
                _hProcess = pi.hProcess;
                this._hThread = pi.hThread;

                var securityAttributes = new SECURITY_ATTRIBUTES();
                securityAttributes.bInheritHandle = true;
                securityAttributes.nLength = (uint)Marshal.SizeOf(securityAttributes);

                _hJob = Win32.CheckResult(Win32.CreateJobObject(ref securityAttributes, "procgov-" + Guid.NewGuid()));

                // create completion port
                _hIOCP = Win32.CheckResult(Win32.CreateIoCompletionPort(Win32.INVALID_HANDLE_VALUE, IntPtr.Zero, IntPtr.Zero, 1));
                var assocInfo = new JOBOBJECT_ASSOCIATE_COMPLETION_PORT
                {
                    CompletionKey = IntPtr.Zero,
                    CompletionPort = _hIOCP
                };
                uint size = (uint)Marshal.SizeOf(assocInfo);
                Win32.CheckResult(Win32.SetInformationJobObject(_hJob, JOBOBJECTINFOCLASS.AssociateCompletionPortInformation,
                        ref assocInfo, size));

                // start listening thread
                listener = new Thread(CompletionPortListener);
                listener.Start(_hIOCP);

                if (StartInfo.MemoryLimit > 0 || StartInfo.CPUTimeLimit > 0)
                {
                    var limitInfo = new JOBOBJECT_EXTENDED_LIMIT_INFORMATION
                    {
                        BasicLimitInformation = new JOBOBJECT_BASIC_LIMIT_INFORMATION
                        {

                        },
                    };
                    JobInformationLimitFlags flags = JobInformationLimitFlags.JOB_OBJECT_LIMIT_BREAKAWAY_OK
                                                     | JobInformationLimitFlags.JOB_OBJECT_LIMIT_SILENT_BREAKAWAY_OK;
                    if (StartInfo.MemoryLimit > 0)
                    {
                        flags |= JobInformationLimitFlags.JOB_OBJECT_LIMIT_PROCESS_MEMORY;
                        limitInfo.ProcessMemoryLimit = (UIntPtr)StartInfo.MemoryLimit;
                    }
                    if (StartInfo.CPUTimeLimit > 0)
                    {
                        flags |= JobInformationLimitFlags.JOB_OBJECT_LIMIT_PROCESS_TIME;
                        limitInfo.BasicLimitInformation.PerProcessUserTimeLimit = StartInfo.CPUTimeLimit * (10000);
                    }

                    limitInfo.BasicLimitInformation.LimitFlags = flags;
                    // configure constraints

                    size = (uint)Marshal.SizeOf(limitInfo);
                    Win32.CheckResult(Win32.SetInformationJobObject(_hJob, JOBOBJECTINFOCLASS.ExtendedLimitInformation,
                            ref limitInfo, size));
                }

                Win32.QueryPerformanceFrequency(out this._QPF);

                // assign a process to a job to apply constraints
                Win32.CheckResult(Win32.AssignProcessToJobObject(_hJob, _hProcess));
                if((uint)this.StartInfo.Affinity > 0)
                {
                    Win32.SetProcessAffinityMask(this._hProcess, this.StartInfo.Affinity);
                }
                
                Win32.SetHandleInformation(this._hThread, HANDLE_FLAGS.INHERIT, HANDLE_FLAGS.INHERIT);
                Win32.SetHandleInformation(this._hProcess, HANDLE_FLAGS.INHERIT, HANDLE_FLAGS.INHERIT);
                if (!StartInfo.StartSuspended)
                {
                    //Thread.Sleep(10);
                    bool res;
                    do
                    { res = StartResume(); if (!res) Console.WriteLine("??"); } while (!res);
                }

                //if (Win32.WaitForSingleObject(_hProcess, Win32.INFINITE) == 0xFFFFFFFF)
                //{
                //    throw new Win32Exception();
                //}
            }
            finally
            {
            }

        }
        /// <summary>
        /// 
        /// </summary>
        /// <returns>Whether the main thread has been resumed</returns>
        public bool StartResume()
        {
            bool resumed = false;
            try
            {
                Win32.CheckResult(Win32.ResumeThread(this._hThread));
                resumed = true;
            }
            catch { }
            this.DateStarted = DateTime.Now;
            if (StartInfo.TotalTimeLimit > 0)
                Task.Factory.StartNew(_timeout, this);
            return resumed;
        }

        public bool WaitForExit(uint milliseconds = Win32.INFINITE)
        {
            return Win32.WaitForSingleObject(this._hProcess, milliseconds) != 0xFFFFFFFF;//non-failed
        }

        public bool Kill(uint code = 1337)
        {
            return Win32.CheckResult(Win32.TerminateProcess(this._hProcess, code));
        }

        public void Close()
        {
            Win32.TryCloseHandle(ref this._hIOCP);
            Win32.TryCloseHandle(ref this._hThread);
            Win32.TryCloseHandle(ref this._hProcess);
            Win32.TryCloseHandle(ref this._hJob);
        }

        public Win32.PROCESS_MEMORY_COUNTERS? GetMemoryCounters()
        {
            if (this._hProcess == IntPtr.Zero)
                return null;
            Win32.PROCESS_MEMORY_COUNTERS counters;
            Win32.GetProcessMemoryInfo(this._hProcess, out counters, Marshal.SizeOf(typeof(Win32.PROCESS_MEMORY_COUNTERS)));
            return counters;
        }

        private void Process_OnExited(Process p)
        {
            HasExited = true;
            DateExited = DateTime.Now;
            Win32.TryCloseHandle(ref _hIOCP);
            //if(this.StartInfo.StdIO.StandardInput.IsValid
            //{
            //    ANON_PIPE pipe = (this.StartInfo.StdIO.StandardInput as ANON_PIPE);
            //    pipe.CloseRead(); //the pipe was read
            //}
            //else this.StartInfo.StdIO.StandardInput.Close();
            //if (this.StartInfo.StdIO.StandardOutput is ANON_PIPE)
            //{
            //    ANON_PIPE pipe = (this.StartInfo.StdIO.StandardOutput as ANON_PIPE);
            //    pipe.CloseWrite(); //the pipe was written
            //}
            //else this.StartInfo.StdIO.StandardOutput.Close();
            if (this.StartInfo.StdIO.StandardInput.ReadHandle != Win32.INVALID_HANDLE_VALUE)
                this.StartInfo.StdIO.StandardInput.CloseRead();
            if (this.StartInfo.StdIO.StandardOutput.WriteHandle != Win32.INVALID_HANDLE_VALUE)
                this.StartInfo.StdIO.StandardOutput.CloseWrite();
            //this.StartInfo.StdIO.StandardInput.Close();
            //this.StartInfo.StdIO.StandardOutput.Close();
            // wait for the listener thread to finish
            //if (listener != null && listener.IsAlive)
            //    listener.Join();
        }


        PROCESS_INFORMATION StartProcess(string procargs, Dictionary<string, string> additionalEnv)
        {
            PROCESS_INFORMATION pi;
            STARTUPINFO si = new STARTUPINFO();
            StringBuilder envEntries = new StringBuilder();
            si.hStdError = IntPtr.Zero;//Win32.GetStdHandle(STD_ERROR_HANDLE);
            si.hStdInput = IntPtr.Zero;//Win32.GetStdHandle(STD_INPUT_HANDLE);
            si.hStdOutput = IntPtr.Zero;//Win32.GetStdHandle(STD_OUTPUT_HANDLE);
            
            foreach (var env in Environment.GetEnvironmentVariables().Keys)
            {
                if (additionalEnv.ContainsKey((string)env)) continue; // overwrite existing env
                envEntries.Append(env);
                envEntries.Append("=");
                envEntries.Append(Environment.GetEnvironmentVariable((string)env));
                envEntries.Append("\0");
            }

            foreach (string env in additionalEnv.Keys)
            {
                envEntries.Append(env);
                envEntries.Append("=");
                envEntries.Append(additionalEnv[env]);
                envEntries.Append("\0");
            }

            if (envEntries.Length < 1) envEntries.Append("\0");
            envEntries.Append("\0");

            CreateProcessFlags flags = 0;//CreateProcessFlags.CREATE_NEW_CONSOLE;

            //if (this.StartInfo.StartSuspended || !this.StartInfo.RapidMode)
            //{
                flags |= CreateProcessFlags.CREATE_SUSPENDED;
            //}
            IntPtr input = this.StartInfo.StdIO.StandardInput.OpenRead();
            IntPtr output = this.StartInfo.StdIO.StandardOutput.OpenWrite();
            if (input != Win32.INVALID_HANDLE_VALUE)
                si.hStdInput = input;
            if (output != Win32.INVALID_HANDLE_VALUE)
                si.hStdOutput = output;

            if (input != Win32.INVALID_HANDLE_VALUE || output != Win32.INVALID_HANDLE_VALUE)
                si.dwFlags = 0x00000100; //HANDLES

            Win32.CheckResult(Win32.CreateProcess(this.StartInfo.Executable, procargs, IntPtr.Zero, IntPtr.Zero, true,
                        flags,
                        envEntries, StartInfo.WorkingDirectory, ref si, out pi));
            return pi;
        }

        void CompletionPortListener(object o)
        {
            var hIOCP = (IntPtr)o;
            uint msgIdentifier;
            IntPtr pCompletionKey, lpOverlapped;

            while (Win32.GetQueuedCompletionStatus(hIOCP, out msgIdentifier, out pCompletionKey,
                        out lpOverlapped, Win32.INFINITE))
            {
                if (msgIdentifier == (uint)JobMsgInfoMessages.JOB_OBJECT_MSG_NEW_PROCESS)
                {
#if _DEBUG_MSG_
                    Console.WriteLine("{0}: process {1} has started", msgIdentifier, (int)lpOverlapped);
#endif
                    this.OnStarted?.Invoke(this);
                }
                else if (msgIdentifier == (uint)JobMsgInfoMessages.JOB_OBJECT_MSG_EXIT_PROCESS)
                {
#if _DEBUG_MSG_
                    Console.WriteLine("{0}: process {1} exited", msgIdentifier, (int)lpOverlapped);
#endif
                    this.OnExited?.Invoke(this);
                }
                else if (msgIdentifier == (uint)JobMsgInfoMessages.JOB_OBJECT_MSG_ACTIVE_PROCESS_ZERO)
                {
                    // nothing
                }
                else if (msgIdentifier == (uint)JobMsgInfoMessages.JOB_OBJECT_MSG_PROCESS_MEMORY_LIMIT)
                {
#if _DEBUG_MSG_
                    Console.WriteLine("{0}: process {1} exceeded its memory limit", msgIdentifier, (int)lpOverlapped);
#endif
                    this.OutOfMemory = true;
                    this.OnExited?.Invoke(this);
                }
                else if (msgIdentifier == (uint)JobMsgInfoMessages.JOB_OBJECT_MSG_END_OF_PROCESS_TIME)
                {
#if _DEBUG_MSG_
                    Console.WriteLine("{0}: process {1} exceeded its time limit", msgIdentifier, (int)lpOverlapped);
#endif
                    this.OutOfTime = true;
                    this.OnExited?.Invoke(this);
                }
                else
                {
#if _DEBUG_MSG_
                    Console.WriteLine("Unknown message: {0}", msgIdentifier);
#endif
                }
            }
        }

    }
}
