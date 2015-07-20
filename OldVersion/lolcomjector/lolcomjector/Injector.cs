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
        //TODO: EnumInjectableModules
        public static bool AlreadyInjected(Process p, string dllPath)
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
        public static void Inject(string processName, string dllPath)
        {
            dllPath = Path.GetFullPath(dllPath); //make sure it is full path
            Process lol = Process.GetProcesses().Where(p => p.ProcessName == processName).FirstOrDefault();
            if (lol == null)
                throw new InvalidProcess("process not found");
            int pid = lol.Id;
            //now we will make the thread to call LoadLibrary with our dll
            IntPtr lolPtr = Win32.OpenProcess(2035711, 0, pid);
            IntPtr LoadLibraryAddr = Win32.GetProcAddress(Win32.GetModuleHandle("kernel32.dll"), "LoadLibraryA");
            IntPtr addr = Win32.VirtualAllocEx(lolPtr, (IntPtr)null, 256, (0x2000 | 0x1000), 0x40);
            byte[] dllNameAscii = Encoding.ASCII.GetBytes(dllPath);
            IntPtr bytesout;
            Win32.WriteProcessMemory(lolPtr, addr, dllNameAscii, (uint)dllNameAscii.Length, out bytesout);
            IntPtr thread = Win32.CreateRemoteThread(lolPtr, (IntPtr)null, (IntPtr)0, LoadLibraryAddr, addr, 0, out bytesout);
            int Result = Win32.WaitForSingleObject(thread, 10 * 1000);
            Win32.CloseHandle(lolPtr);
        }
    }
}
