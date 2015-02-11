using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.InteropServices;
using System.IO;
using System.Diagnostics;

namespace lolcomjector
{

    public class Injector
    {
        [DllImport("kernel32.dll", SetLastError = true)]
        static extern IntPtr OpenProcess(uint dwDesiredAccess, int bInheritHandle, uint dwProcessId);

        [DllImport("kernel32.dll", SetLastError = true)]
        static extern int CloseHandle(IntPtr hObject);

        [DllImport("kernel32.dll", SetLastError = true)]
        static extern IntPtr GetProcAddress(IntPtr hModule, string lpProcName);

        [DllImport("kernel32.dll", SetLastError = true)]
        static extern IntPtr GetModuleHandle(string lpModuleName);

        [DllImport("kernel32.dll", SetLastError = true)]
        static extern IntPtr VirtualAllocEx(IntPtr hProcess, IntPtr lpAddress, int dwSize, uint flAllocationType, uint flProtect);

        [DllImport("kernel32.dll", SetLastError = true)]
        static extern int WriteProcessMemory(IntPtr hProcess, IntPtr lpBaseAddress, byte[] buffer, uint size, out IntPtr lpNumberOfBytesWritten);

        [DllImport("kernel32.dll", SetLastError = true)]
        static extern IntPtr CreateRemoteThread(IntPtr hProcess, IntPtr lpThreadAttribute, IntPtr dwStackSize, IntPtr lpStartAddress, IntPtr lpParameter, uint dwCreationFlags, out IntPtr lpThreadId);

        [DllImport("kernel32.dll", SetLastError = true)]
        static extern int WaitForSingleObject(IntPtr hHandle, int dwMilliseconds);

        static Injector instance = null;
        private Injector()
        {

        }
        public static Injector GetInstnace()
        {
            if (instance == null)
                instance = new Injector();
            return instance;
        }
        //TODO: EnumInjectableModules
        public bool AlreadyInjected(Process p, string dllPath)
        {
            if (!File.Exists(dllPath))
                throw new InvalidDllPath("dll does not exist");
            string[] parts = dllPath.Split(new char[] { '\\' });
            foreach (ProcessModule m in p.Modules)
            {
                if (m.ModuleName == parts[parts.Length - 1])
                    return true;
            }
            return false;
        }
        public void Inject(string processName, string dllPath)
        {
            dllPath = Path.GetFullPath(dllPath); //make sure it is full path
            uint pid = 0;
            foreach (Process p in Process.GetProcesses())
            {
                if (p.ProcessName == processName)
                {
                    pid = (uint)p.Id;
                    if (AlreadyInjected(p, dllPath))
                        throw new AlreadyInjected("dll already injected.");
                    break;
                }
            }
            
            if (pid == 0)
                throw new InvalidProcess("process not found");
            //now we will make the thread to call LoadLibrary with our dll
            IntPtr lolPtr = OpenProcess(2035711, 0, pid);
            IntPtr LoadLibraryAddr = GetProcAddress(GetModuleHandle("kernel32.dll"), "LoadLibraryA");
            IntPtr addr = VirtualAllocEx(lolPtr, (IntPtr)null, 256, (0x2000 | 0x1000), 0x40);
            byte[] dllNameAscii = Encoding.ASCII.GetBytes(dllPath);
            IntPtr bytesout;
            WriteProcessMemory(lolPtr, addr, dllNameAscii, (uint)dllNameAscii.Length, out bytesout);
            IntPtr thread = CreateRemoteThread(lolPtr, (IntPtr)null, (IntPtr)0, LoadLibraryAddr, addr, 0, out bytesout);
            int Result = WaitForSingleObject(thread, 10 * 1000);
            CloseHandle(lolPtr);
        }
    }
}
