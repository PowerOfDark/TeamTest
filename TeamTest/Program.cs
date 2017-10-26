using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading;
using ProcessLib;
using TeamTest.Adapters;
using Win32Lib;
using TeamTest.OI2017;

namespace TeamTest
{
    internal class Program
    {
        public static string ExeDir = Path.Combine(Environment.CurrentDirectory, "exe");
        public static List<FileInfo> Executables = new List<FileInfo>();
        public static string InputDir = Path.Combine(Environment.CurrentDirectory, "in");
        public static string OutputDir = Path.Combine(Environment.CurrentDirectory, "out");
        public static uint MaxMem = 128 * 1024 * 1024;
        public static long MaxTime = 15 * 1000;
        public static string JobName = "rep";
        public static string MasterExecutable;
        public static ulong Mismatches;
        public static Dictionary<string, ulong> TotalWall = new Dictionary<string, ulong>();
        public static Dictionary<string, List<string>> Errors = new Dictionary<string, List<string>>();
        public static ITest JOB;
        public static Queue<TestInfo> QueuedTests = new Queue<TestInfo>();

        public static AutoResetEvent GenHandle = new AutoResetEvent(false);
        public static AutoResetEvent WorkerHandle = new AutoResetEvent(false);

        public static int Tests_OK;
        public static int Tests_BAD;

        public static int CurrentTestID;
        public static Thread GeneratorThread;
        public static Thread TesterThread;
        public static bool _runTesterThread = true;
        public static bool _runGeneratorThread = true;

        private static void Main(string[] args)
        {
            var dir = new DirectoryInfo("exe");
            if (!dir.Exists)
            {
                dir.Create();
            }
            var exes = dir.EnumerateFiles("*.exe");
            var exestr = string.Join(", ", exes.Select(t => t.Name));
            if (exes.Any())
            {
                CHOOSE_MASTER:
                ;
                Console.WriteLine($"Detected {exes.Count()} files: {exestr}");
                Console.Write("Enter master executable: ");
                MasterExecutable = Console.ReadLine();
                var m = exes.FirstOrDefault(t => t.Name == MasterExecutable);
                if (m == null)
                {
                    Console.WriteLine("Invalid master executable.");
                    goto CHOOSE_MASTER;
                }
                Executables.Add(m);
                foreach (var exe in exes)
                {
                    TotalWall.Add(exe.Name, 0);
                    if (exe.Name == MasterExecutable)
                    {
                        continue;
                    }
                    Executables.Add(exe);
                }
            }
            else
            {
                Console.WriteLine("Put at least two executables in ./exe directory");
                Environment.Exit(-1);
            }
            Directory.CreateDirectory(OutputDir);

            StartJob(new PIO(new IntRangeValue(-100, 100), new IntRangeValue(-100, 100), new IntRangeValue(1000, 20000)));

            string s;
            while ((s = Console.ReadLine()) != "stop")
            {
            }
            _runGeneratorThread = false;
            _runTesterThread = false;
            TesterThread.Join();
        }

        public static void StartJob(ITest job)
        {
            JOB = job;
            JobName = job.JobName;
            Console.WriteLine($"-----------BEGIN JOB {JobName}----------");
            GeneratorThread = new Thread(() => { GenerateTests(10); });
            TesterThread = new Thread(() => { TestWorker(); });
            //new Thread(() => { TestWorker(); }).Start();
            //new Thread(() => { TestWorker(); }).Start();
            //new Thread(() => { TestWorker(); }).Start();
            GeneratorThread.Start();
            TesterThread.Start();
        }

        public static void TestWorker()
        {
            TestInfo test;
            bool ok;
            while (_runTesterThread)
            {
                test = null;
                lock (QueuedTests)
                {
                    if (QueuedTests.Count > 0)
                    {
                        test = QueuedTests.Dequeue();
                    }
                }
                GenHandle.Set();
                if (test != null)
                {
                    ok = SingleTest(test);
                    if (ok)
                    {
                        Interlocked.Increment(ref Tests_OK);
                        try
                        {
                            File.Delete(test.File);
                        }
                        catch
                        {
                        }
                    }
                    else
                    {
                        Interlocked.Increment(ref Tests_BAD);
                    }
                }
                else
                {
                    WorkerHandle.WaitOne();
                }
            }
        }

        public static void GenerateTests(int keepN)
        {
            TestInfo test;
            while (_runGeneratorThread)
            {
                if (!JOB.HasNext(CurrentTestID))
                {
                    Console.WriteLine($"[GEN-WORKER] Job {JOB.JobName} ended @ {CurrentTestID}");
                    _runGeneratorThread = false;
                    break;
                }
                if (QueuedTests.Count < keepN)
                {
                    test = new TestInfo(CurrentTestID, JOB.GenerateTest(CurrentTestID, InputDir));
                    lock (QueuedTests)
                    {
                        QueuedTests.Enqueue(test);
                    }
                    WorkerHandle.Set();
                    Interlocked.Increment(ref CurrentTestID);
                }
                else
                {
                    GenHandle.WaitOne();
                }
            }
        }

        public static string GetTitle()
        {
            return $"JOB: {JobName} | OK: {Tests_OK} | BAD: {Tests_BAD}";
        }

        public static bool SingleTest(TestInfo test)
        {
            var anything = false;
            var ok = true;
            var currentTestName = $"{JobName}{test.ID}";
            var testPath = test.File;
            var input = new Win32.FILE(testPath);
            var processes = Test(testPath, currentTestName);
            var testOutputDir = Path.Combine(OutputDir, currentTestName);
            var MasterMD5 = "";
            // Console.WriteLine($"-----------BEGIN TEST {currentTestName}----------");
            var OutputMd5s = new Dictionary<string, List<FileInfo>>();
            Console.Title = $"Running test {JobName}_{test.ID} | {GetTitle()}";

            for (var i = 0; i < Executables.Count; i++)
            {
                var p = processes[i];
                var e = Executables[i];
                p.WaitForExit();
                Thread.Sleep(5);
                processes[i].StartInfo.StdIO.StandardOutput.Close();
                processes[i].StartInfo.StdIO.StandardInput.Close();

                var maxMem = (uint) p.GetMemoryCounters().Value.PeakWorkingSetSize;
                var outputPath = Path.Combine(testOutputDir, $"{currentTestName}_{Executables[i].Name}.out");
                var outputmd5 = md5(outputPath);
                if (Executables[i].Name == MasterExecutable)
                {
                    MasterMD5 = outputmd5;
                }
                if (!OutputMd5s.ContainsKey(outputmd5))
                {
                    OutputMd5s.Add(outputmd5, new List<FileInfo>());
                    File.Move(outputPath, Path.Combine(testOutputDir, outputmd5 + ".out"));
                }
                OutputMd5s[outputmd5].Add(Executables[i]);
                //File.Delete(outputPath);

                //if (outputmd5 != "NIE")
                //{
                //    _Ile_Razy_bylo_cokolwiek_innego_niz_NIE++;
                //}

                var error = p.OutOfMemory || p.OutOfTime || p.ExitCode.GetValueOrDefault(0xBAD) != 0;
                //if (error || outputmd5 != MasterMD5)
                {
                    anything = true;
                    TotalWall[Executables[i].Name] += (ulong) p.Clock.TotalMilliseconds;
                    Console.WriteLine($"-----------BEGIN TEST {currentTestName}----------");
                    Console.ForegroundColor = error ? ConsoleColor.Red : ConsoleColor.Green;
                    var prefix = Executables[i].Name == MasterExecutable ? " [MASTER] " : " ";
                    Console.WriteLine($"--{prefix}{e.Name}");
                    Console.ForegroundColor = p.OutOfMemory || p.OutOfTime ? ConsoleColor.Red : ConsoleColor.Green;
                    Console.WriteLine($"\t OutOfMemory: {p.OutOfMemory} OutOfTime: {p.OutOfTime}");
                    Console.ForegroundColor = maxMem > MaxMem ? ConsoleColor.Red : ConsoleColor.Green;
                    Console.WriteLine($"\t Memory: {maxMem / 1024}/{MaxMem / 1024}");
                    Console.ForegroundColor = (int) p.Clock.TotalMilliseconds > MaxTime || (int) p.FixedProcessTime > MaxTime
                        ? ConsoleColor.Red
                        : ConsoleColor.Green;
                    Console.WriteLine(
                        $"\t Wall time: {(int) p.Clock.TotalMilliseconds}ms/{MaxTime}ms FIXED time: {(int) p.FixedProcessTime}ms/{MaxTime}ms");
                    Console.ForegroundColor = p.ExitCode != 0 ? ConsoleColor.Red : ConsoleColor.Green;
                    Console.WriteLine($"\t ExitCode: {p.ExitCode} " + (p.ExitCode == 0 ? "OK" : "ERROR"));
                    Console.ResetColor();
                    Console.WriteLine($"\t Output: {outputmd5}");
                }


                if (error || outputmd5 != MasterMD5)
                {
                    if (!Errors.ContainsKey(Executables[i].Name))
                    {
                        Errors.Add(Executables[i].Name, new List<string>());
                    }
                    Errors[Executables[i].Name].Add(currentTestName);
                }
                //Console.ForegroundColor = (!outputOK) ? ConsoleColor.Red : ConsoleColor.Green;
                //Console.WriteLine($"\t Output: OK");
            }

            if (OutputMd5s.Count == 1)
            {
                //Console.ForegroundColor = ConsoleColor.Green;
                //Console.WriteLine("\t\t------ALL OUTPUTS MATCH------");
                ok = true;
                var targetTest = Path.Combine(OutputDir, $"{currentTestName}.out");
                if (File.Exists(targetTest))
                {
                    File.Delete(targetTest);
                }
                File.Delete(Path.Combine(testOutputDir, $"{MasterMD5}.out"));
                //File.Move(Path.Combine(testOutputDir, $"{MasterMD5}.out"), targetTest);
                Directory.Delete(testOutputDir, true);
            }
            else
            {
                ok = false;
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine("\t\t------OUTPUTS DO NOT MATCH!------");
                ++Mismatches;
                Console.ResetColor();
                var md5s = OutputMd5s.Keys.OrderBy(t => OutputMd5s[t].Count);
                File.Copy(testPath, Path.Combine(testOutputDir, $"{currentTestName}.in"), true);
                foreach (var key in md5s)
                {
                    var list = OutputMd5s[key];
                    var str = string.Join(", ", list);
                    var file = string.Join("_", list.Select(t => t.Name.Replace(".exe", ""))) + ".out";
                    Console.ForegroundColor = key == MasterMD5 ? ConsoleColor.Green : ConsoleColor.DarkYellow;
                    File.Move(Path.Combine(testOutputDir, $"{key}.out"), Path.Combine(testOutputDir, file));
                    if (key == MasterMD5)
                    {
                        File.Copy(Path.Combine(testOutputDir, file), Path.Combine(OutputDir, $"{currentTestName}.out"), true);
                    }
                    Console.WriteLine($"{key}: {str}");
                }
                Console.ResetColor();
            }
            if (anything)
            {
                Console.WriteLine($"-----------END TEST {currentTestName}----------");
            }
            return ok;
            //Console.ResetColor();
        }

        public static string md5(string path)
        {
            while (!IsFileReady(path))
            {
                Thread.Sleep(10);
            }
            var _tmp = Path.GetTempFileName();

            using (var sw = new StreamWriter(_tmp, true))
            {
                string n;
                using (var sr = new StreamReader(path))
                {
                    while (!sr.EndOfStream)
                    {
                        n = sr.ReadLine().Trim();
                        if (n.Length > 0)
                        {
                            sw.WriteLine(n);
                        }
                    }
                }
            }

            string md;
            using (var md5 = MD5.Create())
            {
                using (var stream = File.Open(_tmp, FileMode.Open, FileAccess.Read, FileShare.Delete | FileShare.ReadWrite))
                {
                    if (JobName == "pio")
                    {
                        var c = new byte[256];
                        var read = stream.Read(c, 0, 256);
                        md = Encoding.UTF8.GetString(c, 0, read).Trim();
                    }
                    else
                    {
                        md = BitConverter.ToString(md5.ComputeHash(stream)).Replace("-", "").ToLower();
                    }
                }
            }
            File.Delete(_tmp);
            return md;
        }

        public static bool IsFileReady(string sFilename)
        {
            // If the file can be opened for exclusive access it means that the file
            // is no longer locked by another process.
            try
            {
                using (var inputStream = File.Open(sFilename, FileMode.Open, FileAccess.Read, FileShare.None))
                {
                    if (inputStream.Length > 0)
                    {
                        return true;
                    }
                    return false;
                }
            }
            catch (Exception)
            {
                return false;
            }
        }

        public static List<Process> Test(string path, string name)
        {
            var processes = new List<Process>();
            var outdir = Path.Combine(OutputDir, name);
            if (Directory.Exists(outdir))
            {
                //Directory.Delete(outdir, true);
                foreach (var file in new DirectoryInfo(outdir).EnumerateFiles("*.out"))
                {
                    try
                    {
                        file.Delete();
                    }
                    catch
                    {
                        Console.WriteLine("FUU");
                    }
                }
            }
            else
            {
                Directory.CreateDirectory(outdir);
            }
            for (var i = 0; i < Executables.Count; i++)
            {
                var _in = new Win32.FILE(path);
                var _out = new Win32.FILE(Path.Combine(outdir, $"{name}_{Executables[i].Name}.out"));
                var io = new ProcessIO(_in, _out);
                var psi = new ProcessStartInfo
                {
                    StartSuspended = false,
                    StdIO = io,
                    Executable = Executables[i].FullName
                }; // TotalTimeLimit = MaxTime
                var p = new Process(psi);
                processes.Add(p);
                p.Start();
            }

            return processes;
        }

        public class TestInfo
        {
            public string File;
            public int ID;

            public TestInfo(int ID, string File)
            {
                this.ID = ID;
                this.File = File;
            }
        }
    }
}