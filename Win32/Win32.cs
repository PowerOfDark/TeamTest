using System;
using System.ComponentModel;
using System.Runtime.InteropServices;
using System.Text;
using DWORD = System.UInt32;

namespace Win32Lib
{
    [StructLayout(LayoutKind.Sequential)]
    public struct SYSTEM_INFO
    {
        public ushort processorArchitecture;
        public ushort reserved;
        public uint pageSize;
        public IntPtr minimumApplicationAddress;
        public IntPtr maximumApplicationAddress;
        public IntPtr activeProcessorMask;
        public uint numberOfProcessors;
        public uint processorType;
        public uint allocationGranularity;
        public ushort processorLevel;
        public ushort processorRevision;
    }


    [Flags]
    public enum HANDLE_FLAGS : uint
    {
        None = 0,
        INHERIT = 1,
        PROTECT_FROM_CLOSE = 2
    }

    [Flags]
    public enum EFileAccess : uint
    {
        //
        // Standart Section
        //

        AccessSystemSecurity = 0x1000000, // AccessSystemAcl access type
        MaximumAllowed = 0x2000000, // MaximumAllowed access type

        Delete = 0x10000,
        ReadControl = 0x20000,
        WriteDAC = 0x40000,
        WriteOwner = 0x80000,
        Synchronize = 0x100000,

        StandardRightsRequired = 0xF0000,
        StandardRightsRead = ReadControl,
        StandardRightsWrite = ReadControl,
        StandardRightsExecute = ReadControl,
        StandardRightsAll = 0x1F0000,
        SpecificRightsAll = 0xFFFF,

        FILE_READ_DATA = 0x0001, // file & pipe
        FILE_LIST_DIRECTORY = 0x0001, // directory
        FILE_WRITE_DATA = 0x0002, // file & pipe
        FILE_ADD_FILE = 0x0002, // directory
        FILE_APPEND_DATA = 0x0004, // file
        FILE_ADD_SUBDIRECTORY = 0x0004, // directory
        FILE_CREATE_PIPE_INSTANCE = 0x0004, // named pipe
        FILE_READ_EA = 0x0008, // file & directory
        FILE_WRITE_EA = 0x0010, // file & directory
        FILE_EXECUTE = 0x0020, // file
        FILE_TRAVERSE = 0x0020, // directory
        FILE_DELETE_CHILD = 0x0040, // directory
        FILE_READ_ATTRIBUTES = 0x0080, // all
        FILE_WRITE_ATTRIBUTES = 0x0100, // all

        //
        // Generic Section
        //

        GenericRead = 0x80000000,
        GenericWrite = 0x40000000,
        GenericExecute = 0x20000000,
        GenericAll = 0x10000000,

        SPECIFIC_RIGHTS_ALL = 0x00FFFF,

        FILE_ALL_ACCESS =
            StandardRightsRequired |
            Synchronize |
            0x1FF,

        FILE_GENERIC_READ =
            StandardRightsRead |
            FILE_READ_DATA |
            FILE_READ_ATTRIBUTES |
            FILE_READ_EA |
            Synchronize,

        FILE_GENERIC_WRITE =
            StandardRightsWrite |
            FILE_WRITE_DATA |
            FILE_WRITE_ATTRIBUTES |
            FILE_WRITE_EA |
            FILE_APPEND_DATA |
            Synchronize,

        FILE_GENERIC_EXECUTE =
            StandardRightsExecute |
            FILE_READ_ATTRIBUTES |
            FILE_EXECUTE |
            Synchronize
    }

    [Flags]
    public enum EFileShare : uint
    {
        /// <summary>
        /// </summary>
        None = 0x00000000,

        /// <summary>
        ///     Enables subsequent open operations on an object to request read access.
        ///     Otherwise, other processes cannot open the object if they request read access.
        ///     If this flag is not specified, but the object has been opened for read access, the function fails.
        /// </summary>
        Read = 0x00000001,

        /// <summary>
        ///     Enables subsequent open operations on an object to request write access.
        ///     Otherwise, other processes cannot open the object if they request write access.
        ///     If this flag is not specified, but the object has been opened for write access, the function fails.
        /// </summary>
        Write = 0x00000002,

        /// <summary>
        ///     Enables subsequent open operations on an object to request delete access.
        ///     Otherwise, other processes cannot open the object if they request delete access.
        ///     If this flag is not specified, but the object has been opened for delete access, the function fails.
        /// </summary>
        Delete = 0x00000004
    }

    public enum ECreationDisposition : uint
    {
        /// <summary>
        ///     Creates a new file. The function fails if a specified file exists.
        /// </summary>
        New = 1,

        /// <summary>
        ///     Creates a new file, always.
        ///     If a file exists, the function overwrites the file, clears the existing attributes, combines the specified file
        ///     attributes,
        ///     and flags with FILE_ATTRIBUTE_ARCHIVE, but does not set the security descriptor that the SECURITY_ATTRIBUTES
        ///     structure specifies.
        /// </summary>
        CreateAlways = 2,

        /// <summary>
        ///     Opens a file. The function fails if the file does not exist.
        /// </summary>
        OpenExisting = 3,

        /// <summary>
        ///     Opens a file, always.
        ///     If a file does not exist, the function creates a file as if dwCreationDisposition is CREATE_NEW.
        /// </summary>
        OpenAlways = 4,

        /// <summary>
        ///     Opens a file and truncates it so that its size is 0 (zero) bytes. The function fails if the file does not exist.
        ///     The calling process must open the file with the GENERIC_WRITE access right.
        /// </summary>
        TruncateExisting = 5
    }

    [Flags]
    public enum EFileAttributes : uint
    {
        Readonly = 0x00000001,
        Hidden = 0x00000002,
        System = 0x00000004,
        Directory = 0x00000010,
        Archive = 0x00000020,
        Device = 0x00000040,
        Normal = 0x00000080,
        Temporary = 0x00000100,
        SparseFile = 0x00000200,
        ReparsePoint = 0x00000400,
        Compressed = 0x00000800,
        Offline = 0x00001000,
        NotContentIndexed = 0x00002000,
        Encrypted = 0x00004000,
        Write_Through = 0x80000000,
        Overlapped = 0x40000000,
        NoBuffering = 0x20000000,
        RandomAccess = 0x10000000,
        SequentialScan = 0x08000000,
        DeleteOnClose = 0x04000000,
        BackupSemantics = 0x02000000,
        PosixSemantics = 0x01000000,
        OpenReparsePoint = 0x00200000,
        OpenNoRecall = 0x00100000,
        FirstPipeInstance = 0x00080000
    }


    [Flags]
    public enum OpenFileStyle : uint
    {
        OF_CANCEL = 0x00000800, // Ignored. For a dialog box with a Cancel button, use OF_PROMPT.
        OF_CREATE = 0x00001000, // Creates a new file. If file exists, it is truncated to zero (0) length.
        OF_DELETE = 0x00000200, // Deletes a file.
        OF_EXIST = 0x00004000, // Opens a file and then closes it. Used to test that a file exists
        OF_PARSE = 0x00000100, // Fills the OFSTRUCT structure, but does not do anything else.
        OF_PROMPT = 0x00002000, // Displays a dialog box if a requested file does not exist 
        OF_READ = 0x00000000, // Opens a file for reading only.
        OF_READWRITE = 0x00000002, // Opens a file with read/write permissions.
        OF_REOPEN = 0x00008000, // Opens a file by using information in the reopen buffer.

        // For MS-DOS–based file systems, opens a file with compatibility mode, allows any process on a 
        // specified computer to open the file any number of times.
        // Other efforts to open a file with other sharing modes fail. This flag is mapped to the 
        // FILE_SHARE_READ|FILE_SHARE_WRITE flags of the CreateFile function.
        OF_SHARE_COMPAT = 0x00000000,

        // Opens a file without denying read or write access to other processes.
        // On MS-DOS-based file systems, if the file has been opened in compatibility mode
        // by any other process, the function fails.
        // This flag is mapped to the FILE_SHARE_READ|FILE_SHARE_WRITE flags of the CreateFile function.
        OF_SHARE_DENY_NONE = 0x00000040,

        // Opens a file and denies read access to other processes.
        // On MS-DOS-based file systems, if the file has been opened in compatibility mode,
        // or for read access by any other process, the function fails.
        // This flag is mapped to the FILE_SHARE_WRITE flag of the CreateFile function.
        OF_SHARE_DENY_READ = 0x00000030,

        // Opens a file and denies write access to other processes.
        // On MS-DOS-based file systems, if a file has been opened in compatibility mode,
        // or for write access by any other process, the function fails.
        // This flag is mapped to the FILE_SHARE_READ flag of the CreateFile function.
        OF_SHARE_DENY_WRITE = 0x00000020,

        // Opens a file with exclusive mode, and denies both read/write access to other processes.
        // If a file has been opened in any other mode for read/write access, even by the current process,
        // the function fails.
        OF_SHARE_EXCLUSIVE = 0x00000010,

        // Verifies that the date and time of a file are the same as when it was opened previously.
        // This is useful as an extra check for read-only files.
        OF_VERIFY = 0x00000400,

        // Opens a file for write access only.
        OF_WRITE = 0x00000001
    }

    [Flags]
    public enum CreateProcessFlags
    {
        CREATE_BREAKAWAY_FROM_JOB = 0x01000000,
        CREATE_DEFAULT_ERROR_MODE = 0x04000000,
        CREATE_NEW_CONSOLE = 0x00000010,
        CREATE_NEW_PROCESS_GROUP = 0x00000200,
        CREATE_NO_WINDOW = 0x08000000,
        CREATE_PROTECTED_PROCESS = 0x00040000,
        CREATE_PRESERVE_CODE_AUTHZ_LEVEL = 0x02000000,
        CREATE_SEPARATE_WOW_VDM = 0x00000800,
        CREATE_SHARED_WOW_VDM = 0x00001000,
        CREATE_SUSPENDED = 0x00000004,
        CREATE_UNICODE_ENVIRONMENT = 0x00000400,
        DEBUG_ONLY_THIS_PROCESS = 0x00000002,
        DEBUG_PROCESS = 0x00000001,
        DETACHED_PROCESS = 0x00000008,
        EXTENDED_STARTUPINFO_PRESENT = 0x00080000,
        INHERIT_PARENT_AFFINITY = 0x00010000
    }

    [Flags]
    public enum ProcessAccessFlags
    {
        CreateThread = 0x0002,
        SetSessionId = 0x0004,
        VmOperation = 0x0008,
        VmRead = 0x0010,
        VmWrite = 0x0020,
        DupHandle = 0x0040,
        CreateProcess = 0x0080,
        SetQuota = 0x0100,
        SetInformation = 0x0200,
        QueryInformation = 0x0400,
        SuspendResume = 0x0800,
        QueryLimitedInformation = 0x1000,
        Synchronize = 0x100000,
        Delete = 0x00010000,
        ReadControl = 0x00020000,
        WriteDac = 0x00040000,
        WriteOwner = 0x00080000,
        StandardRightsRequired = 0x000F0000,
        AllAccess = StandardRightsRequired | Synchronize | 0xFFFF
    }

    [Flags]
    public enum JobInformationLimitFlags
    {
        JOB_OBJECT_LIMIT_ACTIVE_PROCESS = 0x00000008,
        JOB_OBJECT_LIMIT_AFFINITY = 0x00000010,
        JOB_OBJECT_LIMIT_BREAKAWAY_OK = 0x00000800,
        JOB_OBJECT_LIMIT_DIE_ON_UNHANDLED_EXCEPTION = 0x00000400,
        JOB_OBJECT_LIMIT_JOB_MEMORY = 0x00000200,
        JOB_OBJECT_LIMIT_JOB_TIME = 0x00000004,
        JOB_OBJECT_LIMIT_KILL_ON_JOB_CLOSE = 0x00002000,
        JOB_OBJECT_LIMIT_PRESERVE_JOB_TIME = 0x00000040,
        JOB_OBJECT_LIMIT_PRIORITY_CLASS = 0x00000020,
        JOB_OBJECT_LIMIT_PROCESS_MEMORY = 0x00000100,
        JOB_OBJECT_LIMIT_PROCESS_TIME = 0x00000002,
        JOB_OBJECT_LIMIT_SCHEDULING_CLASS = 0x00000080,
        JOB_OBJECT_LIMIT_SILENT_BREAKAWAY_OK = 0x00001000,
        JOB_OBJECT_LIMIT_WORKINGSET = 0x00000001
    }

    public enum JobMsgInfoMessages
    {
        JOB_OBJECT_MSG_END_OF_JOB_TIME = 1,
        JOB_OBJECT_MSG_END_OF_PROCESS_TIME = 2,
        JOB_OBJECT_MSG_ACTIVE_PROCESS_LIMIT = 3,
        JOB_OBJECT_MSG_ACTIVE_PROCESS_ZERO = 4,
        JOB_OBJECT_MSG_NEW_PROCESS = 6,
        JOB_OBJECT_MSG_EXIT_PROCESS = 7,
        JOB_OBJECT_MSG_ABNORMAL_EXIT_PROCESS = 8,
        JOB_OBJECT_MSG_PROCESS_MEMORY_LIMIT = 9,
        JOB_OBJECT_MSG_JOB_MEMORY_LIMIT = 10
    }

    [StructLayout(LayoutKind.Sequential)]
    public struct SECURITY_ATTRIBUTES
    {
        public uint nLength;
        public IntPtr lpSecurityDescriptor;
        [MarshalAs(UnmanagedType.Bool)]
        public bool bInheritHandle;
    }

    public enum JOBOBJECTINFOCLASS
    {
        AssociateCompletionPortInformation = 7,
        BasicLimitInformation = 2,
        BasicUIRestrictions = 4,
        EndOfJobTimeInformation = 6,
        ExtendedLimitInformation = 9,
        SecurityLimitInformation = 5,
        GroupInformation = 11
    }

    [StructLayout(LayoutKind.Sequential)]
    public struct IO_COUNTERS
    {
        public ulong ReadOperationCount;
        public ulong WriteOperationCount;
        public ulong OtherOperationCount;
        public ulong ReadTransferCount;
        public ulong WriteTransferCount;
        public ulong OtherTransferCount;
    }

    [StructLayout(LayoutKind.Sequential)]
    public struct JOBOBJECT_BASIC_LIMIT_INFORMATION
    {
        public long PerProcessUserTimeLimit;
        public long PerJobUserTimeLimit;
        public JobInformationLimitFlags LimitFlags;
        public UIntPtr MinimumWorkingSetSize;
        public UIntPtr MaximumWorkingSetSize;
        public uint ActiveProcessLimit;
        public long Affinity;
        public uint PriorityClass;
        public uint SchedulingClass;
    }

    [StructLayout(LayoutKind.Sequential)]
    public struct JOBOBJECT_EXTENDED_LIMIT_INFORMATION
    {
        public JOBOBJECT_BASIC_LIMIT_INFORMATION BasicLimitInformation;
        public IO_COUNTERS IoInfo;
        public UIntPtr ProcessMemoryLimit;
        public UIntPtr JobMemoryLimit;
        public UIntPtr PeakProcessMemoryUsed;
        public UIntPtr PeakJobMemoryUsed;
    }

    [StructLayout(LayoutKind.Sequential)]
    public struct JOBOBJECT_ASSOCIATE_COMPLETION_PORT
    {
        public IntPtr CompletionKey;
        public IntPtr CompletionPort;
    }

    [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Unicode)]
    public struct STARTUPINFO
    {
        public int cb;
        public string lpReserved;
        public string lpDesktop;
        public string lpTitle;
        public int dwX;
        public int dwY;
        public int dwXSize;
        public int dwYSize;
        public int dwXCountChars;
        public int dwYCountChars;
        public int dwFillAttribute;
        public int dwFlags;
        public short wShowWindow;
        public short cbReserved2;
        public IntPtr lpReserved2;
        public IntPtr hStdInput;
        public IntPtr hStdOutput;
        public IntPtr hStdError;
    }

    [StructLayout(LayoutKind.Sequential)]
    public struct PROCESS_INFORMATION
    {
        public IntPtr hProcess;
        public IntPtr hThread;
        public int dwProcessId;
        public int dwThreadId;
    }

    [StructLayout(LayoutKind.Sequential)]
    public struct OFSTRUCT
    {
        public byte cBytes;
        public byte fFixedDisc;
        public ushort nErrCode;
        public ushort Reserved1;
        public ushort Reserved2;

        [MarshalAs(UnmanagedType.ByValTStr,
             SizeConst = 128)]
        public string szPathName;
    }

    public static class Win32
    {
        [Flags]
        public enum DuplicateOptions : uint
        {
            DUPLICATE_CLOSE_SOURCE = 0x00000001,
            // Closes the source handle. This occurs regardless of any error status returned.
            DUPLICATE_SAME_ACCESS = 0x00000002
            //Ignores the dwDesiredAccess parameter. The duplicate handle has the same access as the source handle.
        }

        public const uint INFINITE = 0xFFFFFFFF;

        public const int STD_OUTPUT_HANDLE = -11;
        public const int STD_INPUT_HANDLE = -10;
        public const int STD_ERROR_HANDLE = -12;
        public static readonly IntPtr INVALID_HANDLE_VALUE = new IntPtr(-1);

        [DllImport("kernel32.dll", CharSet = CharSet.Unicode, SetLastError = true)]
        public static extern IntPtr CreateJobObject([In] ref SECURITY_ATTRIBUTES lpJobAttributes, string lpName);

        [DllImport("kernel32.dll", SetLastError = true)]
        public static extern bool TerminateJobObject(IntPtr hJob, uint uExitCode);

        [DllImport("kernel32.dll", SetLastError = true)]
        [return: MarshalAs(UnmanagedType.Bool)]
        public static extern bool AssignProcessToJobObject(IntPtr hJob, IntPtr hProcess);

        [DllImport("kernel32.dll", SetLastError = true)]
        public static extern bool SetInformationJobObject(IntPtr hJob, JOBOBJECTINFOCLASS JobObjectInfoClass,
            ref JOBOBJECT_EXTENDED_LIMIT_INFORMATION lpJobObjectInfo, uint cbJobObjectInfoLength);

        [DllImport("kernel32.dll", SetLastError = true)]
        public static extern bool SetInformationJobObject(IntPtr hJob, JOBOBJECTINFOCLASS JobObjectInfoClass,
            ref JOBOBJECT_ASSOCIATE_COMPLETION_PORT lpJobObjectInfo, uint cbJobObjectInfoLength);

        [DllImport("kernel32.dll", SetLastError = true)]
        [return: MarshalAs(UnmanagedType.Bool)]
        public static extern bool CloseHandle(IntPtr hObject);


        [DllImport("kernel32.dll", SetLastError = true)]
        public static extern IntPtr CreateIoCompletionPort(IntPtr FileHandle, IntPtr ExistingCompletionPort,
            IntPtr CompletionKey, uint NumberOfConcurrentThreads);

        [DllImport("kernel32.dll", SetLastError = true)]
        public static extern bool GetQueuedCompletionStatus(IntPtr CompletionPort, out uint lpNumberOfBytes,
            out IntPtr lpCompletionKey, out IntPtr lpOverlapped, uint dwMilliseconds);

        [DllImport("kernel32.dll", SetLastError = true)]
        public static extern bool CreateProcess(string lpApplicationName, string lpCommandLine,
            IntPtr lpProcessAttributes,
            IntPtr lpThreadAttributes, bool bInheritHandles, CreateProcessFlags dwCreationFlags,
            [In] [MarshalAs(UnmanagedType.LPStr)] StringBuilder lpEnvironment,
            string lpCurrentDirectory, [In] ref STARTUPINFO lpStartupInfo, out PROCESS_INFORMATION lpProcessInformation);

        [DllImport("kernel32.dll", SetLastError = true)]
        public static extern int ResumeThread(IntPtr hThread);

        [DllImport("kernel32.dll", SetLastError = true)]
        public static extern IntPtr OpenProcess(ProcessAccessFlags dwDesiredAccess,
            [MarshalAs(UnmanagedType.Bool)] bool bInheritHandle, int dwProcessId);

        [DllImport("kernel32.dll", SetLastError = true)]
        public static extern uint WaitForSingleObject(IntPtr hHandle, uint dwMilliseconds);

        [DllImport("kernel32.dll", BestFitMapping = false, ThrowOnUnmappableChar = true)]
        public static extern int OpenFile(
            [MarshalAs(UnmanagedType.LPStr)] string
                lpFileName, out OFSTRUCT lpReOpenBuff,
            OpenFileStyle uStyle);

        [DllImport("kernel32.dll", SetLastError = true)]
        [return: MarshalAs(UnmanagedType.Bool)]
        public static extern bool TerminateProcess(IntPtr hProcess, uint uExitCode);

        [DllImport("kernel32.dll", SetLastError = true, CharSet = CharSet.Auto)]
        public static extern IntPtr CreateFileW(
            string lpFileName,
            EFileAccess dwDesiredAccess,
            EFileShare dwShareMode,
            IntPtr lpSecurityAttributes,
            ECreationDisposition dwCreationDisposition,
            EFileAttributes dwFlagsAndAttributes,
            IntPtr hTemplateFile);

        [DllImport("kernel32", SetLastError = true)]
        public static extern unsafe bool ReadFile
        (
            IntPtr hFile, // handle to file
            void* pBuffer, // data buffer
            int NumberOfBytesToRead, // number of bytes to read
            int* pNumberOfBytesRead, // number of bytes read
            int Overlapped // overlapped buffer
        );

        [DllImport("kernel32.dll", SetLastError = true)]
        public static extern bool SetHandleInformation(IntPtr hObject, HANDLE_FLAGS dwMask,
            HANDLE_FLAGS dwFlags);

        [DllImport("Kernel32", ExactSpelling = true, SetLastError = true)]
        [return: MarshalAs(UnmanagedType.Bool)]
        public static extern bool QueryProcessCycleTime(IntPtr processHandle, out ulong CycleTime);

        [DllImport("Kernel32", ExactSpelling = true, SetLastError = true)]
        [return: MarshalAs(UnmanagedType.Bool)]
        public static extern bool QueryThreadCycleTime(IntPtr threadHandle, out ulong CycleTime);

        [DllImport("kernel32.dll", CharSet = CharSet.Auto, SetLastError = true)]
        public static extern bool CreatePipe(out IntPtr hReadPipe, out IntPtr hWritePipe,
            ref SECURITY_ATTRIBUTES lpPipeAttributes, int nSize);


        [DllImport("kernel32.dll")]
        [return: MarshalAs(UnmanagedType.Bool)]
        public static extern unsafe bool WriteFile(IntPtr hFile, byte* lpBuffer,
            uint nNumberOfBytesToWrite, out uint lpNumberOfBytesWritten,
            IntPtr lpOverlapped);

        [DllImport("kernel32", SetLastError = true)]
        public static extern void GetSystemInfo(out SYSTEM_INFO lpSystemInfo);

        [DllImport("kernel32.dll")]
        public static extern bool SetProcessAffinityMask(IntPtr hProcess,
            UIntPtr dwProcessAffinityMask);

        [DllImport("psapi.dll", SetLastError = true)]
        public static extern bool GetProcessMemoryInfo(IntPtr hProcess, out PROCESS_MEMORY_COUNTERS counters, int size);

        [DllImport("kernel32.dll", SetLastError = true)]
        [return: MarshalAs(UnmanagedType.Bool)]
        public static extern bool QueryPerformanceFrequency(out ulong frequency);

        [DllImport("kernel32.dll", SetLastError = true)]
        [return: MarshalAs(UnmanagedType.Bool)]
        public static extern bool DuplicateHandle(IntPtr hSourceProcessHandle,
            IntPtr hSourceHandle, IntPtr hTargetProcessHandle, out IntPtr lpTargetHandle,
            uint dwDesiredAccess, [MarshalAs(UnmanagedType.Bool)] bool bInheritHandle, uint dwOptions);

        [DllImport("kernel32.dll", SetLastError = true)]
        [return: MarshalAs(UnmanagedType.Bool)]
        public static extern bool GetExitCodeProcess(IntPtr hProcess, out uint lpExitCode);

        [DllImport("kernel32.dll", SetLastError = true)]
        public static extern IntPtr GetStdHandle(int nStdHandle);


        /* Win32 API helper methods */

        public static void TryCloseHandle(IntPtr handle)
        {
            if (handle != INVALID_HANDLE_VALUE)
                try
                {
                    CloseHandle(handle);
                }
                catch
                {
                }
        }

        public static void TryCloseHandle(ref IntPtr handle)
        {
            if (handle != INVALID_HANDLE_VALUE)
                try
                {
                    CloseHandle(handle);
                    handle = INVALID_HANDLE_VALUE;
                }
                catch
                {
                }
        }

        public static bool CheckResult(bool result)
        {
            if (!result)
                throw new Win32Exception();
            return result;
        }

        public static IntPtr CheckResult(IntPtr result)
        {
            if (result == IntPtr.Zero)
                throw new Win32Exception();
            return result;
        }

        public static int CheckResult(int result)
        {
            if (result == -1)
                throw new Win32Exception();
            return result;
        }

        [StructLayout(LayoutKind.Sequential)]
        public struct PROCESS_MEMORY_COUNTERS
        {
            public uint cb;
            public uint PageFaultCount;
            public UIntPtr PeakWorkingSetSize;
            public UIntPtr WorkingSetSize;
            public UIntPtr QuotaPeakPagedPoolUsage;
            public UIntPtr QuotaPagedPoolUsage;
            public UIntPtr QuotaPeakNonPagedPoolUsage;
            public UIntPtr QuotaNonPagedPoolUsage;
            public UIntPtr PagefileUsage;
            public UIntPtr PeakPagefileUsage;
        }


        public abstract class HANDLE
        {
            protected IntPtr readHandle = INVALID_HANDLE_VALUE;
            protected IntPtr writeHandle = INVALID_HANDLE_VALUE;
            public IntPtr Handle { get; protected set; }

            public bool IsOpen
            {
                get { return (Handle != IntPtr.Zero) && (Handle != INVALID_HANDLE_VALUE); }
            } //todo?

            public bool IsValid
            {
                get { return (Handle != IntPtr.Zero) && (Handle != INVALID_HANDLE_VALUE); }
            }

            public virtual IntPtr ReadHandle => readHandle;
            public virtual IntPtr WriteHandle => writeHandle;
            //public abstract IntPtr Open();
            public abstract IntPtr OpenRead();
            public abstract IntPtr OpenWrite();

            public virtual void CloseRead()
            {
                TryCloseHandle(ref readHandle);
            }

            public virtual void CloseWrite()
            {
                TryCloseHandle(ref writeHandle);
            }

            public virtual void Close()
            {
                TryCloseHandle(Handle);
                CloseRead();
                CloseWrite();
                //this.Handle = IntPtr.Zero;
            }

            ~HANDLE()
            {
                try
                {
                    Close();
                }
                catch
                {
                }
            }
        }

        public class INVALID_HANDLE : HANDLE
        {
            public INVALID_HANDLE()
            {
                Handle = INVALID_HANDLE_VALUE;
            }

            public override IntPtr OpenRead()
            {
                return INVALID_HANDLE_VALUE;
            }

            public override IntPtr OpenWrite()
            {
                return INVALID_HANDLE_VALUE;
            }

            public override void Close()
            {
                //do nothing
                Handle = IntPtr.Zero;
            }
        }

        public class FILE : HANDLE
        {
            //public override IntPtr ReadHandle { get { return this._readHandle; } }
            //public override IntPtr WriteHandle { get { return this._writeHandle; } }
            //private IntPtr _readHandle = IntPtr.Zero, _writeHandle = IntPtr.Zero;


            public FILE(string path)
            {
                Path = path;
            }

            public string Path { get; }

            public override IntPtr OpenRead()
            {
                Handle =
                    readHandle =
                        CreateFileW(Path, EFileAccess.GenericRead, EFileShare.Read | EFileShare.Write,
                            IntPtr.Zero, ECreationDisposition.OpenExisting, EFileAttributes.Normal, IntPtr.Zero);
                SetHandleInformation(Handle, HANDLE_FLAGS.INHERIT, HANDLE_FLAGS.INHERIT);
                return Handle;
            }

            public override IntPtr OpenWrite()
            {
                return OpenWrite(ECreationDisposition.CreateAlways);
            }

            public IntPtr OpenWrite(ECreationDisposition method)
            {
                Handle =
                    writeHandle =
                        CreateFileW(Path, EFileAccess.GenericWrite, EFileShare.Read | EFileShare.Write,
                            IntPtr.Zero, method, EFileAttributes.Normal, IntPtr.Zero);
                SetHandleInformation(Handle, HANDLE_FLAGS.INHERIT, HANDLE_FLAGS.INHERIT);
                return Handle;
            }

            public override void Close()
            {
                TryCloseHandle(writeHandle);
                TryCloseHandle(readHandle);
                //this._writeHandle = IntPtr.Zero;
                //this._readHandle = IntPtr.Zero;
            }
        }

        public class ANON_PIPE : HANDLE
        {
            private bool _open;

            private bool _openRead, _openWrite;

            public ANON_PIPE(int BufferSize = 8192)
            {
                this.BufferSize = BufferSize;
            }

            public new bool IsOpen
            {
                get { return base.IsOpen && _open; }
            }

            public override IntPtr ReadHandle
            {
                get { return _openRead ? readHandle : INVALID_HANDLE_VALUE; }
            }

            public override IntPtr WriteHandle
            {
                get { return _openWrite ? writeHandle : INVALID_HANDLE_VALUE; }
            }

            public int BufferSize { get; }


            public unsafe int Read(byte[] buffer, int index, int count)
            {
                if (!IsOpen)
                    return 0;
                var n = 0;
                fixed (byte* p = buffer)
                {
                    if (!ReadFile(readHandle, p + index, count, &n, 0))
                        return 0;
                }
                return n;
            }

            public string ReadToEnd()
            {
                var str = "";
                var read = 0;
                var buf = new byte[8192];
                while ((read = Read(buf, 0, buf.Length)) > 0)
                    str += Encoding.UTF8.GetString(buf, 0, read);
                return str;
            }


            public unsafe bool Write(byte[] buf, uint len)
            {
                fixed (byte* buffer = &buf[0])
                {
                    var _buf = buffer;
                    while (len > 0)
                    {
                        uint wrtn;
                        if (!WriteFile(writeHandle, _buf, len, out wrtn, IntPtr.Zero))
                            return false;
                        _buf += wrtn;
                        len -= wrtn;
                    }
                    return true;
                }
            }

            public bool Write(string str)
            {
                var buf = Encoding.UTF8.GetBytes(str);
                return Write(buf, (uint)buf.LongLength);
            }

            public void Open()
            {
                if (_open)
                    return;
                var sa = new SECURITY_ATTRIBUTES();
                sa.nLength = (uint)Marshal.SizeOf(sa);
                sa.lpSecurityDescriptor = IntPtr.Zero;
                sa.bInheritHandle = true;
                CheckResult(CreatePipe(out readHandle, out writeHandle, ref sa, BufferSize));
                SetHandleInformation(readHandle, HANDLE_FLAGS.INHERIT, HANDLE_FLAGS.INHERIT);
                SetHandleInformation(writeHandle, HANDLE_FLAGS.INHERIT, HANDLE_FLAGS.INHERIT);
                _open = _openRead = _openWrite = true;
            }

            public override IntPtr OpenRead()
            {
                Open();
                return Handle = readHandle;
            }

            public override IntPtr OpenWrite()
            {
                Open();
                return Handle = writeHandle;
            }

            public override void CloseRead()
            {
                if (!_openRead) return;
                _openRead = false;
                TryCloseHandle(ref readHandle);
            }

            public override void CloseWrite()
            {
                if (!_openWrite) return;
                _openWrite = false;
                TryCloseHandle(ref writeHandle);
            }

            public override void Close()
            {
                try
                {
                    if (_openWrite)
                        TryCloseHandle(ref writeHandle);
                    if (_openRead)
                        TryCloseHandle(ref readHandle);
                }
                catch
                {
                }
            }
        }
    }
}